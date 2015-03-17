using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Microsoft.ServiceBus;


namespace InterRoleBroadcast
{
    public class ServiceBusClient<T> where T : class, IClientChannel, IDisposable
    {
        private readonly ChannelFactory<T> _channelFactory;
        private readonly T _channel;
        private bool _disposed = false;


        public ServiceBusClient()
        {



            Uri address = ServiceBusEnvironment.CreateServiceUri("sb", EndpointInformation.ServiceNamespace, EndpointInformation.ServicePath);

            NetEventRelayBinding binding = new NetEventRelayBinding(EndToEndSecurityMode.None, RelayEventSubscriberAuthenticationType.None);

            TransportClientEndpointBehavior credentialsBehaviour = new TransportClientEndpointBehavior();
            credentialsBehaviour.TokenProvider =
              TokenProvider.CreateSharedAccessSignatureTokenProvider(EndpointInformation.KeyName, EndpointInformation.Key);

            //TransportClientEndpointBehavior credentialsBehaviour = new TransportClientEndpointBehavior();
            //credentialsBehaviour.CredentialType = TransportClientCredentialType.SharedSecret;
            //credentialsBehaviour.Credentials.SharedSecret.IssuerName = EndpointInformation.IssuerName;
            //credentialsBehaviour.Credentials.SharedSecret.IssuerSecret = EndpointInformation.IssuerSecret;

            ServiceEndpoint endpoint = new ServiceEndpoint(ContractDescription.GetContract(typeof(T)), binding, new EndpointAddress(address));
            endpoint.Behaviors.Add(credentialsBehaviour);

            _channelFactory = new ChannelFactory<T>(endpoint);

            _channel = _channelFactory.CreateChannel();
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



        public void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    try
                    {
                        if (_channel.State == CommunicationState.Opened)
                        {
                            _channel.Close();
                        }
                        else
                        {
                            _channel.Abort();
                        }
                    }
                    catch 
                    {
                        // Ignore exceptions
                    }


                    try
                    {
                        if (_channelFactory.State == CommunicationState.Opened)
                        {
                            _channelFactory.Close();
                        }
                        else
                        {
                            _channelFactory.Abort();
                        }
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
