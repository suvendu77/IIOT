using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;
using SimulatedDevice;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RabbitMQPublisher
{
    class Program
    {
        static void Main(string[] args)
        {
            int number_of_device = 10;
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                List<Chiller> Chillers = new List<Chiller>(); 

                for(int i =0; i < number_of_device; i++)
                {
                    var number = i + 1;
                    var chiller = new Chiller(number.ToString(), "Device" + number);
                    Chillers.Add(chiller);
                }


                while (true)
                {
                    for (int i = 0; i < number_of_device; i++)
                    {
                        var ch = Chillers[i];
                        ch.SimulateData();
                        string output = JsonConvert.SerializeObject(ch);
                        var body = Encoding.UTF8.GetBytes(output);

                        channel.BasicPublish(exchange: "",
                                             routingKey: "hello",
                                             basicProperties: null,
                                             body: body);
                        Console.WriteLine(" [x] Sent {0}", output);
                    }
                    Thread.Sleep(1000);
                }                
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
