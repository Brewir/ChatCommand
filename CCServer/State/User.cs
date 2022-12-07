using CC.Common;
using CC.Common.Commands;

namespace CC.Server;

public sealed class User : ICommandsReceiver, IObserver<ChatMessage>, IDisposable
{
    private readonly ClientSuccess _success;
    private readonly IUserList _userList;
    private readonly IChannelList _channelList;
    public ICommandsProducer.SendData Send { get; }
    public string? Username { get; private set; } = null;
    public Channel.SubscriptionHandle? _currentChannelSubscription = null;
    public User(IUserList userList, IChannelList channelList, ICommandsProducer.SendData sender)
    {
        _success = new ClientSuccess(this);
        _userList = userList;
        _channelList = channelList;
        Send = sender;
    }
    public async Task UserCreate(UserCreate command)
    {
        // TODO check forbidden characters
        // TODO check insanities
        await _userList.CreateNewAccount(command.Login, command.Password);
        await _success.Execute();
    }
    public async Task UserLogin(UserLogin command)
    {
        if (Username != null)
            throw new ChatMessageException("Already logged in");
        await _userList.CheckPassword(command.Login, command.Password);
        Username = command.Login;
        await _success.Execute();
    }

    public async Task ChannelCreate(ChannelCreate command)
    {
        LoggedIdGuard();
        // TODO check forbidden characters
        // TODO check insanities
        await _channelList.CreateChannel(this, command.ChannelName);
        await _success.Execute();
    }
    public async Task ChannelJoin(ChannelJoin command)
    {
        LoggedIdGuard();
        var channel = await _channelList.GetChannel(command.ChannelName);

        _currentChannelSubscription?.Dispose();
        _currentChannelSubscription = channel.Subscribe(this) as Channel.SubscriptionHandle;
        await _success.Execute();
    }
    public async Task ChannelRemove(ChannelRemove command)
    {
        LoggedIdGuard();
        await _channelList.RemoveChannel(Username!, command.ChannelName);
        await _success.Execute();
    }

    public async Task ChannelList(Common.Commands.ChannelList _)
    {
        LoggedIdGuard();
        var list = await _channelList.GetChannelList();
        foreach (var channel in list)
            await new ClientReceiveMessage(this, "Admin", channel).Execute();
        await _success.Execute();
    }

    public async Task ChannelSendMessage(ChannelSendMessage command)
    {
        LoggedIdGuard();
        if (_currentChannelSubscription == null)
            throw new ChatMessageException("must join a channel");
        await _currentChannelSubscription.Chan.SendMessageToChannel(new ChatMessage { Sender = Username!, Message = command.Message });
        await _success.Execute();
    }

    public Task ClientError(ClientError command)
    {
        return Send(command);
    }
    public Task ClientReceiveMessage(ClientReceiveMessage command)
    {
        return Send(command);
    }
    public Task ClientSuccess(ClientSuccess command)
    {
        return Send(command);
    }

    private void LoggedIdGuard()
    {
        if (Username == null)
            throw new ChatMessageException("Not logged in");
    }

    public void OnCompleted()
    {
        _currentChannelSubscription?.Dispose();
        _currentChannelSubscription = null;
    }

    public void OnError(Exception error)
    {
        // never called
        throw new NotImplementedException();
    }

    public void OnNext(ChatMessage value)
    {
        new ClientReceiveMessage(this, value.Sender, value.Message).Execute();
    }

    public void Dispose()
    {
        _currentChannelSubscription?.Dispose();
    }
}