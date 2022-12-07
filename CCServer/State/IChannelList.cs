namespace CC.Server;

public interface IChannelList
{
    Task CreateChannel(User user, string channelName);
    Task<Channel> GetChannel(string channelName);
    Task<List<string>> GetChannelList();
    Task RemoveChannel(string user, string channelName);
}
