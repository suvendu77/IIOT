using Actor1.Interfaces;
using CommonEntity;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using System;
using System.Threading.Tasks;
using Thing.Interfaces;
using SampleScriptActor.Interfaces;
using System.Collections.Generic;
using SimulatedDevice;

namespace ConsoleApp3
{
    class Program
    {
        static void Main(string[] args)
        {
            int number_of_device = 10;
            ////IActor1 actor = ActorProxy.Create<IActor1>(ActorId.CreateRandom(), new Uri("fabric:/Application1/Actor1ActorService"));
            ////System.Threading.Tasks.Task<string> retval = actor.GetHelloWorldAsync();
            ////Console.Write(retval.Result);
            ////Console.ReadLine();
            IThing[] things = new IThing[number_of_device];
            for (int i = 1; i <= number_of_device; i++)
            {
                ThingData chillerConfigData = new ThingData();
                chillerConfigData.ThingMetaData = new ThingMetaData()
                {
                    ID = i,
                    Name = "Device" + i
                };

                var attributeList = new List<AttributeData>();
                chillerConfigData.AttributeDatas = attributeList;
                AddAttribute(attributeList, "Temperature", chillerConfigData.ThingMetaData.Name + "Temperature", "Temperature", DataType.Double, "F", 0, CreateTempLimitData());
                AddAttribute(attributeList, "Humidity", chillerConfigData.ThingMetaData.Name + "Humidity", "Humidity", DataType.Double, "%", 0, CreateHumidityLimitData());
                AddAttribute(attributeList, "Pressure", chillerConfigData.ThingMetaData.Name + "Humidity", "Pressure", DataType.Double, "psi", 0, CreatePresureLimitData());
                IThing actor1 = ActorProxy.Create<IThing>(new ActorId(chillerConfigData.ThingMetaData.ID), new Uri("fabric:/Application1/ThingActorService"));
                actor1.SetData(chillerConfigData);
                things[i - 1] = actor1;
            }

            Console.Write("Deplyment is done");


            for (int i = 1; i <= number_of_device; i++)
            {
                /////IThing actor1 = ActorProxy.Create<IThing>(new ActorId(i), new Uri("fabric:/Application1/ThingActorService"));
                var actor1 = things[i - 1];
                Task<ThingData> model = actor1.GetData();
                ThingData data = (ThingData)model.Result;
                Console.Write(data.ThingMetaData.ID + ":" + data.ThingMetaData.Name + "\n");
            }

            Console.ReadLine();
            /*
            IThing device1 = ActorProxy.Create<IThing>(new ActorId(1), new Uri("fabric:/Application1/ThingActorService"));
            var chiller1 = new Chiller("1", "Device1");
            chiller1.SimulateData();
            var runtimeDatas = chiller1.AttributeRuntimeDatas.Values;
            ////Task<bool> retval1 = device1.ProcessEventAsync(runtimeDatas);
            ////Console.Write(retval1.Result);           

            List<AttributeRuntimeData> attributeRuntimeDatas = new List<AttributeRuntimeData>();
            foreach (var data in chiller1.AttributeRuntimeDatas.Values)
            {
                attributeRuntimeDatas.Add(data);
            }
            */

            ////AttributeRuntimeData attributeRuntime = new AttributeRuntimeData();
            ////attributeRuntime.Name = "Temperature";
            ////attributeRuntime.dqt = new DataQualityTimestamp()
            ////{
            ////    Value = 50,
            ////    Timestamp = DateTime.UtcNow,
            ////    Status = "Good"
            ////};
            //attributeRuntimeDatas.Add(attributeRuntime);
            ///device1.ProcessEventAsync(attributeRuntimeDatas);

            ////ISampleScriptActor script = ActorProxy.Create<ISampleScriptActor>(new ActorId(Guid.NewGuid()), new Uri("fabric:/Application1/SampleScriptActorService"));
            ////while (true)
            ////{
            ////    Console.ReadLine();
            ////    Console.Write(script.GetValueOfB().Result);
            ////}
        }

        private static void AddAttribute(List<AttributeData> attributeList, 
            string name, string context, string description, DataType type, string unit,object initialValue,
            List<LimitAlarmDesc> limits
            )
            {
            var attribute = new AttributeData()
            {
                AttributeMetaData = new AttributeMetaData()
                {
                    Name = name,
                    Context = context,
                    Description = description,
                    DataType = type,
                    EngUnit = unit,
                    InitialValue = initialValue.ToString(),
                }
            };
            attributeList.Add(attribute);
            AddLimitData(attribute, limits);
            }

