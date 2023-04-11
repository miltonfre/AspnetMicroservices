using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Discount.GRPC.Protos;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;
// Add services to the container.
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = Configuration["CacheSettings:ConnectionString"];
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IBasketRepository, BasketRepository>();

builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>
    (op=>op.Address=new Uri(Configuration["GrpcSettings:DiscountUrl"]));

builder.Services.AddScoped<DiscountGrpcService>();

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
