// Copyright (c) 2024 DVoaviarison

using ClicketyClack.Core.Models;

namespace ClicketyClack.Core;

public interface IEWClient : IDisposable
{
    Task ConnectAsync(ServerInfo serverInfo);
    
    Task DisconnectAsync();

    Task SendAsync(string message);

    Task<string> ReceiveAsync();
}
