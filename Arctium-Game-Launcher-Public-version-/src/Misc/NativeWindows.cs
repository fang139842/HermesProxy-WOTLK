// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Arctium.Game.Launcher.Misc;

static class NativeWindows
{
    [DllImport("kernel32.dll", EntryPoint = "CreateProcessA", SetLastError = true)]
    public static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, nint lpProcessAttributes, nint lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, nint lpEnvironment, string lpCurrentDirectory, ref StartupInfo lpStartupInfo, out ProcessInformation lpProcessInformation);

    [DllImport("kernel32.dll", EntryPoint = "TerminateProcess")]
    public static extern void TerminateProcess(nint processHandle, int exitCode);

    [DllImport("kernel32.dll", EntryPoint = "CloseHandle")]
    public static extern void CloseHandle(nint handle);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool VirtualProtectEx(nint hProcess, nint lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool ReadProcessMemory(nint hProcess, nint lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesRead);

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern bool WriteProcessMemory(nint hProcess, nint lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);

    [DllImport("kernel32.dll", EntryPoint = "FlushInstructionCache", SetLastError = true)]
    public static extern bool FlushInstructionCache(nint hProcess, nint lpBaseAddress, uint dwSize);

    [DllImport("ntdll.dll", SetLastError = true)]
    public static extern int NtQueryInformationProcess(nint hProcess, int pic, ref ProcessBasicInformation pbi, int cb, out int pSize);

    [DllImport("ntdll.dll", SetLastError = true)]
    public static extern nint NtResumeProcess(nint processHandle);

    [DllImport("ntdll.dll", SetLastError = true)]
    public static extern nint NtSuspendProcess(nint processHandle);
}
