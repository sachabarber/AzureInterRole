using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Microsoft.ServiceBus;


namespace InterRoleBroadcast
{
    public class ServiceBusHost<T> where T : class
    {
        private readonly ServiceHost _serviceHost;
        private bool _disposed = false;


        public ServiceBusHost()
        {
            Uri address = ServiceBusEnvironment.CreateServiceUri("sb", EndpointInformation.ServiceNamespace, EndpointInformation.ServicePath);

            NetEventRelayBinding binding = new NetEventRelayBinding(EndToEndSecurityMode.None, RelayEventSubscriberAuthenticationType.None);

            TransportClientEndpointBehavior credentialsBehaviour = new TransportClientEndpointBehavior();
            credentialsBehaviour.TokenProvider =
              TokenProvider.CreateSharedAccessSignatureTokenProvider(EndpointInformation.KeyName, EndpointInformation.Key);


            //credentialsBehaviour.TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider()
            //credentialsBehaviour.TokenProvider = new TokenProvider()
            //credentialsBehaviour.CredentialType = TransportClientCredentialType.SharedSecret;
            //credentialsBehaviour.Credentials.SharedSecret.IssuerName = EndpointInformation.IssuerName;
            //credentialsBehaviour.Credentials.SharedSecret.IssuerSecret = EndpointInformation.IssuerSecret;

            ServiceEndpoint endpoint = new ServiceEndpoint(ContractDescription.GetContract(typeof(T)), binding, new EndpointAddress(address));
            endpoint.Behaviors.Add(credentialsBehaviour);

            _serviceHost = new ServiceHost(Activator.CreateInstance(typeof(T)));

            _serviceHost.Description.Endpoints.Add(endpoint);

            _serviceHost.Open();
        }



        public T ServiceInstance
        {
            get
            {
                return _serviceHost.SingletonInstance as T;
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
                        if (_serviceHost.State == CommunicationState.Opened)
                        {
                            _serviceHost.Close();
                        }
                        else
                        {
                            _serviceHost.Abort();
                        }
                    }
                    catch 
                    {
                        // Ignore exceptions
                    }
                    finally
                    {
                        _disposed = true;
                    }
                }
            }
        }



        ~ServiceBusHost()
        {
            Dispose(false);
        }
    }
}
