using Framework;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World;

public abstract class ServerPacket
{
	private byte[] buffer;

	private ConnectionType connectionType;

	private Opcode _universalOpcode;

	protected WorldPacket _worldPacket;

	public bool SkipSend { get; set; }

	protected ServerPacket(Opcode universalOpcode)
	{
		this.connectionType = ConnectionType.Realm;
		this._universalOpcode = universalOpcode;
		uint opcode = ModernVersion.GetCurrentOpcode(universalOpcode);
		this._worldPacket = new WorldPacket(opcode);
	}

	protected ServerPacket(Opcode universalOpcode, ConnectionType type = ConnectionType.Realm)
	{
		this.connectionType = type;
		this._universalOpcode = universalOpcode;
		uint opcode = ModernVersion.GetCurrentOpcode(universalOpcode);
		this._worldPacket = new WorldPacket(opcode);
	}

	public void Clear()
	{
		this._worldPacket.Clear();
		this.buffer = null;
	}

	public uint GetOpcode()
	{
		return this._worldPacket.GetOpcode();
	}

	public Opcode GetUniversalOpcode()
	{
		return this._universalOpcode;
	}

	public byte[] GetData()
	{
		return this.buffer;
	}

	public void LogPacket(ref SniffFile sniffFile)
	{
		if (Settings.PacketsLog)
		{
			if (sniffFile == null)
			{
				sniffFile = new SniffFile("modern", (ushort)Settings.ClientBuild);
				sniffFile.WriteHeader();
			}
			sniffFile.WritePacket(this.GetOpcode(), isFromClient: false, this.GetData());
		}
	}

	public abstract void Write();

	public void WritePacketData()
	{
		if (this.buffer == null)
		{
			this.Write();
			this.buffer = this._worldPacket.GetData();
			this._worldPacket.Dispose();
		}
	}

	public ConnectionType GetConnection()
	{
		return this.connectionType;
	}
}
