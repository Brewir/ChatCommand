using CC.Common;
using MongoDB.Driver;


namespace CC.Server;

public class Database : IDatabase, IAsyncInitializable
{
    private class MongoAccount
    {
        public string username = string.Empty;
        public string password = string.Empty;
        public DateTime created_at = DateTime.UtcNow;
    }
    private class MongoChannel
    {
        public string name = string.Empty;
        public string owner = string.Empty;
        public DateTime created_at = DateTime.UtcNow;
    }
    private class MongoMessage
    {
        public string channel = string.Empty;
        public ChatMessage message = new();
        public DateTime posted_at = DateTime.UtcNow;
    }
    private readonly IMongoClient _client = new MongoClient("mongodb://localhost:27017");
    private readonly IMongoDatabase _db;
    private readonly IMongoCollection<MongoAccount> _accounts;
    private readonly IMongoCollection<MongoChannel> _channels;
    private readonly IMongoCollection<MongoMessage> _messages;
    public Database()
    {
        _db = _client.GetDatabase("db");
        _accounts = _db.GetCollection<MongoAccount>("account");
        _channels = _db.GetCollection<MongoChannel>("channel");
        _messages = _db.GetCollection<MongoMessage>("message");
    }
    public async Task InitializeAsync(CancellationToken token)
    {
        var indexOptions = new CreateIndexOptions { Unique = true };

        var indexKeysAcc = Builders<MongoAccount>.IndexKeys.Ascending(f => f.username);
        var indexModelAcc = new CreateIndexModel<MongoAccount>(indexKeysAcc, indexOptions);
        await _accounts.Indexes.CreateOneAsync(indexModelAcc, cancellationToken: token);

        var indexKeysChan = Builders<MongoChannel>.IndexKeys.Ascending(f => f.name);
        var indexModelChan = new CreateIndexModel<MongoChannel>(indexKeysChan, indexOptions);
        await _channels.Indexes.CreateOneAsync(indexModelChan, cancellationToken: token);

        var indexKeysMess = Builders<MongoMessage>.IndexKeys.Ascending(f => f.channel);
        var indexModelMess = new CreateIndexModel<MongoMessage>(indexKeysMess);
        await _messages.Indexes.CreateOneAsync(indexModelMess, cancellationToken: token);
    }

    public async Task<List<string>> GetChannelList()
    {
        var list = await _channels.Find(_ => true).ToListAsync();
        return list.Select(ch => ch.name).ToList();
    }

    public async Task<List<ChatMessage>> GetMessagesInChannel(string channelName)
    {
        var list = await _messages.Find(m => m.channel == channelName).ToListAsync();
        return list.Select(m => m.message).ToList();
    }

    public Task InsertMessage(string channelName, ChatMessage message)
    {
        return _messages.InsertOneAsync(new MongoMessage { channel = channelName, message = message });
    }

    public async Task<bool> RemoveChannel(string channelName)
    {
        await _messages.DeleteManyAsync(m => m.channel == channelName);
        var res = await _channels.DeleteOneAsync(c => c.name == channelName);
        return res.DeletedCount == 1;
    }

    public async Task<bool> TryAddAccount(string username, string password)
    {
        try
        {
            await _accounts.InsertOneAsync(new MongoAccount { username = username, password = password });
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> TryCreateChannel(string owner, string channelName)
    {
        try
        {
            await _channels.InsertOneAsync(new MongoChannel { name = channelName, owner = owner });
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<string?> TryGetChannel(string channelName)
    {
        var chan = await _channels.Find(c => c.name == channelName).FirstOrDefaultAsync();
        return chan?.owner;
    }

    public async Task<string?> TryGetPassword(string username)
    {
        var acc = await _accounts.Find(a => a.username == username).FirstOrDefaultAsync();
        return acc?.username;
    }
}