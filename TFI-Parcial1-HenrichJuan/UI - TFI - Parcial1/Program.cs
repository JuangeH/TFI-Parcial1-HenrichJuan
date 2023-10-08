using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using UI___TFI___Parcial1.Data;
using MudBlazor.Services;
using UI___TFI___Parcial1.Helpers.Contracs;
using UI___TFI___Parcial1.Helpers.Service;
using UI___TFI___Parcial1.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
builder.Services.AddSingleton<WeatherForecastService>(); 
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IDapper, DapperHelper>(); // Registra Dapper
//builder.Services.AddScoped<ILoggingService, LoggingService>(); // Registra el servicio de registro
builder.Services.AddSingleton<IHostedService, PrintResponseService>();
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
