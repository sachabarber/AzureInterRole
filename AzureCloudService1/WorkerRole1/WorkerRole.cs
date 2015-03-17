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

namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        private volatile BroadcastCommunicator _broadcastCommunicator;
        private volatile BroadcastEventSubscriber _broadcastEventSubscriber;
        private volatile IDisposable _broadcastSubscription;
        private volatile bool _keepLooping = true;

        public override bool OnStart()
        {
            _broadcastCommunicator = new BroadcastCommunicator();
            _broadcastEventSubscriber = new BroadcastEventSubscriber();
            _broadcastSubscription = _broadcastCommunicator.Subscribe(_broadcastEventSubscriber);


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
                    BroadcastEvent broadcastEvent = new BroadcastEvent(RoleEnvironment.CurrentRoleInstance.Id, "Hello world from  WorkerRole1");

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


        //private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        //private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        //public override void Run()
        //{
        //    Trace.TraceInformation("WorkerRole1 is running");

        //    try
        //    {
        //        this.RunAsync(this.cancellationTokenSource.Token).Wait();
        //    }
        //    finally
        //    {
        //        this.runCompleteEvent.Set();
        //    }
        //}

        //public override bool OnStart()
        //{
        //    // Set the maximum number of concurrent connections
        //    ServicePointManager.DefaultConnectionLimit = 12;

        //    // For information on handling configuration changes
        //    // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

        //    bool result = base.OnStart();

        //    Trace.TraceInformation("WorkerRole1 has been started");

        //    return result;
        //}

        //public override void OnStop()
        //{
        //    Trace.TraceInformation("WorkerRole1 is stopping");

        //    this.cancellationTokenSource.Cancel();
        //    this.runCompleteEvent.WaitOne();

        //    base.OnStop();

        //    Trace.TraceInformation("WorkerRole1 has stopped");
        //}

        //private async Task RunAsync(CancellationToken cancellationToken)
        //{
        //    // TODO: Replace the following with your own logic.
        //    while (!cancellationToken.IsCancellationRequested)
        //    {
        //        Trace.TraceInformation("Working");
        //        await Task.Delay(1000);
        //    }
        //}
    }
}
