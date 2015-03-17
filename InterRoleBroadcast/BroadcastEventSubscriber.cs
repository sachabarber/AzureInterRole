using System;
using Microsoft.WindowsAzure.ServiceRuntime;


namespace InterRoleBroadcast
{
    public class BroadcastEventSubscriber : IObserver<BroadcastEvent>
    {
        public void OnNext(BroadcastEvent value)
        {
            Logger.AddLogEntry(RoleEnvironment.CurrentRoleInstance.Id + " got message from " + value.SenderInstanceId + " : " + value.Message);
        }



        public void OnCompleted()
        {
            // Handle on completed
        }



        public void OnError(Exception error)
        {
            // Handle on error
        }
    }
}
