// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net;
using System.Net.Sockets;

namespace Arctium.Game.Launcher.Misc;

static class Helpers
{
    public static bool IsDebugBuild()
    {
#if DEBUG
        return true;
#else
        return false;
#endif
    }

    public static void PublisherCheck()
    {
        // "Arctium" should not be removed from the final binary name.
        if (!Process.GetCurrentProcess().ProcessName.Contains("arctium", StringComparison.InvariantCultureIgnoreCase))
            Process.GetCurrentProcess().Kill();
    }

    public static (int Major, int Minor, int Revision, int Build) GetVersionValueFromClient(string fileName)
    {
        var fileVersionInfo = FileVersionInfo.GetVersionInfo(fileName);
        var major = fileVersionInfo.FileMajorPart;
        var minor = fileVersionInfo.FileMinorPart;
        var build = fileVersionInfo.FileBuildPart;
        var privatePart = fileVersionInfo.FilePrivatePart;

        // Special case for build numbers greater than 65535.
        // Note: fileVersionInfo.FileVersion is an alternative based on string parsing.
        if (build >= 6553 && privatePart < ushort.MaxValue)
        {
            privatePart = build * 10 + privatePart;
            build = minor;
            minor = major % 100;
            major /= 100;
        }

        return (major, minor, build, privatePart);
    }

    public static void PrintHeader()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Arctium Game Launcher");
        Console.ResetColor();
        Console.Write("Game: ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("World of Warcraft");
        Console.ResetColor();
        Console.Write("Support: ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("https://arctium.io");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine($"Operating System: {RuntimeInformation.OSDescription}");
    }

    public static (string IPAddress, string HostName, int Port) ParseOrSetPortal(ref string config, string customPortal = null)
    {
        const string portalKey = "SET portal";

        var portalIndex = config.IndexOf(portalKey, StringComparison.OrdinalIgnoreCase);

        if (portalIndex == -1)
        {
            if (string.IsNullOrEmpty(customPortal))
                throw new ArgumentException("Config file does not contain a valid portal variable.");

            // Append a new portal variable to the config.
            config += $"{Environment.NewLine}{portalKey} \"{customPortal}\"";
        }

        // Re-evaluate the portal variable.
        portalIndex = config.IndexOf(portalKey, StringComparison.OrdinalIgnoreCase);

        if (portalIndex == -1)
            throw new ArgumentException("Config file does not contain a valid portal variable.");

        var startQuoteIndex = config.IndexOf('"', portalIndex);

        if (startQuoteIndex == -1)
            throw new ArgumentException("Invalid format for the portal variable.");

        var endQuoteIndex = config.IndexOf('"', startQuoteIndex + 1);

        if (endQuoteIndex == -1)
            throw new ArgumentException("Invalid format for the portal variable.");

        var portalLength = endQuoteIndex - startQuoteIndex - 1;
        var portalSpan = config.AsSpan(startQuoteIndex + 1, portalLength);
        var colonIndex = portalSpan.IndexOf(':');
        var ipSpan = colonIndex != -1 ? portalSpan[..colonIndex] : portalSpan;
        var port = colonIndex != -1 ? int.Parse(portalSpan[(colonIndex + 1)..]) : 1119;

        // Check if we have more than one portal line.
        var lastPortalIndex = config.IndexOf(portalKey, portalIndex + portalLength, StringComparison.OrdinalIgnoreCase);

        if (lastPortalIndex != -1 && lastPortalIndex != portalIndex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine("Client portal is set multiple times. Please check your config file!");
            Console.ResetColor();

            return (string.Empty, string.Empty, port);
        }

        var portalString = ipSpan.ToString().Trim();

        // Override portal variable if a custom one is provided.
        if (!string.IsNullOrEmpty(customPortal))
        {
            portalString = customPortal;
            config = string.Concat(config.AsSpan(0, startQuoteIndex + 1), portalString, config.AsSpan(endQuoteIndex));

            // Let's re-parse the new portal for verification purposes.
            return ParseOrSetPortal(ref config);
        }

        try
        {
            if (IPAddress.TryParse(portalString, out var ipAddress))
                return (ipAddress.ToString(), portalString, port);

            var ipv4Address = Dns.GetHostAddresses(portalString).FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);

            if (ipv4Address == null)
            {
                var ipv6Address = Dns.GetHostAddresses(portalString).FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetworkV6);

                if (ipv6Address == null)
                    throw new Exception("No IPv4/IPv6 address found for the provided host name.");

                return (ipv6Address.ToString(), portalString, port);
            }

            return (ipv4Address.ToString(), portalString, port);
        }
        catch (SocketException socketException) when (socketException.SocketErrorCode == SocketError.HostNotFound)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"DNS resolution failed for portal: {portalString}");
            Console.ForegroundColor = ConsoleColor.Gray;

            return (string.Empty, portalString, port);
        }
        catch (SocketException)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("No valid portal found.");
            Console.ForegroundColor = ConsoleColor.Gray;

            return (string.Empty, portalString, port);
        }
    }

    public static async Task<bool> CheckUrl(string url, string[] fallbackUrls)
    {
        using var httpClient = new HttpClient();

        httpClient.Timeout = TimeSpan.FromSeconds(5);

        try
        {
            var result = await httpClient.GetAsync(url);

            if (!result.IsSuccessStatusCode)
            {
                Console.WriteLine();
                Console.WriteLine($"{url} not reachable. Falling back to: ");

                foreach (var furl in fallbackUrls)
                    Console.WriteLine($"- {furl}");
            }

            return result.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            Console.WriteLine();
            Console.WriteLine($"{url} not reachable. Falling back to: ");

            foreach (var furl in fallbackUrls)
                Console.WriteLine($"- {furl}");

            return false;
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool UsesVersionV2(byte[] appPath)
    {
        return appPath.FindPattern(Patterns.Common.Version2UrlNew.ToPattern()) > 0;
    }
}
