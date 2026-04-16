using Framework;
using HermesProxy.World;
using Ionic.Zlib;
using System.IO;

namespace HermesProxy.World.Server.Packets;

public class AddonListPkt : ClientPacket
{
    public uint AddonCount;
    public AddonListPkt(WorldPacket packet) : base(packet) { }
    public override void Read()
    {
        try {
            uint compressedSize = base._worldPacket.ReadUInt32();
            if (compressedSize > 0)
            {
                byte[] compressed = base._worldPacket.ReadBytes(compressedSize);
                using (var ms = new MemoryStream(compressed))
                using (var zlib = new ZlibStream(ms, CompressionMode.Decompress))
                using (var outMs = new MemoryStream())
                {
                    zlib.CopyTo(outMs);
                    byte[] decompressed = outMs.ToArray();
                    using (var reader = new WorldPacket(decompressed))
                    {
                        this.AddonCount = reader.ReadUInt32();
                    }
                }
            }
        } catch { }
    }
}
