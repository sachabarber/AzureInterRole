using System.Runtime.Serialization;


namespace InterRoleBroadcast
{
    [DataContract(Namespace = BroadcastNamespaces.DataContract)]
    public class BroadcastEvent
    {
        public BroadcastEvent(string senderInstanceId, string message)
        {
            this.SenderInstanceId = senderInstanceId;
            this.Message = message;
        } 


        [DataMember]
        public string SenderInstanceId { get; private set; }


        [DataMember]
        public string Message { get; private set; } 
    }
}
