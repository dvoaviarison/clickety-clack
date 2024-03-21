// Copyright (c) 2024 DVoaviarison
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClicketyClack.Core;

public class EWCLient : IEWClient
{
    private readonly Socket _client;
    private readonly IPEndPoint _ipEndPoint;


    public EWCLient(string ipAddress, int port)
    {
        _ipEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
        _client = new Socket(
            _ipEndPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);
    }

    public async Task ConnectAsync()
    {
        await _client.ConnectAsync(_ipEndPoint);
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