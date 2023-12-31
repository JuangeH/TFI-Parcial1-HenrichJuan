﻿using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Reciever.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reciever.Managers
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

        public async Task ConsumeMessages(string queueName)
        {
            var consumer = new EventingBasicConsumer(_channel);
            Random random = new Random();

            consumer.Received += async (model, ea) =>
            {
                try
                {
                    if (random.Next(0, 3) == 1)
                    {
                        var mensaje = Encoding.UTF8.GetString(ea.Body.ToArray());

                        var deserializedMessage = JsonConvert.DeserializeObject<FileDataModel>(mensaje);
                        //Modifica el atributo Prioridad y FechaImpresion
                        deserializedMessage.Prioridad = 1;
                        deserializedMessage.FechaImpresion = DateTime.Now;
                        DeclareQueue("ReceiverQueue");
                        //Enviar el mensaje modificado a la cola ReceiverQueue
                        SendMessage("ReceiverQueue", deserializedMessage);
                        _channel.BasicAck(ea.DeliveryTag, false);
                    }
                    else
                    {
                        //No hace nada y se eliminan de la queue
                        _channel.BasicReject(ea.DeliveryTag, false);
                    }

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
