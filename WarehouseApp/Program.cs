using APBD_Zadanie_6.Repositories;
using APBD_Zadanie_6.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IWarehouseService, WarehouseService>();
builder.Services.AddTransient<IWarehouseRepository, WarehouseRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();