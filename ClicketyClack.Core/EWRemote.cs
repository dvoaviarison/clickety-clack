// Copyright (c) 2024 DVoaviarison
using System.Text.Json;
using System.Text.Json.Serialization;
using ClicketyClack.Core.Models;
using Microsoft.Extensions.Logging;

namespace ClicketyClack.Core;

public class EWRemoteSimulator : IEWRemoteSimulator
{
    private readonly IEWClient _client;
    private readonly ILogger<EWRemoteSimulator> _logger;
    private const int HeartBeatsEveryMs = 3000;
    private Status Status { get; set; } = new();
    private readonly JsonSerializerOptions _deSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    public EWRemoteSimulator(IEWClient client, ILogger<EWRemoteSimulator> logger)
    {
        _client = client;
        _logger = logger;
    }
    
    public async Task SetupPairingAsync(CancellationToken cancellationToken)
    {
        // Connect
        await _client.ConnectAsync();
        
        // Start HeartBeat Job
        RunHeartBeats(HeartBeatsEveryMs, cancellationToken);
        
        // Start Reception Job
        RunReceiveJob(cancellationToken);
        
        // Request for pairing
        await _client.SendAsync(Messages.PairingRequest);
    }

    public async Task NextSlideAsync()
    {
        try
        {
            _logger.LogDebug("Next >");
            await _client.SendAsync(Messages.NextSlide(Status.RequestRev));
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, exception.Message);
        }
    }

    public async Task PreviousSlideAsync()
    {
        try
        {
            _logger.LogDebug("< Previous");
            await _client.SendAsync(Messages.PreviousSlide(Status.RequestRev));
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, exception.Message);
        }
    }

    public async Task TerminatePairingAsync()
    {
        _logger.LogDebug("Terminating Pairing Gracefully...");
       await _client.DisconnectAsync();
    }

    private void RunHeartBeats(int sendEveryMs, CancellationToken cancellationToken)
    {
        var heartBeatThread = new Thread(StartHeartBeatAsync);
        heartBeatThread.Start();
        return;

        async void StartHeartBeatAsync()
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await _client.SendAsync(Messages.HeartBeat);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, exception.Message);
                }
            
                Thread.Sleep(sendEveryMs);
            }
            
            _logger.LogDebug("Heartbeat stopped gracefully");
        }
    }
    
    private void RunReceiveJob(CancellationToken cancellationToken)
    {
        var heartBeatThread = new Thread(StartReceiveAsync);
        heartBeatThread.Start();
        return;

        async void StartReceiveAsync()
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var received = await _client.ReceiveAsync();
                    if (!string.IsNullOrEmpty(received))
                    {
                        _logger.LogDebug($"Received: {received}");
                        if (received.Contains("\"action\":\"status\"", StringComparison.OrdinalIgnoreCase))
                        {
                            var previousPermissions = Status.Permissions;
                            Status = JsonSerializer.Deserialize<Status>(received, _deSerializerOptions) ?? new Status();
                            if (Status.Permissions is 0)
                            {
                                _logger.LogInformation("\ud83d\udd10 Remote permission read-only. Please reach our to EW admin.");
                            }

                            if (Status.Permissions is 1 && previousPermissions is 0)
                            {
                                _logger.LogInformation("\ud83d\udd13 Remote command permission granted.");
                            }
                        }

                        if (received.Contains("\"action\":\"notPaired\"", StringComparison.OrdinalIgnoreCase))
                        {
                            _logger.LogInformation("\ud83d\udfe1 Remote connected but NOT paired. Please reach our to EW admin.");
                        }
                        
                        if (received.Contains("\"action\":\"paired\"", StringComparison.OrdinalIgnoreCase))
                        {
                            _logger.LogInformation("\ud83d\udfe2 Remote connected and paired. you can start using the app now!");
                        }
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, exception.Message);
                }

                Thread.Sleep(500);
            }
            _logger.LogDebug("Listening stopped gracefully");
        }
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}