using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Reciever.Managers;
using Reciever.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Reciever.Services
{
    public class RabbitMqWorkerService : BackgroundService
    {
        private readonly RabbitMqManager _rabbitMqManager;

        public RabbitMqWorkerService(RabbitMqManager rabbitMqManager)
        {
            _rabbitMqManager = rabbitMqManager;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await _rabbitMqManager.ConsumeMessages("SenderQueue");
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que ocurra durante la recepción o procesamiento del mensaje
                Console.WriteLine($"Error: {ex.Message}");
            }
            
        }
    }
}
