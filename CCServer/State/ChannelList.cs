using System.Collections.Concurrent;
using CC.Common;

namespace CC.Server;

public class ChannelList : IChannelList
{
    private readonly ConcurrentDictionary<string, Channel> _list = new();
    private readonly ILogger<ChannelList> _logger;
    private readonly IDatabase _database;

    public ChannelList(ILogger<ChannelList> logger, IDatabase database)
    {
        _logger = logger;
        _database = database;
    }
    public async Task CreateChannel(User user, string channelName)
    {
        if (!await _database.TryCreateChannel(user.Username!, channelName))
            throw new ChatMessageException("Channel already exists");
        var chan = new Channel(_database, channelName, user.Username!);
        _list.GetOrAdd(channelName, chan);
    }

    public async Task<Channel> GetChannel(string channelName)
    {
        if (!_list.TryGetValue(channelName, out var chan))
        {
            var owner = await _database.TryGetChannel(channelName);
            if (owner == null)
                throw new ChatMessageException("Unknown channel");
            chan = new Channel(_database, channelName, owner);
            await chan.Initialize();
            chan = _list.GetOrAdd(channelName, chan);
        }

        return chan;
    }

    public Task<List<string>> GetChannelList()
    {
        return _database.GetChannelList();
    }

    public async Task RemoveChannel(string user, string channelName)
    {
        var owner = await _database.TryGetChannel(channelName);
        if (owner == null)
            return;
        if (owner != user)
            throw new ChatMessageException("only owner can remove channel");
        await _database.RemoveChannel(channelName);
        _list.TryRemove(channelName, out var chan);
        chan?.Dispose();
    }
}