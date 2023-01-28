using PublicApi.AdjustedPrices.Services;
using PublicApi.FxPrices.Services;
using PublicApi.MultiplePrices.Services;
using PublicApi.RecordReaders;
using PublicApi.Repository;
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
    services.AddSingleton<IDynamoRepository, DynamoRepository>();
}