namespace CC.Common.Commands;

public class ChannelSendMessage : AbstractCommand
{
    public string Message { get; }
    public ChannelSendMessage(string message)
    {
        Message = message;
    }
    public ChannelSendMessage(ICommandsReceiver receiver, string message)
        : base(receiver)
    {
        Message = message;
    }
    internal ChannelSendMessage(ICommandsReceiver receiver, BinaryReader data)
        : base(receiver)
    {
        Message = data.ReadString();
    }
    public override Task Execute()
    {
        if (_receiver == null)
            throw new InvalidOperationException($"{nameof(_receiver)} should not be null");
        return _receiver.ChannelSendMessage(this);
    }

    public override void Serialize(BinaryWriter data)
    {
        data.Write((byte)CommandKind.ChannelSendMessage);
        data.Write(Message);
    }
}