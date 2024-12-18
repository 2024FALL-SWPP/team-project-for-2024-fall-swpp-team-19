using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
public static class ExternalIPHelper
{
    // public static string GetExternalIPAddress()
    // {
    //     try
    //     {
    //         using (WebClient webClient = new WebClient())
    //         {
    //             string ip = webClient.DownloadString("http://checkip.dyndns.org");
    //             ip = ip.Substring(ip.IndexOf(":") + 1);
    //             ip = ip.Substring(0, ip.IndexOf("<")).Trim();
    //             return ip;
    //         }
    //     }
    //     catch (System.Exception ex)
    //     {
    //         Debug.LogError($"Failed to get external IP: {ex.Message}");
    //         return "Unknown";
    //     }
    // }


    public static string GetExternalIPAddress()
    {
        try
        {
            Debug.WriteLine("Entering GetInternalIPAddress method.");

            string hostName = Dns.GetHostName();
            Debug.WriteLine($"Host name: {hostName}"); 

            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
            Debug.WriteLine($"Found {hostEntry.AddressList.Length} addresses for host.");

            foreach (IPAddress ipAddress in hostEntry.AddressList)
            {
                Debug.WriteLine($"Inspecting IP address: {ipAddress}");

                if (ipAddress.AddressFamily == AddressFamily.InterNetwork &&
                    !IPAddress.IsLoopback(ipAddress))
                {
                    Debug.WriteLine($"Selected IPv4 address: {ipAddress}");
                    return ipAddress.ToString();
                }
            }

            Debug.WriteLine("No suitable IPv4 address found.");
            return "No IPv4 address found";
        }
        catch (Exception ex) 
        {
            Debug.WriteLine($"Exception occurred: {ex.Message}");
            return "Unknown";
        }
    }
    public static string GetInternalIPAddress()
    {
        try
        {
            Debug.WriteLine("Entering GetInternalIPAddress method.");

            string hostName = Dns.GetHostName();
            Debug.WriteLine($"Host name: {hostName}");

            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
            Debug.WriteLine($"Found {hostEntry.AddressList.Length} addresses for host.");

            foreach (IPAddress ipAddress in hostEntry.AddressList)
            {
                Debug.WriteLine($"Inspecting IP address: {ipAddress}");

                if (ipAddress.AddressFamily == AddressFamily.InterNetwork &&
                    !IPAddress.IsLoopback(ipAddress))
                {
                    Debug.WriteLine($"Selected IPv4 address: {ipAddress}");
                    return ipAddress.ToString();
                }
            }

            Debug.WriteLine("No suitable IPv4 address found.");
            return "No IPv4 address found";
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception occurred: {ex.Message}");
            return "Unknown";
        }
    }
}
