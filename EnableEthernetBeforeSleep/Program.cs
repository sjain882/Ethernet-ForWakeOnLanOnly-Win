using System.Configuration;
using System.Diagnostics;

namespace EnableEthernetBeforeSleep
{
    internal class Program
    {
        static void Main(string[] args)
        {

            EnableNetworkAdapter(ConfigurationManager.AppSettings.Get("AdapterName"));

            // Why are there no error checks or status updates?
            // Answer: beecause Power Triggers cannot pause sleep until a process has exited.
            // It seems this is not possible in Windows without heavy driver reverse engineering.
            // Thus, the objective is to complete execution as fast as possible, rather than try to pause sleep until an ethernet connection has been established.
            // This should still be fine in most cases as ethernet is quite quick to connect.
            // So - no error checks, no status updates, just do it and hope for the best :)

        }



        // Enable Disable Network Adapters: https://stackoverflow.com/a/18761206/12948636
        // Hide Window of Process: https://stackoverflow.com/a/739136/12948636
        // Hide Window of this app: Build > Output Type > Windows Application
        static void EnableNetworkAdapter(string interfaceName)
        {
            System.Diagnostics.ProcessStartInfo psi =
                    new System.Diagnostics.ProcessStartInfo("netsh", "interface set interface \"" + interfaceName + "\" enable");
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo = psi;
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.CreateNoWindow = true;
            p.Start();
        }

        static void DisableNetworkAdapter(string interfaceName)
        {
            System.Diagnostics.ProcessStartInfo psi =
                new System.Diagnostics.ProcessStartInfo("netsh", "interface set interface \"" + interfaceName + "\" disable");
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo = psi;
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.CreateNoWindow = true;
            p.Start();
        }
    }
}