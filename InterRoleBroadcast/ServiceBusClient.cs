using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Microsoft.ServiceBus;


namespace InterRoleBroadcast
{
    public class ServiceBusClient<T> where T : class, IClientChannel, IDisposable
    {
        private ChannelFactory<T> _channelFactory;
        private T _channel;
        private bool _disposed = false;


        public ServiceBusClient()
        {

            CreateChannel();

                   
        }

        public void CreateChannel()
        {
            Uri address = ServiceBusEnvironment.CreateServiceUri("sb", EndpointInformation.ServiceNamespace, EndpointInformation.ServicePath);

            NetTcpRelayBinding binding = new NetTcpRelayBinding(EndToEndSecurityMode.None, RelayClientAuthenticationType.None);

            TransportClientEndpointBehavior credentialsBehaviour = new TransportClientEndpointBehavior();
            credentialsBehaviour.TokenProvider =
              TokenProvider.CreateSharedAccessSignatureTokenProvider(EndpointInformation.KeyName, EndpointInformation.Key);
            ServiceEndpoint endpoint = new ServiceEndpoint(ContractDescription.GetContract(typeof(T)), binding, new EndpointAddress(address));
            endpoint.Behaviors.Add(credentialsBehaviour);

            _channelFactory = new ChannelFactory<T>(endpoint);

            _channel = _channelFactory.CreateChannel();
            _channel.Faulted += Channel_Faulted;

        }

        void Channel_Faulted(object sender, EventArgs e)
        {
            ICommunicationObject theChannel = (ICommunicationObject) sender;
            theChannel.Faulted -= Channel_Faulted;
            KillChannel(theChannel);
            KillChannelFactory(_channelFactory);
            CreateChannel();
        }


        public T Client
        {
            get
            {
                if (_channel.State == CommunicationState.Opening)
                {
                    return null;
                }

                if (_channel.State != CommunicationState.Opened)
                {
                    _channel.Open();
                }

                return _channel;
            }
        }



        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        private void KillChannel(ICommunicationObject theChannel)
        {
            if (theChannel.State == CommunicationState.Opened)
            {
                theChannel.Close();
            }
            else
            {
                theChannel.Abort();
            }
        }


        private void KillChannelFactory<T>(ChannelFactory<T> theChannelFactory)
        {
            if (theChannelFactory.State == CommunicationState.Opened)
            {
                theChannelFactory.Close();
            }
            else
            {
                theChannelFactory.Abort();
            }
        }


        public void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    try
                    {
                        KillChannel(_channel);
                    }
                    catch 
                    {
                        // Ignore exceptions
                    }


                    try
                    {
                        KillChannelFactory(_channelFactory);
                    }
                    catch
                    {
                        // Ignore  exceptions
                    }

                    _disposed = true;
                }
            }
        }



        ~ServiceBusClient()
        {
            Dispose(false);
        }
    }
}
