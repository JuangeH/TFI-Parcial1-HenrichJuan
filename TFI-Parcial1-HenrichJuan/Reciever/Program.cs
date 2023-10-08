using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Reciever.Managers;
using Reciever.Models;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reciever.Services;

var builder = new ConfigurationBuilder();
builder.SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var Configuration = builder.Build();

var host = new HostBuilder()
    .ConfigureHostConfiguration(configHost => {
    })
    .ConfigureServices((hostContext, services) => {
        string username = Configuration.GetValue<string>("RabbitMq:Username");
        string password = Configuration.GetValue<string>("RabbitMq:Password");
        string host = Configuration.GetValue<string>("RabbitMq:Host");
        services.AddTransient<RabbitMqWorkerService>(x=> new RabbitMqWorkerService(new RabbitMqManager(host,username,password)));
        services.AddTransient<RabbitMqManager>(x => new(host, username, password));
        services.AddHostedService<RabbitMqWorkerService>();
    })
    .UseConsoleLifetime()
    .Build();


host.Run();

