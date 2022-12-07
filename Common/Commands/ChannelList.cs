namespace CC.Common.Commands;

public class ChannelList : AbstractCommand
{
    public ChannelList()
    {
    }
    public ChannelList(ICommandsReceiver receiver)
        : base(receiver)
    {
    }
    internal ChannelList(ICommandsReceiver receiver, BinaryReader data)
        : base(receiver)
    {
    }
    public override Task Execute()
    {
        if (_receiver == null)
            throw new InvalidOperationException($"{nameof(_receiver)} should not be null");
        return _receiver.ChannelList(this);
    }

    public override void Serialize(BinaryWriter data)
    {
        data.Write((byte)CommandKind.ChannelList);
    }
}