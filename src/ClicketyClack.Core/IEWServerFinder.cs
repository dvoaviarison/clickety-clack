// Copyright (c) 2024 DVoaviarison
using ClicketyClack.Core.Models;

namespace ClicketyClack.Core;

public interface IEWServerFinder
{
    Task<ServerInfo> FindAsync();
}