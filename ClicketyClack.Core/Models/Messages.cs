// Copyright (c) 2024 DVoaviarison
using System.Net.NetworkInformation;

namespace ClicketyClack.Core.Models;

public class Messages
{
    private static readonly string DeviceName = Environment.MachineName;
    public static readonly string PairingRequest = @$"{{""device_type"":2,""action"":""connect"",""uid"":""{GetMacAddress()}"",""device_name"":""{DeviceName}""}}{Environment.NewLine}";
    public static readonly string HeartBeat = @$"{{""action"":""heartbeat"",""requestrev"":0}}{Environment.NewLine}";
    public static string NextSlide(int currentRequestRev) => @$"{{""action"":""nextBuild"",""requestrev"":{currentRequestRev + 1}}}{Environment.NewLine}";
    public static string PreviousSlide(int currentRequestRev) => @$"{{""action"":""prevBuild"",""requestrev"":{currentRequestRev + 1}}}{Environment.NewLine}";
    
    private static string GetMacAddress()
    {
        var macAddresses = "";
        foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (nic.NetworkInterfaceType != NetworkInterfaceType.Ethernet) continue;
            if (nic.OperationalStatus != OperationalStatus.Up) continue;
            macAddresses += nic.GetPhysicalAddress().ToString();
            break;
        }
        return macAddresses;
    }
}

