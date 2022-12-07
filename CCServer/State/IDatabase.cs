using CC.Common;

namespace CC.Server;

public interface IDatabase
{
    Task<bool> TryAddAccount(string username, string password);
    Task<string?> TryGetPassword(string username);
    Task<bool> TryCreateChannel(string owner, string channelName);
    Task<string?> TryGetChannel(string channelName);
    Task<List<string>> GetChannelList();
    Task<bool> RemoveChannel(string channelName);
    Task InsertMessage(string channelName, ChatMessage message);
    Task<List<ChatMessage>> GetMessagesInChannel(string channelName);
}
