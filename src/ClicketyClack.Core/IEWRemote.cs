// Copyright (c) 2024 DVoaviarison
namespace ClicketyClack.Core;

public interface IEWRemoteSimulator : IDisposable
{
    Task SetupPairingAsync(CancellationToken cancellationToken);

    Task NextSlideAsync();

    Task PreviousSlideAsync();

    Task TerminatePairingAsync();
}