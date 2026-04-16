using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class InitializeFactions : ServerPacket
{
	private const ushort MaxFactionCount = 1000;

	public int[] FactionStandings = new int[1000];

	public bool[] FactionHasBonus = new bool[1000];

	public ReputationFlags[] FactionFlags = new ReputationFlags[1000];

	public uint Count = 0;

	public InitializeFactions()
		: base(Opcode.SMSG_INITIALIZE_FACTIONS, ConnectionType.Instance)
	{
	}

	public override void Write()
	{
		if (ModernVersion.ExpansionVersion >= 3)
		{
			// 3.4.3: uint32 count + for each: (uint8 flag + int32 standing) + for each: bit hasBonus
			base._worldPacket.WriteUInt32(this.Count);
			for (uint i = 0; i < this.Count; i++)
			{
				base._worldPacket.WriteUInt8((byte)this.FactionFlags[i]);
				base._worldPacket.WriteInt32(this.FactionStandings[i]);
			}
			for (uint i = 0; i < this.Count; i++)
			{
				base._worldPacket.WriteBit(this.FactionHasBonus[i]);
			}
			base._worldPacket.FlushBits();
		}
		else
		{
			// Legacy fallback or other versions
			base._worldPacket.WriteUInt32(this.Count);
			for (uint i = 0; i < this.Count; i++)
			{
				base._worldPacket.WriteUInt8((byte)this.FactionFlags[i]);
				base._worldPacket.WriteInt32(this.FactionStandings[i]);
			}
		}
	}
}
