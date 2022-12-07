namespace CC.Common.Commands;

public interface ICommandsReceiver
{
    Task UserCreate(UserCreate command);
    Task UserLogin(UserLogin command);
    Task ChannelCreate(ChannelCreate command);
    Task ChannelRemove(ChannelRemove command);
    Task ChannelList(ChannelList command);
    Task ChannelJoin(ChannelJoin command);
    Task ChannelSendMessage(ChannelSendMessage command);
    Task ClientReceiveMessage(ClientReceiveMessage command);
    Task ClientError(ClientError command);
    Task ClientSuccess(ClientSuccess command);
}