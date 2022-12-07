namespace CC.Common.Commands;

public class ChannelJoin : AbstractCommand
{
    public string ChannelName { get; }
    public ChannelJoin(string channelName)
    {
        ChannelName = channelName;
    }
    public ChannelJoin(ICommandsReceiver receiver, string channelName)
        : base(receiver)
    {
        ChannelName = channelName;
    }
    internal ChannelJoin(ICommandsReceiver receiver, BinaryReader data)
        : base(receiver)
    {
        ChannelName = data.ReadString();
    }
    public override Task Execute()
    {
        if (_receiver == null)
            throw new InvalidOperationException($"{nameof(_receiver)} should not be null");
        return _receiver.ChannelJoin(this);
    }

    public override void Serialize(BinaryWriter data)
    {
        data.Write((byte)CommandKind.ChannelJoin);
        data.Write(ChannelName);
    }
}