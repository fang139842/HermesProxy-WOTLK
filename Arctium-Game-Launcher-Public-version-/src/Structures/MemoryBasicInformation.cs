// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Arctium.Game.Launcher.Structures;

[StructLayout(LayoutKind.Sequential)]
struct MemoryBasicInformation
{
    public nint BaseAddress;
    public nint AllocationBase;
    public MemProtection AllocationProtect;
    public nint RegionSize;
    public int State;
    public MemProtection Protect;
    public int Type;

    public static int Size => Marshal.SizeOf<MemoryBasicInformation>();
}
