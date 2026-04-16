using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class AccountDataTimes : ServerPacket
{
	public WowGuid128 PlayerGuid;
	public uint ServerTime;
	public uint Mask;
	public uint[] AccountTimes;

	public AccountDataTimes()
		: base(Opcode.SMSG_ACCOUNT_DATA_TIMES)
	{
	}

	public override void Write()
	{
		if (ModernVersion.ExpansionVersion >= 3)
		{
			// 3.4.3: ServerTime (uint32) + Mask (uint32) + times
			base._worldPacket.WriteUInt32(this.ServerTime);
			base._worldPacket.WriteUInt32(this.Mask);
			for (int i = 0; i < 32; i++)
			{
				if ((this.Mask & (1u << i)) != 0)
				{
					base._worldPacket.WriteUInt32(this.AccountTimes[i]);
				}
			}
		}
		else
		{
			base._worldPacket.WritePackedGuid128(this.PlayerGuid);
			base._worldPacket.WriteUInt32(this.ServerTime);
			base._worldPacket.WriteUInt32(this.Mask);
			for (int i = 0; i < 32; i++)
			{
				if ((this.Mask & (1u << i)) != 0)
				{
					base._worldPacket.WriteUInt32(this.AccountTimes[i]);
				}
			}
		}
	}
}
