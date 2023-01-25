using PublicApi.RecordReaders;
using PublicApi.Repository;
using PublicApi.Services;
using PublicApi.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IMultiplePricesReader, MultiplePricesReader>();
builder.Services.AddScoped(typeof(IRecordsReader<>), typeof(RecordsReader<>));
builder.Services.AddScoped<IDynamoRepository, DynamoRepository>();

builder.Services.Configure<MultiplePricesSettings>(builder.Configuration.GetSection("MultiplePricesSettings"));



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
