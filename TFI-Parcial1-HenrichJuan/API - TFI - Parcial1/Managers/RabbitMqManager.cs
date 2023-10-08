using Microsoft.AspNetCore.Connections;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace API___TFI___Parcial1.Managers
{
    public class RabbitMqManager
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqManager(string hostName, string userName, string password)
        {
            var factory = new ConnectionFactory
            {
                HostName = hostName,
                UserName = userName,
                Password = password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void DeclareQueue(string queueName)
        {
            _channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }

        public void SendMessage(string queueName, object message)
        {
            var properties = message.GetType().GetProperties();
            var basicProperties = _channel.CreateBasicProperties();
            foreach (var property in properties)
            {
                if (property.Name == "Prioridad")
                {
                    basicProperties.Persistent = true;
                    basicProperties.Priority = Convert.ToByte(property.GetValue(message));
                }
            }

            string serializedObject = JsonConvert.SerializeObject(message);

            var body = Encoding.UTF8.GetBytes(serializedObject);
            _channel.BasicPublish(exchange: "",
                                  routingKey: queueName,
                                  basicProperties: basicProperties,
                                  body: body);
            Console.WriteLine($"[x] Enviado '{message}' a '{queueName}'");
        }

        public async Task<string> ConsumeMessages(string queueName)
        {
            var tcs = new TaskCompletionSource<string>();
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var mensaje = Encoding.UTF8.GetString(ea.Body.ToArray());
                    tcs.SetResult(mensaje);
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {

                    _channel.BasicReject(ea.DeliveryTag, false);
                }

            };

            _channel.BasicConsume(queue: queueName,
                                  autoAck: false,
                                  consumer: consumer);

            return await tcs.Task;
        }


        public void Close()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
