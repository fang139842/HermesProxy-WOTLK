using Framework;
using HermesProxy.World;

namespace HermesProxy.World.Server.Packets;

public class ZoneUpdatePkt : ClientPacket
{
    public uint ZoneId;
    public ZoneUpdatePkt(WorldPacket packet) : base(packet) { }
    public override void Read()
    {
        this.ZoneId = base._worldPacket.ReadUInt32();
    }
}

public class GMTicketUpdateTextPkt : ClientPacket
{
    public string Message;
    public GMTicketUpdateTextPkt(WorldPacket packet) : base(packet) { }
    public override void Read()
    {
        this.Message = base._worldPacket.ReadCString();
    }
}
