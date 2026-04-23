using ECommerce.Authorization.Handlers;
using ECommerce.Authorization.Policies;
using ECommerce.Data;
using ECommerce.Middlewares;
using ECommerce.Repositories.Itens;
using ECommerce.Repositories.Logins;
using ECommerce.Repositories.Pedidos;
using ECommerce.Repositories.Usuarios;
using ECommerce.Services.Itens;
using ECommerce.Services.Logins;
using ECommerce.Services.Pedidos;
using ECommerce.Services.Tokens;
using ECommerce.Services.Usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IsAdmin", policy =>
        policy.RequireClaim("isAdmin", "true"));

    options.AddPolicy("IsAccountOwnerOrAdmin", policy =>
        policy.AddRequirements(new AccountOwnerOrAdminRequirement()));

    options.AddPolicy("IsOrderOwnerOrAdmin", policy =>
        policy.AddRequirements(new OrderOwnerOrAdminRequirement()));
});

builder.Services.AddScoped<IUsuariosRepository, UsuariosRepository>();
builder.Services.AddScoped<IUsuariosService, UsuariosService>();

builder.Services.AddScoped<ILoginRepository, LoginRepository>();
builder.Services.AddScoped<ILoginService, LoginService>();

builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IItemService, ItemService>();

builder.Services.AddScoped<IPedidosRepository, PedidosRepository>();
builder.Services.AddScoped<IPedidoService, PedidoService>();

builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<IAuthorizationHandler, AccountOwnerOrAdminHandler>();
builder.Services.AddScoped<IAuthorizationHandler, OrderOwnerOrAdminHandler>();
builder.Services.AddHttpContextAccessor();

DotNetEnv.Env.Load();

if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("TestDb"));
}
else
{
    var DbConnection = Environment.GetEnvironmentVariable("DB_CONNECTION");

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(DbConnection));

}

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

public partial class Program { }