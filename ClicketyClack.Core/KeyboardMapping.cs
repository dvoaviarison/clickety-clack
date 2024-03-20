// Copyright (c) 2024 DVoaviarison
namespace ClicketyClack.Core;

public class KeyboardMapping
{
    public static readonly HashSet<ConsoleKey> NextKeys =
    [
        ConsoleKey.RightArrow,
        ConsoleKey.DownArrow,
        ConsoleKey.Spacebar,
        ConsoleKey.PageDown
    ];
    
    public static readonly HashSet<ConsoleKey> PreviousKeys =
    [
        ConsoleKey.LeftArrow,
        ConsoleKey.UpArrow,
        ConsoleKey.Backspace,
        ConsoleKey.PageUp
    ];
}