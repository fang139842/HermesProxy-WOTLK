// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Arctium.Game.Launcher.Misc;

static class Extensions
{
    public static nint ToNint(this byte[] buffer) => (nint)BitConverter.ToInt64(buffer, 0);

    public static long FindPattern(this byte[] data, short[] pattern, long start, long baseOffset = 0)
    {
        long matches;

        for (long i = start; i < data.Length; i++)
        {
            if (pattern.Length > (data.Length - i))
                return 0;

            for (matches = 0; matches < pattern.Length; matches++)
            {
                if ((pattern[matches] != -1) && (data[i + matches] != (byte)pattern[matches]))
                    break;
            }

            if (matches == pattern.Length)
                return baseOffset + i;
        }

        return 0;
    }

    public static long FindPattern(this byte[] data, short[] pattern, long baseOffset = 0) => FindPattern(data, pattern, 0L, baseOffset);
    public static short[] ToPattern(this string data) => Encoding.UTF8.GetBytes(data).Select(b => (short)b).ToArray();
}
