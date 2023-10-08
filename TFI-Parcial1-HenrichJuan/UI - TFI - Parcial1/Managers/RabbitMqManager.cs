using Dapper;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using UI___TFI___Parcial1.Data;
using UI___TFI___Parcial1.Helpers.Contracs;
using static MudBlazor.CategoryTypes;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace UI___TFI___Parcial1.Managers
{
    public class RabbitMqManager
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IDapper _dapper;

        public static JsonSerializerOptions Default { get; } = new(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };

        public RabbitMqManager(string hostName, string userName, string password, IDapper dapper)
        {
            var factory = new ConnectionFactory
            {
                HostName = hostName,
                UserName = userName,
                Password = password,
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _dapper = dapper;
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

        public async Task ConsumeMessages(string queueName)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var mensaje = Encoding.UTF8.GetString(ea.Body.ToArray());

                    var deserializedMessage = JsonSerializer.Deserialize<FileDataModel>(ea.Body.Span,Default);

                    var sp = "INSERT INTO [dbo].[DocumentosRegistrados] (Nombre, FechaInsercion, FechaImpresion) VALUES (@Nombre, @FechaInsercion, @FechaImpresion)";
                    var parms = new DynamicParameters();
                    parms.Add("Nombre", deserializedMessage.Nombre, DbType.String);
                    parms.Add("FechaInsercion", deserializedMessage.FechaInsercion, DbType.DateTime);
                    parms.Add("FechaImpresion", deserializedMessage.FechaImpresion, DbType.DateTime);
                    await Task.Run(() => _dapper.Execute(sp, parms, CommandType.Text));

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

        }


        public void Close()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
