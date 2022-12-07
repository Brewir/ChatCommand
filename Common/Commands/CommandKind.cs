namespace CC.Common.Commands;

public enum CommandKind : byte
{
    Unknown = 0,
    UserCreate = 1,
    UserLogin = 2,
    ChannelCreate = 10,
    ChannelRemove = 11,
    ChannelList = 12,
    ChannelJoin = 13,
    ChannelSendMessage = 14,
    ClientReceiveMessage = 20,
    ClientError = 21,
    ClientSuccess = 22,
}