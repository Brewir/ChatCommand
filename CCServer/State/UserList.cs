using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using CC.Common;

namespace CC.Server;

public class UserList : IUserList
{
    private readonly ConcurrentDictionary<string, string> _accounts = new();
    private readonly ILogger<UserList> _logger;
    private readonly IDatabase _database;
    private readonly SHA256 _hasher;

    public UserList(ILogger<UserList> logger, IDatabase database)
    {
        _logger = logger;
        _database = database;
        _hasher = SHA256.Create();
    }

    public async Task CheckPassword(string name, string password)
    {
        var hpass = Convert.ToHexString(_hasher.ComputeHash(Encoding.UTF8.GetBytes(password)));

        if (!_accounts.TryGetValue(name, out var truePass))
        {
            truePass = await _database.TryGetPassword(name);
            if (truePass == null)
                throw new ChatMessageException($"Invalid account or password");
            truePass = _accounts.GetOrAdd(name, truePass);
        }

        if (truePass != hpass)
            throw new ChatMessageException($"Invalid account or password");
    }

    public async Task CreateNewAccount(string name, string password)
    {
        // todo check db
        var hpass = _hasher.ComputeHash(Encoding.UTF8.GetBytes(password));
        if (!await _database.TryAddAccount(name, Convert.ToHexString(hpass)))
            throw new ChatMessageException($"Account already exists: {name}");
    }
}