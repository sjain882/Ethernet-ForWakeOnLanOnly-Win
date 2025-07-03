using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Management;
using System;
using System.Configuration;

namespace DisableEthernetAfterWakeup
{
    internal class Program
    {
        static void Main(string[] args)
        {

            string adapterName = ConfigurationManager.AppSettings.Get("AdapterName");

            if (!string.IsNullOrEmpty(adapterName))
            {

                // Check if adapter exists (this function only returns enabled network adapters - good in this case)
                NetworkInterface[] networkCards = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
                bool adapterNameExists = false;

                foreach (NetworkInterface networkCard in networkCards)
                {
                    if (networkCard.Name.Equals(adapterName))
                    {
                        adapterNameExists = true;
                    }
                }

                if (!adapterNameExists)
                {
                    MessageBox.Show("An enabled network adapter with name '" + adapterName + $"' was not found! \r\n");
                    return;
                }



                // It must exist by this point, so disable it
                // WMI is unreliable here so we will not use it (https://stackoverflow.com/a/6170507/12948636)
                DisableNetworkAdapter(adapterName);

                // Handoff back to Power Triggers
                Environment.Exit(0);
                return;

            }
            else
            {
                MessageBox.Show("Adapter name not set in config file! \r\nWoL may be unavailable!");
                Environment.Exit(0);
                return;
            }
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