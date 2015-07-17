using System;
using System.Reflection;
using System.Configuration.Install;
using System.ServiceProcess;

using System.Windows.Forms;

namespace WinCron
{
    class CommandLine
    {
        public static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                string parameter = string.Concat(args);
                Console.Out.WriteLine(parameter);
                switch (parameter)
                {
                    case "--install":
                        try
                        {
                            ManagedInstallerClass.InstallHelper(new[] { "/LogToConsole=true", Assembly.GetExecutingAssembly().Location });
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message);
                        }
                        break;
                    case "--uninstall":
                        ManagedInstallerClass.InstallHelper(new[] { "/u", Assembly.GetExecutingAssembly().Location });
                        break;
                }
            }
            else
            {
                ServiceBase[] servicesToRun = new ServiceBase[] 
                          { 
                              new CronService() 
                          };
                ServiceBase.Run(servicesToRun);
            }
        }
    }
}
