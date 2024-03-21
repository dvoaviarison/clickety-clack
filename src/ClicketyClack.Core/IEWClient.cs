// Copyright (c) 2024 DVoaviarison
namespace ClicketyClack.Core;

public interface IEWClient : IDisposable
{
    Task ConnectAsync();
    
    Task DisconnectAsync();

    Task SendAsync(string message);

    Task<string> ReceiveAsync();
}
