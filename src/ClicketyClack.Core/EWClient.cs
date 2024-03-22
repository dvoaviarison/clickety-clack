// Copyright (c) 2024 DVoaviarison
using System.Net;
using System.Net.Sockets;
using System.Text;
using ClicketyClack.Core.Models;

namespace ClicketyClack.Core;

public class EWClient : IEWClient
{
    private readonly Socket _client;

    public EWClient()
    {
        _client = new Socket(
            AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp);
    }
    
    public async Task ConnectAsync(ServerInfo serverInfo)
    {
        await _client.ConnectAsync(new IPEndPoint(
            IPAddress.Parse(serverInfo.IPAddress), 
            serverInfo.Port));
    }

    public async Task DisconnectAsync()
    {
        await Task.CompletedTask;
        _client.Shutdown(SocketShutdown.Both);
        await _client.DisconnectAsync(true);
    }

    public async Task SendAsync(string message)
    {
        var messageBytes = Encoding.ASCII.GetBytes(message);
        await _client.SendAsync(messageBytes);
    }

    public async Task<string> ReceiveAsync()
    {
        var buffer = new byte[8192];
        var received = await _client.ReceiveAsync(buffer, SocketFlags.None);
        if (received is not 0)
        {
            var response = Encoding.UTF8.GetString(buffer, 0, received);
            return await Task.FromResult(response);
        }

        return string.Empty;
    }

    public void Dispose()
    {
        _client.Shutdown(SocketShutdown.Both);
        _client.Dispose();
    }
}