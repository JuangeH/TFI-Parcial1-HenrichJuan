using Dapper;
using Newtonsoft.Json;
using System.Data;
using UI___TFI___Parcial1.Data;
using UI___TFI___Parcial1.Helpers.Contracs;
using UI___TFI___Parcial1.Managers;
using static MudBlazor.CategoryTypes;

namespace UI___TFI___Parcial1.Services
{
    public class PrintResponseService : BackgroundService
    {
        private readonly RabbitMqManager _rabbitMqManager;

        public PrintResponseService(RabbitMqManager rabbitMqManager)
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
                await _rabbitMqManager.ConsumeMessages("ReceiverQueue");
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que ocurra durante la recepción o procesamiento del mensaje
                Console.WriteLine($"Error: {ex.Message}");
            }

        }
    }
}
