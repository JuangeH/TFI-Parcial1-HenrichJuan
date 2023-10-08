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

        private object ModifyMessage(string message)
        {
            // Implementa la lógica para modificar el mensaje según sea necesario
            // Por ejemplo, puedes deserializar el mensaje, modificar un atributo y luego volver a serializarlo
            // Aquí hay un ejemplo básico de cómo podrías modificar un atributo (Prioridad) del mensaje:
            var deserializedMessage = JsonConvert.DeserializeObject<FileDataModel>(message);
            // Modifica el atributo Prioridad
            deserializedMessage.Prioridad = 1;
            deserializedMessage.FechaImpresion = DateTime.Now;
            // Devuelve el mensaje modificado
            return deserializedMessage;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _rabbitMqManager.DeclareQueue("ReceiverQueue");

                try
                {
                    var message = await _rabbitMqManager.ConsumeMessages("SenderQueue");
                }
                catch (Exception ex)
                {
                    // Manejar cualquier excepción que ocurra durante la recepción o procesamiento del mensaje
                    Console.WriteLine($"Error: {ex.Message}");
                }
            
        }
    }
}
