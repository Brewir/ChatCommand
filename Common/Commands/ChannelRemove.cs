namespace CC.Common.Commands;

public class ChannelRemove : AbstractCommand
{
    public string ChannelName { get; }
    public ChannelRemove(string channelName)
    {
        ChannelName = channelName;
    }
    public ChannelRemove(ICommandsReceiver receiver, string channelName)
        : base(receiver)
    {
        ChannelName = channelName;
    }
    internal ChannelRemove(ICommandsReceiver receiver, BinaryReader data)
        : base(receiver)
    {
        ChannelName = data.ReadString();
    }
    public override Task Execute()
    {
        if (_receiver == null)
            throw new InvalidOperationException($"{nameof(_receiver)} should not be null");
        return _receiver.ChannelRemove(this);
    }

    public override void Serialize(BinaryWriter data)
    {
        data.Write((byte)CommandKind.ChannelRemove);
        data.Write(ChannelName);
    }
}