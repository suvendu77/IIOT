using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using Attribute.Interfaces;
using CommonEntity;
using LimitAlarm.Interfaces;
using IOExtention.Interfaces;
using RabbitMQ.Client;
using StoreHotData.Interfaces;

namespace Attribute
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
    internal class Attribute : Actor, IAttribute
    {
        private const string MetadataState = "metadata";

        private const string AlarmState = "alarmdata";

        private ILimitAlarm limitAlarmActor = null;
        private IIOExtention ioExtentionActor = null;
        private IStoreHotData storeHotDataActor = null;
        private int numberOfStoreHotDataActor = 25;
        private int numberOflimitAlarmActor = 5;
        private string HotDataStorePoxyID = string.Empty;
        private LimitAlarmData limitAlarmData = null;
        private AttributeMetaData metadata = null;
        private string context;
        /// <summary>
        /// Initializes a new instance of Attribute
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public Attribute(ActorService actorService, ActorId actorId) 
            : base(actorService, actorId)
        {
           
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            var result = await StateManager.TryGetStateAsync<AttributeMetaData>(MetadataState);
            if (!result.HasValue)
            {
                var metadata = new AttributeMetaData();
                await StateManager.TryAddStateAsync(MetadataState, metadata);
            }
            else
            {
                metadata = result.Value;
                this.context = metadata.Context;
                if (metadata.IsLimitAlarms)
                {
                    var result1 = await StateManager.TryGetStateAsync<LimitAlarmData>(AlarmState);
                    if (result1.HasValue)
                    {
                        this.limitAlarmData = result1.Value;
                    }
                }
            }          

        }

        public async Task<AttributeData> GetData()
        {
            AttributeData metadata = new AttributeData();
            var metadataResult = await StateManager.TryGetStateAsync<AttributeMetaData>(MetadataState);
            if (metadataResult.HasValue)
            {
                metadata.AttributeMetaData = metadataResult.Value;
            }
            else
            {
                metadata.AlamLimitData = null;
                metadata.AttributeMetaData = new AttributeMetaData()
                {
                    Name = "attribute001",

                    Description = "attribute001",

                    DataType = DataType.Integer,

                    InitialValue = "0",

                    EngUnit = ""

                };
            }
            return metadata;
        }

        public async Task ProcessEventAsync(DataQualityTimestamp dtq)
        {
            ActorEventSource.Current.ActorMessage(this, dtq.Value.ToString());
            ////processing alarm
            if (metadata.IsLimitAlarms)
            {
                await ProcessLimitAlarms(dtq);
            }
            //// storing last value
            await StoreHotData(dtq);
        }
        private async Task StoreHotData(DataQualityTimestamp dtq)
        {
            string nameSpace = this.metadata.NameSpace;
            if(string.IsNullOrEmpty(HotDataStorePoxyID))
            {
                HotDataStorePoxyID = HashFunction(nameSpace, numberOfStoreHotDataActor).ToString();
            }
            IStoreHotData dataConnection = GetStroreHotDataActorPoxy(HotDataStorePoxyID);
            await dataConnection.StoreData(nameSpace, dtq);
        }

        static int HashFunction(string s, int number)
        {
            int total = 0;
            char[] c;
            c = s.ToCharArray();

            // Summing up all the ASCII values  
            // of each alphabet in the string 
            for (int k = 0; k <= c.GetUpperBound(0); k++)
                total += (int)c[k];

            return total % number;
        }

        private async Task ProcessLimitAlarms(DataQualityTimestamp dtq)
        {
            double value = dtq.Value;
            bool isAlarm = false;
            LimitAlarmDesc limitAlarmDesc = null;
            foreach (var limit in this.limitAlarmData.LimitAlarms)
            {
                switch(limit.Key)
                {
                    case AlarmCategoty.Lo:
                        {
                            if(value <= limit.Value.Value)
                            {
                                limitAlarmDesc = limit.Value;
                                isAlarm = true;
                            }
                        }
                        break;
                    case AlarmCategoty.Lolo:
                        {
                            if (value <= limit.Value.Value)
                            {
                            limitAlarmDesc = limit.Value;
                            isAlarm = true;
                            }
                        }
                        break;
                    case AlarmCategoty.Hi:
                        {
                            if (value >= limit.Value.Value)
                            {
                            limitAlarmDesc = limit.Value;
                            isAlarm = true;
                            }
                        }
                        break;
                    case AlarmCategoty.HiHi:
                        {
                            if (value >= limit.Value.Value)
                            {
                            limitAlarmDesc = limit.Value;
                            isAlarm = true;
                            }
                        }
                        break;
                }
            }

            if(isAlarm)
            {
                await SendAlarm(limitAlarmDesc, dtq);                   
            }           
        }

        private async Task SendAlarm(LimitAlarmDesc limitAlarmDesc, DataQualityTimestamp dtq)
        {
            if (limitAlarmDesc == null)
            {
                return;
            }
            ILimitAlarm alarmProcessor = GetlimitAlarmActorPoxy("AlarmProcessor");
            await alarmProcessor.ProcessEventAsync(this.context, limitAlarmDesc, dtq);
        }

        public async Task SetData(AttributeData data)
        {
            await StateManager.SetStateAsync(MetadataState, data.AttributeMetaData);
            metadata = data.AttributeMetaData;
            if (data.AttributeMetaData.IsLimitAlarms)
            {
                ////var limitAlarm = GetlimitAlarmActorPoxy(this.Id.ToString() + "_LimitAlarm");
                ////await limitAlarm.SetData(data.AlamLimitData);
               await StateManager.SetStateAsync(AlarmState, data.AlamLimitData);
               this.limitAlarmData = data.AlamLimitData;
            }

            if (data.AttributeMetaData.IsIO)
            {
                var ioExtention = GetIOExtentionActorPoxy(this.Id.ToString() + "_IO");
                await ioExtention.SetData(data.IOExtentionData);
            }
        }

        
        private ILimitAlarm GetlimitAlarmActorPoxy(string actorID)
        {
            if(limitAlarmActor ==null)
            {
                limitAlarmActor = ActorProxy.Create<ILimitAlarm>(new ActorId(actorID), new Uri("fabric:/Application1/LimitAlarmActorService"));
            }

            return limitAlarmActor;
        }
        

        private IIOExtention GetIOExtentionActorPoxy(string actorID)
        {
            if (ioExtentionActor == null)
            {
                ioExtentionActor = ActorProxy.Create<IIOExtention>(new ActorId(actorID), new Uri("fabric:/Application1/IOExtentionActorService"));
            }

            return ioExtentionActor;
        }

        private IStoreHotData GetStroreHotDataActorPoxy(string actorID)
        {
            if (storeHotDataActor == null)
            {
                storeHotDataActor = ActorProxy.Create<IStoreHotData>(new ActorId(actorID), new Uri("fabric:/Application1/StoreHotDataActorService"));
            }

            return storeHotDataActor;
        }

    }
}
