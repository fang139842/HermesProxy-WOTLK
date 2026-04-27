// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using static Arctium.Game.Launcher.Misc.NativeWindows;
using static Arctium.Game.Launcher.Misc.Helpers;

namespace Arctium.Game.Launcher.IO;

class WinMemory
{
    public byte[] Data { get; set; }
    public nint BaseAddress { get; }
    
    readonly nint _processHandle;

    public WinMemory(ProcessInformation processInformation)
    {
        _processHandle = processInformation.ProcessHandle;

        if (processInformation.ProcessHandle == 0)
            throw new InvalidOperationException("No valid process found.");

        BaseAddress = ReadImageBaseFromPEB(processInformation.ProcessHandle);

        if (BaseAddress == 0)
            throw new InvalidOperationException("Error while reading PEB data.");
    }

    public void RefreshMemoryData(int size)
    {
        // Reset previous memory data.
        if (Data != null)
            Array.Clear(Data, 0, Data.Length);

        while (Data == null || Unsafe.ReadUnaligned<long>(ref Data[0]) == 0)
        {
            Console.WriteLine("Refreshing client data...");
            ReadToData(BaseAddress, size);
        }
    }

    public nint Read(nint address)
    {
        try
        {
            var buffer = new byte[8];

            if (ReadProcessMemory(_processHandle, address, buffer, buffer.Length, out var dummy))
                return buffer.ToNint();

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return 0;
    }

    public void ReadToData(nint address, int size)
    {
        try
        {
            Data ??= new byte[size];

            ReadProcessMemory(_processHandle, address, Data, size, out var dummy);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }


    public byte[] Read(nint address, int size)
    {
        try
        {
            var buffer = new byte[size];

            if (ReadProcessMemory(_processHandle, address, buffer, size, out var dummy))
                return buffer;

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return null;
    }

    public void Write(nint address, byte[] data, MemProtection newProtection = MemProtection.ReadWrite)
    {
        try
        {
            VirtualProtectEx(_processHandle, address, (uint)data.Length, (uint)newProtection, out var oldProtect);

            WriteProcessMemory(_processHandle, address, data, data.Length, out var written);

            FlushInstructionCache(_processHandle, address, (uint)data.Length);
            VirtualProtectEx(_processHandle, address, (uint)data.Length, oldProtect, out oldProtect);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public async Task TryPatchPatterns(byte[][] patches, string patchName, bool? printInfo = null, bool exitOnFail = true, bool patchAll = false, params short[][] patterns)
    {
        if (patches.Length == 0 || patterns.Length == 0)
            return;

        if (!patchAll && await PatchMemory(patterns[0], patches[0], $"{patchName}", printInfo, exitOnFail))
            return;

        for (var i = 0; i < patterns.Length; i++)
        {
            Console.ResetColor();

            if (await PatchMemory(patterns[i], patches[i], $"{patchName} {i + 1}", printInfo, exitOnFail: false) && !patchAll)
                break;
        }
    }

    public Task<bool> PatchMemory(short[] pattern, byte[] patch, string patchName, bool? printInfo = null, bool exitOnFail = true)
    {
        printInfo ??= IsDebugBuild();

        if (printInfo.Value)
            Console.WriteLine($"[{patchName}] Patching...");

        long patchOffset = Data.FindPattern(pattern, BaseAddress);

        // No result for the given pattern.
        if (patchOffset == 0)
        {
            if (exitOnFail)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{patchName}] No result found.");
                Console.WriteLine("Press any key to exit...");

                // Only wait if a console is available.
                if (!Console.IsInputRedirected)
                    Console.ReadKey();

                Launcher.CancellationTokenSource.Cancel();

                return Task.FromResult(false);
            }

            if (printInfo.Value)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[{patchName}] No result found. This is just a warning! KEEP GOING...");
                Console.ResetColor();
            }

            return Task.FromResult(false);
        }

        while (Read((nint)patchOffset, patch.Length)?.SequenceEqual(patch) == false)
            Write((nint)patchOffset, patch);

        if (printInfo.Value)
        {
            Console.Write($"[{patchName}] at 0x{patchOffset:X}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" Done.");
            Console.ResetColor();
            Console.WriteLine();
        }

        return Task.FromResult(true);
    }

    nint ReadImageBaseFromPEB(nint processHandle)
    {
        try
        {
            ProcessBasicInformation peb = default;

            if (NtQueryInformationProcess(processHandle, 0, ref peb, ProcessBasicInformation.Size, out _) == 0)
                return Read(peb.PebBaseAddress + 0x10);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return 0;
    }
}
