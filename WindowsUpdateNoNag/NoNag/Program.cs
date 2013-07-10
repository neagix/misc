using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.ServiceProcess;

namespace NoNag
{
    class Program
    {
        const string WAserviceName = "wuauserv";

        static void Main(string[] args)
        {
            using (RegistryKey rk = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (var waK = rk.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\WindowsUpdate\\Auto Update", true))
                {
                    if (waK.GetSubKeyNames().Contains("RebootRequired"))
                    {
                        Console.WriteLine("Deleting RebootRequired key");
                        waK.DeleteSubKeyTree("RebootRequired");

                        Console.WriteLine("Stopping {0} service...", WAserviceName);
                        ServiceController service = new ServiceController(WAserviceName);
                        service.Stop();

                        service.WaitForStatus(ServiceControllerStatus.Stopped);

                        Console.WriteLine("Starting {0} service...", WAserviceName);
                        service.Start();

                        service.WaitForStatus(ServiceControllerStatus.Running);
                    }
                    else
                        Console.WriteLine("Nothing to do");
                }
            }

        }
    }
}
