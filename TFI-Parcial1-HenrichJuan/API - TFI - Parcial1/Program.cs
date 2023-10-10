using BLL.Contracts;
using BLL.Services;
using DAL.Contracts;
using DAL.Helper.Contract;
using DAL.Helper.Service;
using DAL.Repos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//Dapper
builder.Services.AddSingleton<IDapper, DapperHelper>();
//Gestores
builder.Services.AddSingleton<IGestorDocService, GestorDocService>();
//Repos
builder.Services.AddSingleton<IGestorDocRepo, GestorDocRepo>();

var app = builder.Build();

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
