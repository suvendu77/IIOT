using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using SampleScriptActor.Interfaces;
using NLua;

namespace SampleScriptActor
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
    public class SampleScriptActor : Actor, ISampleScriptActor
    {
        private IActorTimer _updateTimer;

        /// <summary>
        /// Initializes a new instance of SampleScriptActor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public SampleScriptActor(ActorService actorService, ActorId actorId) 
            : base(actorService, actorId)
        {
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            _updateTimer = RegisterTimer(
            MoveObject,                     // Callback method
            null,                           // Parameter to pass to the callback method
            TimeSpan.FromMilliseconds(500),  // Amount of time to delay before the callback is invoked
            TimeSpan.FromMilliseconds(500)); // Time interval between invocations of the callback method
            return base.OnActivateAsync();
           }

        protected override Task OnDeactivateAsync()
        {
            if (_updateTimer != null)
            {
                UnregisterTimer(_updateTimer);
            }

            return base.OnDeactivateAsync();
        }

        private Task MoveObject(object statepara)
        {
            Lua state = new Lua();           
            state.LoadCLRPackage();
            state.DoString(@" import ('SampleScriptActor', 'SampleScriptActor') 
			   import ('System.Web') ");           
            state.DoString(@"
	        local res4 = SampleObject.GetValue('b')
            res4 = res4 + 1;
            SampleObject.SetValue('b', res4);
	        ");
           
            return Task.FromResult(true);
        }

        public Task<int> GetValueOfB()
        {
            return Task.FromResult<int>(SampleObject.GetValue("b"));
        }
    }
}
