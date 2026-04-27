// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CommandLine.Parsing;
using Arctium.Game.Launcher;

using static Arctium.Game.Launcher.Misc.Helpers;

PublisherCheck();
PrintHeader();

var versionInfo = FileVersionInfo.GetVersionInfo(Environment.ProcessPath);
Console.WriteLine($"Using {(int)(Environment.ProcessorCount * 0.75)} of {Environment.ProcessorCount} processors.");
Console.WriteLine();

PublisherCheck();

LaunchOptions.RootCommand.SetHandler(async context =>
{
    try
    {
        // Prefer / instead of \ for the client path.
        var appPath = (await Launcher.PrepareGameLaunch(context.ParseResult, default)).Replace("\\", "/");
        var gameCommandLine = string.Join(" ", context.ParseResult.UnmatchedTokens);

        // Add config parameter to the game command line.
        gameCommandLine += $" -config {context.ParseResult.GetValueForOption(LaunchOptions.GameConfig)}";

        if (string.IsNullOrEmpty(appPath) || !Launcher.LaunchGame(appPath, gameCommandLine, context.ParseResult))
            WaitAndExit(5000);
    }
    catch (Exception ex)
    {
        if (Launcher.GameProcessHandle != nint.Zero)
            NativeWindows.TerminateProcess(Launcher.GameProcessHandle, 0);

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(ex.Message);
        Console.ResetColor();
    }
});

await LaunchOptions.Instance.InvokeAsync(args);

static void WaitAndExit(int ms = 2000)
{
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.WriteLine($"Closing in {ms / 1000} seconds...");

    Thread.Sleep(ms);

    Environment.Exit(0);
}