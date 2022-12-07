using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.HighPerformance;

namespace CC.Common.Commands;

public abstract class AbstractCommand
{
    protected ICommandsReceiver? _receiver;

    public AbstractCommand()
    {

    }
    public AbstractCommand(ICommandsReceiver receiver)
    {
        _receiver = receiver;
    }
    public void SetReceiver(ICommandsReceiver receiver) => _receiver = receiver;
    public abstract Task Execute();
    public abstract void Serialize(BinaryWriter data);
    public static AbstractCommand Deserialize(ICommandsReceiver receiver, BinaryReader data)
    {
        var kind = (CommandKind)data.ReadByte();
        return kind switch
        {
            CommandKind.UserCreate => new UserCreate(receiver, data),
            CommandKind.UserLogin => new UserLogin(receiver, data),
            CommandKind.ChannelCreate => new ChannelCreate(receiver, data),
            CommandKind.ChannelRemove => new ChannelRemove(receiver, data),
            CommandKind.ChannelList => new ChannelList(receiver, data),
            CommandKind.ChannelJoin => new ChannelJoin(receiver, data),
            CommandKind.ChannelSendMessage => new ChannelSendMessage(receiver, data),
            CommandKind.ClientReceiveMessage => new ClientReceiveMessage(receiver, data),
            CommandKind.ClientError => new ClientError(receiver, data),
            CommandKind.ClientSuccess => new ClientSuccess(receiver, data),
            _ => throw new NotImplementedException()
        };
    }
    public static Command SerializeProtobuf(AbstractCommand command)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        command.Serialize(writer);
        stream.Position = 0;
        return new Command { Data = ByteString.FromStream(stream) };
    }
    public static ReadOnlyMemory<byte> SerializeCommand(AbstractCommand command)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        command.Serialize(writer);
        stream.Position = 0;
        return stream.ToArray();
    }
    public static AbstractCommand DeserializeCommand(ICommandsReceiver receiver, ReadOnlyMemory<byte> data)
    {
        using var reader = new BinaryReader(data.AsStream());
        return Deserialize(receiver, reader);
    }
}