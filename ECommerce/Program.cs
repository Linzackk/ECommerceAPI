using ECommerce.Data;
using ECommerce.Middlewares;
using ECommerce.Repositories.Usuarios;
using ECommerce.Services.Usuarios;
using Microsoft.EntityFrameworkCore;
using System;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUsuariosRepository, UsuariosRepository>();
builder.Services.AddScoped<IUsuariosService, UsuariosService>();

DotNetEnv.Env.Load();

var DbConnection = Environment.GetEnvironmentVariable("DB_CONNECTION");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(DbConnection));

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
