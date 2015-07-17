using System;
using System.Collections;
using System.Configuration.Install;
using System.ServiceProcess;
using System.ComponentModel;

[RunInstaller(true)]
public class CronInstaller : ServiceInstaller
{
    private ServiceProcessInstaller processInstaller;
    private ServiceInstaller serviceInstaller;

    public CronInstaller()
    {
        processInstaller = new ServiceProcessInstaller();
        serviceInstaller = new ServiceInstaller();

        processInstaller.Account = ServiceAccount.LocalSystem;
        serviceInstaller.StartType = ServiceStartMode.Manual; // TODO: Should be set to automatic later.
        serviceInstaller.ServiceName = "WinCron";

        Installers.Add(serviceInstaller);
        Installers.Add(processInstaller);
    }
}