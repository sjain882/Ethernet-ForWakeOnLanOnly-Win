using System.Configuration;
using System.Diagnostics;

namespace DisableEthernetAfterWakeup
{
    internal class Program
    {
        static void Main(string[] args)
        {

            DisableNetworkAdapter(ConfigurationManager.AppSettings.Get("AdapterName"));

            // Why are there no error checks or status updates?
            // See reasons outlined in EnableEthernetBeforeSleep project
            // No point adding them here if I can't add them in the other half...
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