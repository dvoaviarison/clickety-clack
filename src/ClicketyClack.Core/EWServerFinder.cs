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
        
        var ipAddress = string.Empty;
        int? port = null;
        while (string.IsNullOrEmpty(ipAddress) || port is null)
        {
            var cancellation = new CancellationTokenSource(5_000);
            using (var mdns = new MulticastService())
            {
                try
                {
                    _logger.LogInformation("\ud83d\udd59 Discovering EW Server...");
                    mdns.Start();

                    var response = await mdns.ResolveAsync(query, cancellation.Token);
                    port = response.AdditionalRecords.OfType<SRVRecord>().FirstOrDefault()?.Port;
                    ipAddress = response.AdditionalRecords.OfType<ARecord>().FirstOrDefault()?.Address?.ToString();
                }
                catch (Exception exception)
                {
                    _logger.LogError($"\u2620\ufe0f EW Server Discovery failed with exception: {exception.Message}");
                    _logger.LogDebug(exception.StackTrace);
                }
                finally
                {
                    _logger.LogInformation("\ud83d\udd59 No EW server found, trying again");
                    mdns.Stop();
                }
            }
            
            Thread.Sleep(3000);
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