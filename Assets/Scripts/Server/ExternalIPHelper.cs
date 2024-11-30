using System.Net;
using UnityEngine;

public static class ExternalIPHelper
{
    public static string GetExternalIPAddress()
    {
        try
        {
            using (WebClient webClient = new WebClient())
            {
                string ip = webClient.DownloadString("http://checkip.dyndns.org");
                ip = ip.Substring(ip.IndexOf(":") + 1);
                ip = ip.Substring(0, ip.IndexOf("<")).Trim();
                return ip;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to get external IP: {ex.Message}");
            return "Unknown";
        }
    }
}
