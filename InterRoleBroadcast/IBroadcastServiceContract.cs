using System;
using System.ServiceModel;


namespace InterRoleBroadcast
{
    [ServiceContract(Name = "BroadcastServiceContract", Namespace = BroadcastNamespaces.ServiceContract)]

    public interface IBroadcastServiceContract 
    {
        [OperationContract(IsOneWay = true)]
        void Publish(BroadcastEvent e);

        [OperationContract]
        IObservable<BroadcastEvent> ObtainStream();

    }
}