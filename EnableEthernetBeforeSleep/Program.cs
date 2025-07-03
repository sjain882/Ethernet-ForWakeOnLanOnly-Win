using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Management;
using System.Configuration;

namespace EnableEthernetBeforeSleep
{
    internal class Program
    {
        static void Main(string[] args)
        {

            EnableNetworkAdapter(ConfigurationManager.AppSettings.Get("AdapterName"));


            // DO NOT DO BELOW: It's commented out because Power Triggers cannot pause sleep until a process has exited.
            // It seems this is not possible in Windows without heavy driver reverse engineering.
            // Thus, the objective is to complete execution as fast as possible, rather than try to pause sleep until an ethernet connection has been established.
            // This should still be fine in most cases as ethernet is quite quick to connect.
            // So - no error checks, no status updates, just do it and hope for the best :)
            /*
            
            // string adapterName = ConfigurationManager.AppSettings.Get("AdapterName");

            if (!string.IsNullOrEmpty(adapterName))
            {

                // Check if adapter exists
                // https://stackoverflow.com/a/6170507/12948636
                // Using this because GetAllNetworkInterfaces() does not return disabled adapters

                NetworkInterface[] networkCards = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
                bool adapterNameExists = false;

                SelectQuery wmiQuery = new SelectQuery("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionId != NULL");
                ManagementObjectSearcher searchProcedure = new ManagementObjectSearcher(wmiQuery);
                foreach (ManagementObject item in searchProcedure.Get())
                {
                    if (((string)item["NetConnectionId"]) == adapterName)
                    {
                        // MessageBox.Show("Adapter found: " + item["Name"] + " - " + item["Description"]);
                        adapterNameExists = true;
                    }
                }

                if (!adapterNameExists)
                {
                    MessageBox.Show("Adapter name not found! WoL may be unavailable!");
                    Environment.Exit(0);
                    return;
                }



                // It must exist by this point, so enable it
                // WMI is unreliable here so we will not use it (https://stackoverflow.com/a/6170507/12948636)
                EnableNetworkAdapter(adapterName);


                // Alternative to below: Just wait 5 seconds...
                // System.Threading.Thread.Sleep(5000);
                // 
                // Check if adapter is connected
                // Alternative would be to check NetConnectionStatus = 2 in Win32_NetworkAdapter
                // https://learn.microsoft.com/en-gb/windows/win32/cimwin32prov/win32-networkadapter?redirectedfrom=MSDN/
                // Note: GetAllNetworkInterfaces() below only returns enabled adapters (good in this case)

                bool isAdapterConnected = false;
                Stopwatch timer = new Stopwatch();
                timer.Start();

                while (!isAdapterConnected && timer.ElapsedMilliseconds < 20000)
                {
                    //MessageBox.Show(isAdapterConnected.ToString() + " - " + timer.ElapsedMilliseconds.ToString() + "ms");
                    wmiQuery = new SelectQuery("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionId != NULL");
                    searchProcedure = new ManagementObjectSearcher(wmiQuery);
                    foreach (ManagementObject item in searchProcedure.Get())
                    {
                        if (((string)item["NetConnectionId"]) == adapterName)
                        {
                            if (((UInt16)item["NetConnectionStatus"]) == (UInt16)2)
                            {
                                isAdapterConnected = true;
                                // It must be connected by this point, so handoff back to Power Triggers so it can sleep the PC
                                //MessageBox.Show("Adapter is connected! \r\nYou can now sleep the PC safely!");
                                timer.Stop();
                                Environment.Exit(0);
                                return;
                            }
                        }
                    }
                }

                // 20 seconds have elapsed, so check if its connected
                if (!isAdapterConnected)
                {
                    MessageBox.Show($"Adapter failed to connect after 20 seconds! \r\nWoL may be unavailable!");
                    timer.Stop();
                    Environment.Exit(0);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Adapter name not set in config file! \r\nWoL may be unavailable!");
                Environment.Exit(0);
                return;
            }
            
         */

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




        /*

                while (!isAdapterConnected && timer.Elapsed.TotalSeconds < 20)
                {
                    NetworkInterface[] currentNetworkCards = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();

                    NetworkInterface[] matchedInterfaces = networkCards.Where(x => x.Name.Equals(adapterName)).ToArray();

                    MessageBox.Show(matchedInterfaces.Length.ToString());


                    if (matchedInterfaces.Length > 0)
                    {
                        MessageBox.Show("Adapter found: " + matchedInterfaces[0].Name + " - " + matchedInterfaces[0].Description + " - " + matchedInterfaces[0].OperationalStatus.ToString());
                        if ((int) matchedInterfaces[0].OperationalStatus == 1) isAdapterConnected = true;
                    }
                }


        */


    }
}