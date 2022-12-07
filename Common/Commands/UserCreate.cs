namespace CC.Common.Commands;

public class UserCreate : AbstractCommand
{
    public string Login { get; }
    public string Password { get; }
    public UserCreate(string username, string password)
    {
        Login = username;
        Password = password;
    }
    public UserCreate(ICommandsReceiver receiver, string username, string password)
        : base(receiver)
    {
        Login = username;
        Password = password;
    }
    internal UserCreate(ICommandsReceiver receiver, BinaryReader data)
        : base(receiver)
    {
        Login = data.ReadString();
        Password = data.ReadString();
    }
    public override Task Execute()
    {
        if (_receiver == null)
            throw new InvalidOperationException($"{nameof(_receiver)} should not be null");
        return _receiver.UserCreate(this);
    }

    public override void Serialize(BinaryWriter data)
    {
        data.Write((byte)CommandKind.UserCreate);
        data.Write(Login);
        data.Write(Password);
    }
}