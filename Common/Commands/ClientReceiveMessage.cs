namespace CC.Common.Commands;

public class ClientReceiveMessage : AbstractCommand
{
    public ChatMessage Message = new();
    public ClientReceiveMessage(string sender, string message)
    {
        Message.Sender = sender;
        Message.Message = message;
    }
    public ClientReceiveMessage(ChatMessage message)
    {
        Message = message;
    }
    public ClientReceiveMessage(ICommandsReceiver receiver, ChatMessage message)
        : base(receiver)
    {
        Message = message;
    }
    public ClientReceiveMessage(ICommandsReceiver receiver, string sender, string message)
        : base(receiver)
    {
        Message.Sender = sender;
        Message.Message = message;
    }
    internal ClientReceiveMessage(ICommandsReceiver receiver, BinaryReader data)
        : base(receiver)
    {
        Message = ChatMessage.Deserialize(data);
    }
    public override Task Execute()
    {
        if (_receiver == null)
            throw new InvalidOperationException($"{nameof(_receiver)} should not be null");
        return _receiver.ClientReceiveMessage(this);
    }

    public override void Serialize(BinaryWriter data)
    {
        data.Write((byte)CommandKind.ClientReceiveMessage);
        Message.Serialize(data);
    }
}