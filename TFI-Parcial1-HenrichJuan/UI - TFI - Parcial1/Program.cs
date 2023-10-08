using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using UI___TFI___Parcial1.Data;
using MudBlazor.Services;
using UI___TFI___Parcial1.Helpers.Contracs;
using UI___TFI___Parcial1.Helpers.Service;
using UI___TFI___Parcial1.Services;
using Microsoft.Extensions.Configuration;
using UI___TFI___Parcial1.Managers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices(); 
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IDapper, DapperHelper>(); // Registra Dapper
string username = builder.Configuration.GetValue<string>("RabbitMq:Username");
string password = builder.Configuration.GetValue<string>("RabbitMq:Password");
string host = builder.Configuration.GetValue<string>("RabbitMq:Host");
var serviceProvider = builder.Services.BuildServiceProvider();
builder.Services.AddTransient<PrintResponseService>(x => new PrintResponseService(new RabbitMqManager(host, username, password,serviceProvider.GetRequiredService<IDapper>())));
builder.Services.AddTransient<RabbitMqManager>(x => new(host, username, password, serviceProvider.GetRequiredService<IDapper>()));
builder.Services.AddHostedService<PrintResponseService>();
builder.Services.AddSingleton<HttpClient>(sp =>
{
    var httpClient = new HttpClient();
    httpClient.BaseAddress = new Uri("https://localhost:44369");
    return httpClient;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
