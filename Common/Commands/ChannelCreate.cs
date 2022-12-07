namespace CC.Common.Commands;

public class ChannelCreate : AbstractCommand
{
    public string ChannelName { get; }
    public ChannelCreate(string channelName)
    {
        ChannelName = channelName;
    }
    public ChannelCreate(ICommandsReceiver receiver, string channelName)
        : base(receiver)
    {
        ChannelName = channelName;
    }
    internal ChannelCreate(ICommandsReceiver receiver, BinaryReader data)
        : base(receiver)
    {
        ChannelName = data.ReadString();
    }
    public override Task Execute()
    {
        if (_receiver == null)
            throw new InvalidOperationException($"{nameof(_receiver)} should not be null");
        return _receiver.ChannelCreate(this);
    }

    public override void Serialize(BinaryWriter data)
    {
        data.Write((byte)CommandKind.ChannelCreate);
        data.Write(ChannelName);
    }
}