using Microsoft.AspNetCore.Identity;
using PublicApi.AdjustedPrices.Services;
using PublicApi.FxPrices.Services;
using PublicApi.MultiplePrices.Items;
using PublicApi.MultiplePrices.Services;
using PublicApi.RecordReaders;
using PublicApi.Repository;
using PublicApi.Repository.TableSeed;
using PublicApi.RollCalendars.Services;
using PublicApi.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

ConfigureServices(builder.Services);

builder.Services.Configure<LocalFoldersSettings>(builder.Configuration.GetSection("LocalFoldersSettings"));
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));
var app = builder.Build();

app.Logger.LogInformation("PublicApi App created...");
app.Logger.LogInformation("Seeding Database...");

using (var scope = app.Services.CreateScope())
{
    var scopedProvider = scope.ServiceProvider;
    try
    {
        var catalogContext = scopedProvider.GetRequiredService<IDynamoDBRepository<MultiplePricesItem>>();
        await TableSeedService.SeedAsync(catalogContext, app.Logger);
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred seeding the DB.");
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();

static void ConfigureServices(IServiceCollection services)
{
    services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped);
    services.AddScoped<IFxPricesServices, FxPricesServices>();
    services.AddScoped<IAdjustedPricesService, AdjustedPricesService>();
    services.AddScoped<IMultiplePricesService, MultiplePricesService>();
    services.AddScoped<IRollCalendarServices, RollCalendarServices>();

    services.AddScoped(typeof(IRecordsReader<>), typeof(RecordsReader<>));

    services.AddSingleton(typeof(IDynamoDBRepository<>), typeof(DynamoDBRepository<>));
} 