using CommonEntity;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SimulatedDevice;
using System;
using System.Collections.Generic;
using System.Text;
using Thing.Interfaces;

namespace RabbitMQReceiver
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<int, IThing> devices = new Dictionary<int, IThing>();
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    ////Console.WriteLine(" [x] Received {0}", message);
                    var chiller = Newtonsoft.Json.JsonConvert.DeserializeObject<Chiller>(message);
                    Console.WriteLine(" [x] Received {0} : Temparate {1}\n", chiller.Name, chiller.AttributeRuntimeDatas["Temperature"].dqt.Value);
                    int id = Convert.ToInt32(chiller.Id);
                    devices.TryGetValue(id, out IThing value);
                    if(value == null)
                    {
                        value = ActorProxy.Create<IThing>(new ActorId(id), new Uri("fabric:/Application1/ThingActorService"));
                        devices.Add(id, value);                        
                    }
                    
                    List<AttributeRuntimeData> attributeRuntimeDatas = new List<AttributeRuntimeData>();
                    foreach (var data in chiller.AttributeRuntimeDatas.Values)
                    {
                        attributeRuntimeDatas.Add(data);
                    }
                    value.ProcessEventAsync(attributeRuntimeDatas);
                };
                channel.BasicConsume(queue: "hello",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
