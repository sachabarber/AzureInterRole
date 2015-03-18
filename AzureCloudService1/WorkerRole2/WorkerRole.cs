using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using InterRoleBroadcast;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;

using System.Reactive.Linq;

namespace WorkerRole2
{
    public class WorkerRole : RoleEntryPoint
    {
        private volatile BroadcastCommunicator _broadcastCommunicator;
        //private volatile BroadcastEventSubscriber _broadcastEventSubscriber;
        private volatile IDisposable _broadcastSubscription;
        private volatile bool _keepLooping = true;

        public override bool OnStart()
        {
            _broadcastCommunicator = new BroadcastCommunicator();

            //worker2
            _broadcastSubscription = _broadcastCommunicator.BroadcastEventsStream
                .Where(x => x.SenderInstanceId != RoleEnvironment.CurrentRoleInstance.Id)
                .Subscribe(
                theEvent =>
                {
                    Logger.AddLogEntry(
                        String.Format("{0} got message from {1} {2}",
                            RoleEnvironment.CurrentRoleInstance.Id,
                            theEvent.SenderInstanceId,
                            theEvent.Message));
                },
                ex =>
                {
                    Logger.AddLogEntry(ex);
                });


            return base.OnStart();
        }



        public override void Run()
        {
            // Just keep sending messasges
            while (_keepLooping)
            {
                //int secs = ((new Random()).Next(30) + 60);
                int secs = 2;

                Thread.Sleep(secs * 1000);
                try
                {
                    BroadcastEvent broadcastEvent =
                        new BroadcastEvent(RoleEnvironment.CurrentRoleInstance.Id,
                            "Hello world from  WorkerRole2");
                    _broadcastCommunicator.Publish(broadcastEvent);
                }
                catch (Exception ex)
                {
                    Logger.AddLogEntry(ex);
                }
            }
        }



        public override void OnStop()
        {
            _keepLooping = false;

            if (_broadcastCommunicator != null)
            {
                _broadcastCommunicator.Dispose();
            }

            if (_broadcastSubscription != null)
            {
                _broadcastSubscription.Dispose();
            }

            base.OnStop();
        }



    }
}
