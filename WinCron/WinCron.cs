using System;
using System.Threading;
using System.ServiceProcess;

public class CronService : ServiceBase
{
    private CronJob job;
    private Timer stateTimer;
    private TimerCallback timerDelegate;

    public CronService()
    {
        this.ServiceName = "WinCron";
        this.CanStop = true;
        this.CanPauseAndContinue = false;
        this.AutoLog = true;
    }

    protected override void OnStart(string[] args)
    {
        // do startup stuff
        job = new CronJob();
        timerDelegate = new TimerCallback(job.ExecuteJob);
        stateTimer = new Timer(timerDelegate, null, 1000, 1000);
    }

    protected override void OnStop()
    {
        // do shutdown stuff
        stateTimer.Dispose();
    }

    public static void Main()
    {
        System.ServiceProcess.ServiceBase.Run(new CronService());
    }
}