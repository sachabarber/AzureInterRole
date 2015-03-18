using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.ServiceModel;


namespace InterRoleBroadcast
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class BroadcastService : IBroadcastServiceContract
    {

        private Subject<BroadcastEvent> eventStream = new Subject<BroadcastEvent>();


        public IObservable<BroadcastEvent> ObtainStream()
        {
            return eventStream.AsObservable();
        }

        public void Publish(BroadcastEvent e)
        {
            try
            {
                eventStream.OnNext(e);
            }
            catch (Exception exception)
            {
                eventStream.OnError(exception);
            }

            //ParallelQuery<IObserver<BroadcastEvent>> subscribers = 
            //    from sub in _subscribers.AsParallel().AsUnordered() 
            //    select sub;


            //subscribers.ForAll((subscriber) =>
            //{
            //    try
            //    {
            //        subscriber.OnNext(e);
            //    }
            //    catch (Exception ex)
            //    {
            //        try
            //        {
            //            subscriber.OnError(ex);
            //        }
            //        catch (Exception)
            //        {
            //            // Handle exception
            //        }
            //    }
            //});
        }

      


        //public IDisposable Subscribe(IObserver<BroadcastEvent> subscriber)
        //{
        //    if (!_subscribers.Contains(subscriber))
        //    {
        //        _subscribers.Add(subscriber);
        //    }

        //    return new UnsubscribeCallbackHandler(_subscribers, subscriber);
        //}



        //private class UnsubscribeCallbackHandler : IDisposable
        //{
        //    private readonly IList<IObserver<BroadcastEvent>> _subscribers;
        //    private readonly IObserver<BroadcastEvent> _subscriber;

        //    public UnsubscribeCallbackHandler(IList<IObserver<BroadcastEvent>> subscribers, IObserver<BroadcastEvent> subscriber)
        //    {
        //        _subscribers = subscribers;
        //        _subscriber = subscriber;
        //    }

        //    public void Dispose()
        //    {
        //        if ((_subscribers != null) && (_subscriber != null) && (_subscribers.Contains(_subscriber)))
        //        {
        //            _subscribers.Remove(_subscriber);
        //        }
        //    }
        //}
    }
}
