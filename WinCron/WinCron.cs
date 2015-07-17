using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Configuration.Install;
using System.ServiceProcess;

namespace WinCron
{
    public class CronService : ServiceBase
    {

        public CronService()
        {
            this.ServiceName = "WinCron";
            this.CanStop = true;
            this.CanPauseAndContinue = false;

            //Setup logging
            this.AutoLog = false;

            ((ISupportInitialize)this.EventLog).BeginInit();
            if (!EventLog.SourceExists(this.ServiceName))
            {
                EventLog.CreateEventSource(this.ServiceName, "Application");
            }
            ((ISupportInitialize)this.EventLog).EndInit();

            this.EventLog.Source = this.ServiceName;
            this.EventLog.Log = "Application";
        }

        protected override void OnStart(string[] args)
        {
            // do startup stuff
            EventLog.WriteEntry("Starting WinCron service.");
        }

        protected override void OnStop()
        {
            // do shutdown stuff
            EventLog.WriteEntry("Stopping WinCron service.");
        }
    }
}