using System;
using System.Collections;
using System.Configuration.Install;
using System.ServiceProcess;
using System.ComponentModel;

namespace WinCron
{
    /// <summary>
    /// Service installer that will add the service to the Windows Service Manager.
    /// </summary>
    [RunInstaller(true)]
    public class CronInstaller : Installer
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
}