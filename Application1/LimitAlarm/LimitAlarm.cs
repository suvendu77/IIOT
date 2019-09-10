using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using LimitAlarm.Interfaces;
using CommonEntity;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;

namespace LimitAlarm
{ 

    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    public class LimitAlarm : Actor, ILimitAlarm
    {
        private const string MetadataState = "metadata";
        IConnection connection;
        IModel channel;
        /// <summary>
        /// Initializes a new instance of LimitAlarm
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public LimitAlarm(ActorService actorService, ActorId actorId) 
            : base(actorService, actorId)
        {
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override async Task OnActivateAsync()
        {
            await Task.Run(() =>
            {

                var factory = new ConnectionFactory() { HostName = "localhost" };
                connection = factory.CreateConnection();
                channel = connection.CreateModel();
                channel.QueueDeclare(queue: "Alarm",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
            });

        }

        protected override async Task OnDeactivateAsync()
        {
            await Task.Run(() =>
            {               
                connection.Close();
                connection.Dispose();
                channel.Close();
                channel.Dispose();
            });
        }

        public async Task ProcessEventAsync(string context, LimitAlarmDesc limitAlarm, double value)
        {
            await Task.Run(() =>
            {
                var alarm = new AlarmMessage(context, limitAlarm, value);
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(alarm));
                channel.BasicPublish(exchange: "",
                                                routingKey: "Alarm",
                                                basicProperties: null,
                                                body: body);
            });
        }

        public async Task SetData(LimitAlarmData data)
        {
            await StateManager.SetStateAsync(MetadataState, data);
        }

        public Task<LimitAlarmData> GetData()
        {
            throw new NotImplementedException();
        }
    }
}
