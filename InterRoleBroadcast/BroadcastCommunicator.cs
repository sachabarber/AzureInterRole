using System;


namespace InterRoleBroadcast
{
    public class BroadcastCommunicator : IDisposable
    {
        private ServiceBusClient<IBroadcastServiceChannel> _publisher;
        private ServiceBusHost<BroadcastService> _subscriber;
        private bool _disposed = false;


        public void Publish(BroadcastEvent e)
        {
            if (this.Publisher.Client != null)
            {
                this.Publisher.Client.Publish(e);
            }
        }

        public IObservable<BroadcastEvent> BroadcastEventsStream
        {
            get { return this.Subscriber.ServiceInstance.ObtainStream(); }
        }


        private ServiceBusClient<IBroadcastServiceChannel> Publisher
        {
            get
            {
                if (_publisher == null)
                {
                    _publisher = new ServiceBusClient<IBroadcastServiceChannel>();
                }

                return _publisher;
            }
        }



        private ServiceBusHost<BroadcastService> Subscriber
        {
            get
            {
                if (_subscriber == null)
                {
                    _subscriber = new ServiceBusHost<BroadcastService>();
                }

                return _subscriber;
            }
        }



        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }



        public void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                try
                {
                    _subscriber.Dispose();
                    _subscriber = null;
                }
                catch
                {
                    // Ignore exceptions
                }

                try
                {
                    _publisher.Dispose();
                    _publisher = null;
                }
                catch
                {
                    // Ignore exceptions
                }

                _disposed = true;
            }
        }



        ~BroadcastCommunicator()
        {
            Dispose(false);
        }
    }
}
