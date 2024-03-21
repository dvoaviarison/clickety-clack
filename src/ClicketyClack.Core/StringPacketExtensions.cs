// Copyright (c) 2024 DVoaviarison
using System;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ClicketyClack.Core;

public static class StringPacketExtensions
{
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

        var jsonPattern = @"{[^{}]*[^{}]}"; // Excludes empty brackets
        foreach (Match match in Regex.Matches(rawPacketString, jsonPattern))
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
}

