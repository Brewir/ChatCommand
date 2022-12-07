namespace CC.Common.Commands;

public class ClientSuccess : AbstractCommand
{
    public ClientSuccess()
    {
    }
    public ClientSuccess(ICommandsReceiver receiver)
        : base(receiver)
    {
    }
    internal ClientSuccess(ICommandsReceiver receiver, BinaryReader data)
        : base(receiver)
    {
    }
    public override Task Execute()
    {
        if (_receiver == null)
            throw new InvalidOperationException($"{nameof(_receiver)} should not be null");
        return _receiver.ClientSuccess(this);
    }

    public override void Serialize(BinaryWriter data)
    {
        data.Write((byte)CommandKind.ClientSuccess);
    }
}