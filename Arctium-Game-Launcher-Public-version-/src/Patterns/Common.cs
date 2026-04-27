// Copyright (c) Arctium.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Arctium.Game.Launcher.Patterns;

static class Common
{
    public static short[] ConnectToModulus = { 0x91, 0xD5, 0x9B, 0xB7, 0xD4, 0xE1, 0x83, 0xA5 };
    public static short[] CryptoRsaModulus = { 0x71, 0xFD, 0xFA, 0x60, 0x14, 0x0D, 0xF2, 0x05 };
    public static short[] CryptoEdPublicKey = { 0x15, 0xD6, 0x18, 0xBD, 0x7D, 0xB5, 0x77, 0xBD, 0x9A, 0x8D, 0x45, 0x76, 0x9C, 0x59, 0xE4, 0xFC, 0x63 };

    public static string VersionUrl = "http://%s.patch.battle.net:1119/%s/versions";
    public static string Version2Url = "https://%s.version.battle.net/v2/products/%s/versions";
    public static string Version2UrlNew = "https://%s.version.battle.net/v2/products/%s/%s";
    public static string Version2ChinaUrl = "https://cn.version.battlenet.com.cn/v2/products/%s/versions";
    public static string Version2ChinaUrlNew = "https://cn.version.battlenet.com.cn/v2/products/%s/%s";
    public static string CdnsUrl = "http://%s.patch.battle.net:1119/%s/cdns";
    public static short[] Portal = ".actual.battle.net\0".ToPattern();
}
