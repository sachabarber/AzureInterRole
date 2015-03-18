using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Microsoft.ServiceBus;


namespace InterRoleBroadcast
{
    public class ServiceBusHost<T> where T : class
    {
        private ServiceHost _serviceHost;
        private bool _disposed = false;


        public ServiceBusHost()
        {
            CreateHost();
        }


        private void CreateHost()
        {
            Uri address = ServiceBusEnvironment.CreateServiceUri("sb", EndpointInformation.ServiceNamespace, EndpointInformation.ServicePath);

            NetTcpRelayBinding binding = new NetTcpRelayBinding(EndToEndSecurityMode.None, RelayClientAuthenticationType.None);

            TransportClientEndpointBehavior credentialsBehaviour = new TransportClientEndpointBehavior();
            credentialsBehaviour.TokenProvider =
              TokenProvider.CreateSharedAccessSignatureTokenProvider(EndpointInformation.KeyName, EndpointInformation.Key);
            ServiceEndpoint endpoint = new ServiceEndpoint(ContractDescription.GetContract(typeof(T)), binding, new EndpointAddress(address));
            endpoint.Behaviors.Add(credentialsBehaviour);

            _serviceHost = new ServiceHost(Activator.CreateInstance(typeof(T)));
            _serviceHost.Faulted += ServiceHost_Faulted;

            _serviceHost.Description.Endpoints.Add(endpoint);

            _serviceHost.Open();
        }

        void ServiceHost_Faulted(object sender, EventArgs e)
        {
            ServiceHost host = (ServiceHost)sender;
            host.Faulted -= ServiceHost_Faulted;
            KillHost(host);
            CreateHost();
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

        private void KillHost(ServiceHost theHost)
        {
            if (theHost.State == CommunicationState.Opened)
            {
                theHost.Close();
            }
            else
            {
                theHost.Abort();
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
                        KillHost(_serviceHost);
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
