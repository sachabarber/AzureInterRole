using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.ServiceModel;


namespace InterRoleBroadcast
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, 
        ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class BroadcastService : IBroadcastServiceContract
    {
        private object syncLock = new object();
        private Subject<BroadcastEvent> eventStream = 
            new Subject<BroadcastEvent>();


        public IObservable<BroadcastEvent> ObtainStream()
        {
            return eventStream.AsObservable();
        }

        public void Publish(BroadcastEvent e)
        {
            lock (syncLock)
            {
                try
                {
                    eventStream.OnNext(e);
                }
                catch (Exception exception)
                {
                    eventStream.OnError(exception);
                }
            }
        }
    }
}
