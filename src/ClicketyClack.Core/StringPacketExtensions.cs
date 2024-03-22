// Copyright (c) 2024 DVoaviarison
using System;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ClicketyClack.Core;

public static class StringPacketExtensions
{
    internal const string JsonPattern = @"{[^{}]*[^{}]}"; // Excludes empty brackets;
    
    public static bool IsValidJson(this string jsonString)
    {
        if (string.IsNullOrEmpty(jsonString))
        {
            return false;
        }

        try
        {
            JsonDocument.Parse(jsonString);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    public static string? GetFirstPacketObject(this string rawPacketString)
    {
        if (string.IsNullOrEmpty(rawPacketString))
        {
            return null;
        }

        foreach (Match match in Regex.Matches(rawPacketString, JsonPattern))
        {
            var jsonString = match.Value;
            if (jsonString.IsValidJson())
            {
                return jsonString;
            }
        }

        return null;
    }

    public static bool IsStatusMessage(this string packetString) 
        => packetString.Contains("\"action\":\"status\"", StringComparison.OrdinalIgnoreCase);

    public static bool IsNotPairedMessage(this string packetString) 
        => packetString.Contains("\"action\":\"notPaired\"", StringComparison.OrdinalIgnoreCase); 

    public static bool IsPairedMessage(this string packetString) 
        => packetString.Contains("\"action\":\"paired\"", StringComparison.OrdinalIgnoreCase); 

    public static bool IsConnetionResetMessage(this string? packetExceptionMessage)
    {
        var markerString = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "An established connection was aborted"
            : RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                ? "Connection reset by peer"
                : "Transport endpoint is not connected";

        return packetExceptionMessage?.Contains(markerString, StringComparison.OrdinalIgnoreCase) is true;
    }

    public static bool IsBrokenPipeMessage(this string? packetExceptionMessage)
    {
        var markerString = "Broken pipe";

        return packetExceptionMessage?.Contains(markerString, StringComparison.OrdinalIgnoreCase) is true;
    }
}