        private static void AddLimitData(AttributeData attribute, List<LimitAlarmDesc> limits)
        {
            LimitAlarmData limitAlarmData = new LimitAlarmData();
            foreach(var limit in limits)
            {
                limitAlarmData.AddLimitAlarm(limit);
            }
            attribute.AlamLimitData = limitAlarmData;
            attribute.AttributeMetaData.IsLimitAlarms = true;
        }

        private static List<LimitAlarmDesc> CreateHumidityLimitData()
        {
            List<LimitAlarmDesc> limitAlarms = new List<LimitAlarmDesc>(4);
            //5,10,90,95
            LimitAlarmDesc lolo = new LimitAlarmDesc()
            {
                AlarmCategoty = AlarmCategoty.Lolo,
                Value = 5,
                Message = "Pressure lolo alarm ",
                Piority = 100,
            };
            limitAlarms.Add(lolo);

            LimitAlarmDesc lo = new LimitAlarmDesc()
            {
                AlarmCategoty = AlarmCategoty.Lo,
                Value = 10,
                Message = "Pressure lo alarm ",
                Piority = 200,
            };
            limitAlarms.Add(lo);

            LimitAlarmDesc hi = new LimitAlarmDesc()
            {
                AlarmCategoty = AlarmCategoty.Hi,
                Value = 90,
                Message = "Pressure hi alarm ",
                Piority = 200,
            };
            limitAlarms.Add(hi);

            LimitAlarmDesc hihi = new LimitAlarmDesc()
            {
                AlarmCategoty = AlarmCategoty.HiHi,
                Value = 95,
                Message = "Pressure hihi alarm ",
                Piority = 100,
            };
            limitAlarms.Add(hihi);

            return limitAlarms;
        }

        private static List<LimitAlarmDesc> CreateTempLimitData()
        {
            List<LimitAlarmDesc> limitAlarms = new List<LimitAlarmDesc>(4);
            //5,10,90,95
            LimitAlarmDesc lolo = new LimitAlarmDesc()
            {
                AlarmCategoty = AlarmCategoty.Lolo,
                Value = 5,
                Message = "temp lolo alarm ",
                Piority = 100,
            };
            limitAlarms.Add(lolo);

            LimitAlarmDesc lo = new LimitAlarmDesc()
            {
                AlarmCategoty = AlarmCategoty.Lo,
                Value = 10,
                Message = "temp lo alarm ",
                Piority = 200,
            };
            limitAlarms.Add(lo);

            LimitAlarmDesc hi = new LimitAlarmDesc()
            {
                AlarmCategoty = AlarmCategoty.Hi,
                Value = 80,
                Message = "temp hi alarm ",
                Piority = 200,
            };
            limitAlarms.Add(hi);

            LimitAlarmDesc hihi = new LimitAlarmDesc()
            {
                AlarmCategoty = AlarmCategoty.HiHi,
                Value = 85,
                Message = "temp hihi alarm ",
                Piority = 100,
            };
            limitAlarms.Add(hihi);

            return limitAlarms;
        }

        private static List<LimitAlarmDesc> CreatePresureLimitData()
        {
            List<LimitAlarmDesc> limitAlarms = new List<LimitAlarmDesc>(4);
            //5,10,90,95
            LimitAlarmDesc lolo = new LimitAlarmDesc()
            {
                AlarmCategoty = AlarmCategoty.Lolo,
                Value = 110,
                Message = "Humidity lolo alarm ",
                Piority = 100,
            };
            limitAlarms.Add(lolo);

            LimitAlarmDesc lo = new LimitAlarmDesc()
            {
                AlarmCategoty = AlarmCategoty.Lo,
                Value = 120,
                Message = "Humidity lo alarm ",
                Piority = 200,
            };
            limitAlarms.Add(lo);

            LimitAlarmDesc hi = new LimitAlarmDesc()
            {
                AlarmCategoty = AlarmCategoty.Hi,
                Value = 260,
                Message = "Humidity hi alarm ",
                Piority = 200,
            };
            limitAlarms.Add(hi);

            LimitAlarmDesc hihi = new LimitAlarmDesc()
            {
                AlarmCategoty = AlarmCategoty.HiHi,
                Value = 280,
                Message = "Humidity hihi alarm ",
                Piority = 100,
            };
            limitAlarms.Add(hihi);

            return limitAlarms;
        }
    }
}
