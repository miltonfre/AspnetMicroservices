using Ordering.Infraestructure;
using Ordering.Application;
using Ordering.API.Extensions;
using Ordering.Infraestructure.Persistance;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(Configuration);

var app = builder.Build();

app.MigrateDatabase<OrderContext>(
    (context,services) =>
    {
        var logger=services.GetService<ILogger<OrderContextSeed>>();
       OrderContextSeed.SeedAsync(context, logger).Wait(); 
    });
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
