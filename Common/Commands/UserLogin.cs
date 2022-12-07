namespace CC.Common.Commands;

public class UserLogin : AbstractCommand
{
    public string Login { get; }
    public string Password { get; }
    public UserLogin(string loggin, string password)
    {
        Login = loggin;
        Password = password;
    }
    public UserLogin(ICommandsReceiver receiver, string loggin, string password)
        : base(receiver)
    {
        Login = loggin;
        Password = password;
    }
    internal UserLogin(ICommandsReceiver receiver, BinaryReader data)
        : base(receiver)
    {
        Login = data.ReadString();
        Password = data.ReadString();
    }
    public override Task Execute()
    {
        if (_receiver == null)
            throw new InvalidOperationException($"{nameof(_receiver)} should not be null");
        return _receiver.UserLogin(this);
    }

    public override void Serialize(BinaryWriter data)
    {
        data.Write((byte)CommandKind.UserLogin);
        data.Write(Login);
        data.Write(Password);
    }
}