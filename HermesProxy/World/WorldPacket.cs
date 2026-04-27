using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Framework.IO;
using HermesProxy.World.Client;
using HermesProxy.World.Enums;

namespace HermesProxy.World;

public class WorldPacket : ByteBuffer
{
	private uint opcode;

	private long m_receivedTime;

	public WorldPacket(uint opcode = 0u)
	{
		this.opcode = opcode;
	}

	public WorldPacket(Opcode opcode)
	{
		this.opcode = LegacyVersion.GetCurrentOpcode(opcode);
	}

	public WorldPacket(uint opcode, byte[] data)
		: base(data)
	{
		this.opcode = opcode;
	}

	public WorldPacket(byte[] data)
		: base(data)
	{
		this.opcode = base.ReadUInt16();
	}

	public KeyValuePair<int, bool> ReadEntry()
	{
		uint entry = base.ReadUInt32();
		uint realEntry = entry & 0x7FFFFFFF;
		return new KeyValuePair<int, bool>((int)realEntry, realEntry != entry);
	}

	public WowGuid64 ReadGuid()
	{
		return new WowGuid64(base.ReadUInt64());
	}

	public WowGuid64 ReadPackedGuid()
	{
		return new WowGuid64(this.ReadPackedUInt64(base.ReadUInt8()));
	}

	public WowGuid128 ReadPackedGuid128()
	{
		byte loLength = base.ReadUInt8();
		byte hiLength = base.ReadUInt8();
		ulong low = this.ReadPackedUInt64(loLength);
		return new WowGuid128(this.ReadPackedUInt64(hiLength), low);
	}

	private ulong ReadPackedUInt64(byte length)
	{
		if (length == 0)
		{
			return 0uL;
		}
		ulong guid = 0uL;
		for (int i = 0; i < 8; i++)
		{
			if (((1 << i) & length) != 0)
			{
				guid |= (ulong)base.ReadUInt8() << i * 8;
			}
		}
		return guid;
	}

	public UpdateField ReadUpdateField()
	{
		uint val = base.ReadUInt32();
		return new UpdateField(val);
	}

	public WorldPacket Inflate(int inflatedSize)
	{
		byte[] arr = base.ReadToEnd();
		using (MemoryStream ms = new MemoryStream(arr))
		using (DeflateStream zlib = new DeflateStream(ms, CompressionMode.Decompress))
		using (MemoryStream outMs = new MemoryStream())
		{
			zlib.CopyTo(outMs);
			byte[] newarr = outMs.ToArray();
			WorldPacket pkt = new WorldPacket(this.GetOpcode(), newarr);
			pkt.SetReceiveTime(this.GetReceivedTime());
			return pkt;
		}
	}

	public void WriteGuid(WowGuid64 guid)
	{
		base.WriteUInt64(guid.GetLowValue());
	}

	public void WritePackedGuid(WowGuid64 guid)
	{
		this.WritePackedUInt64(guid.Low);
	}

	public void WritePackedGuid128(WowGuid128 guid)
	{
		if (guid.IsEmpty())
		{
			base.WriteUInt8(0);
			base.WriteUInt8(0);
			return;
		}
		byte lowMask;
		byte[] lowPacked;
		uint loSize = this.PackUInt64(guid.GetLowValue(), out lowMask, out lowPacked);
		byte highMask;
		byte[] highPacked;
		uint hiSize = this.PackUInt64(guid.GetHighValue(), out highMask, out highPacked);
		base.WriteUInt8(lowMask);
		base.WriteUInt8(highMask);
		base.WriteBytes(lowPacked, loSize);
		base.WriteBytes(highPacked, hiSize);
	}

	public void WritePackedUInt64(ulong guid)
	{
		byte mask;
		byte[] packed;
		uint packedSize = this.PackUInt64(guid, out mask, out packed);
		base.WriteUInt8(mask);
		base.WriteBytes(packed, packedSize);
	}

	private uint PackUInt64(ulong value, out byte mask, out byte[] result)
	{
		uint resultSize = 0u;
		mask = 0;
		result = new byte[8];
		byte i = 0;
		while (value != 0)
		{
			if ((value & 0xFF) != 0)
			{
				mask |= (byte)(1 << (int)i);
				result[resultSize++] = (byte)(value & 0xFF);
			}
			value >>= 8;
			i++;
		}
		return resultSize;
	}

	public void WriteBytes(WorldPacket data)
	{
		base.FlushBits();
		base.WriteBytes(data.GetData());
	}

	public uint GetOpcode()
	{
		return this.opcode;
	}

	public Opcode GetUniversalOpcode(bool isModern)
	{
		if (isModern)
		{
			return ModernVersion.GetUniversalOpcode(this.GetOpcode());
		}
		return LegacyVersion.GetUniversalOpcode(this.GetOpcode());
	}

	public long GetReceivedTime()
	{
		return this.m_receivedTime;
	}

	public void SetReceiveTime(long receivedTime)
	{
		this.m_receivedTime = receivedTime;
	}
}
