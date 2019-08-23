using Actor1.Interfaces;
using CommonEntity;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using System;
using System.Threading.Tasks;
using Thing.Interfaces;
using SampleScriptActor.Interfaces;

namespace ConsoleApp3
{
    class Program
    {
        static void Main(string[] args)
        {
            IActor1 actor = ActorProxy.Create<IActor1>(ActorId.CreateRandom(), new Uri("fabric:/Application1/Actor1ActorService"));
            System.Threading.Tasks.Task<string> retval = actor.GetHelloWorldAsync();
            Console.Write(retval.Result);
            Console.ReadLine();


            IThing actor1 = ActorProxy.Create<IThing>(new ActorId(Guid.NewGuid()), new Uri("fabric:/Application1/ThingActorService"));
            Task<ThingData> model = actor1.GetData();
            ThingData data = (ThingData)model.Result;
            Console.Write(data.ThingMetaData.Name);
            Console.ReadLine();

            ISampleScriptActor script = ActorProxy.Create<ISampleScriptActor>(new ActorId(Guid.NewGuid()), new Uri("fabric:/Application1/SampleScriptActorService"));
            while (true)
            {
                Console.ReadLine();
                Console.Write(script.GetValueOfB().Result);              
            }
        }
    }
}
