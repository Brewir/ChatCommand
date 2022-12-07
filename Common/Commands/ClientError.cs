namespace CC.Common.Commands;

public class ClientError : AbstractCommand
{
    public string Message { get; }

    public ClientError(string message)
    {
        Message = message;
    }
    public ClientError(ICommandsReceiver receiver, string message)
        : base(receiver)
    {
        Message = message;
    }
    internal ClientError(ICommandsReceiver receiver, BinaryReader data)
        : base(receiver)
    {
        Message = data.ReadString();
    }
    public override Task Execute()
    {
        if (_receiver == null)
            throw new InvalidOperationException($"{nameof(_receiver)} should not be null");
        return _receiver.ClientError(this);
    }

    public override void Serialize(BinaryWriter data)
    {
        data.Write((byte)CommandKind.ClientError);
        data.Write(Message);
    }
}