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
            // Answer: because Power Triggers cannot pause sleep until a process has exited.
            //
            // It seems this is not possible in Windows without heavy driver reverse engineering.
            // Thus, the objective is to complete execution as fast as possible, rather than try to pause sleep until an Ethernet connection has been established.
            // So - no error checks, no status updates, just do it and hope for the best :)
            // This should still be fine in most cases as ethernet is quite quick to connect.
            //
            // This is the same reason the output type is Windows Application (has the benefit of invisible window, but returns immediately - impossible to tell if Ethernet connection was established),
            // rather than Console Application (only reports a return code once execution completes, rather than returning immediately - good for being able to tell if Ethernet connection was established, bad for looking clunky).
            //
            // This is also the same reason that using Power Triggers v2 over Task Scheduler is beneficial,
            // because it runs suspend/resume tasks in a higher-priority thread, meaning it is more liekly to finish before sleep.
            // Source: win7suspendresume.zip\sourceCode\PowerTriggers v2\PowerTriggers\Documents\Author Notes.txt
            // Zip found in https://ia903400.us.archive.org/view_archive.php?archive=/12/items/sylirana_ms_codeplex_zips/tars/mscodeplex-w-2.tar
            // Source2: https://codeplexarchive.org/projecttab/sourceCode/win7suspendresume//PowerTriggers%20v2/PowerTriggers/Documents/Author%20Notes.txt

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