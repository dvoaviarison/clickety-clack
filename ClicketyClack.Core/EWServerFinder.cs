// Copyright (c) 2024 DVoaviarison
using ClicketyClack.Core.Models;
using Makaretu.Dns;
using Microsoft.Extensions.Logging;

namespace ClicketyClack.Core;

public class EWServerFinder : IEWServerFinder
{
    private readonly ILogger<EWServerFinder> _logger;
    private const string ServiceName = "_ezwremote._tcp.local";

    public EWServerFinder(ILogger<EWServerFinder> logger)
    {
        _logger = logger;
    }
    
    public async Task<ServerInfo> FindAsync()
    {
        var query = new Message();
        query.Questions.Add(new Question { Name = ServiceName, Type = DnsType.ANY });
        var cancellation = new CancellationTokenSource(10_000);
        
        var ipAddress = string.Empty;
        int? port = null;
        while (string.IsNullOrEmpty(ipAddress) || port is null)
        {
            try
            {
                _logger.LogInformation("\ud83d\udd59 Discovering EW Server...");
                using (var mdns = new MulticastService())
                {
                    mdns.Start();

                    var response = await mdns.ResolveAsync(query, cancellation.Token);
                    port = response.AdditionalRecords.OfType<SRVRecord>().FirstOrDefault()?.Port;
                    ipAddress = response.AdditionalRecords.OfType<ARecord>().FirstOrDefault()?.Address?.ToString();

                    mdns.Stop();
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
            }
        }
        
        _logger.LogInformation($"\ud83d\udd35 EW Server found @ {ipAddress}:{port}");
        return await Task.FromResult(
            new ServerInfo
            {
                IPAddress = ipAddress,
                Port = port.Value
            });
    }
}