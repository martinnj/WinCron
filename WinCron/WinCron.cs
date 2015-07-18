using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Configuration.Install;
using System.ServiceProcess;

namespace WinCron
{
    public class CronService : ServiceBase
    {

        CronTab crontab;
        List<Thread> threadtable;
        Thread masterthread;

        public CronService()
        {
            this.ServiceName = "WinCron";
            this.CanStop = true;
            this.CanPauseAndContinue = true;

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

            crontab = new CronTab();
            threadtable = new List<Thread>();
            masterthread = new Thread(new ThreadStart(MasterThread));
        }

        public void MasterThread()
        {
            while (true)
            {
                // Clean up threadtable, remove finished or aborted threads.
                threadtable.RemoveAll(t => !t.IsAlive);

                // Run over jobs and create threads for the jobs that need to be run.
                DateTime now = DateTime.Now;
                foreach (CronJob job in crontab.GetJobs())
                {
                    if (job.ShouldRun(now))
                    {
                        Thread t = new Thread(new ThreadStart(job.ExecuteJob));
                        threadtable.Add(t);
                        t.Start(this.EventLog);
                    }
                }

                // Sleep for one second which is the tick length.
                Thread.Sleep(1000);
            }
        }

        protected override void OnStart(string[] args)
        {
            // do startup stuff
            EventLog.WriteEntry("Starting WinCron service.");
            foreach (string line in File.ReadLines("crontab.txt"))
            {
                if (line.StartsWith("#")) { continue; }
                try
                {
                    CronJob job = new CronJob(line);
                    crontab.AddJob(job);
                }
                catch (ArgumentException ex)
                {
                    this.EventLog.WriteEntry("Unparsable line: (" + line + ") - " + ex.Message);
                    continue;
                }
            }
            masterthread.Start();
        }

        protected override void OnStop()
        {
            // do shutdown stuff
            EventLog.WriteEntry("Stopping WinCron service.");
            masterthread.Abort();
            EventLog.WriteEntry("Aborting child threads.");
            foreach (Thread t in threadtable)
            {
                t.Abort();
            }
            EventLog.WriteEntry("Child threads aborted, shutdown complete.");
        }

        protected override void OnPause()
        {
            masterthread.Suspend();
            base.OnPause();
        }

        protected override void OnContinue()
        {
            masterthread.Resume();
            base.OnContinue();
        }
    }
}