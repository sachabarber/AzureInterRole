using System.ServiceModel;


namespace InterRoleBroadcast
{
    public interface IBroadcastServiceChannel : IBroadcastServiceContract, IClientChannel
    {
    }
}
