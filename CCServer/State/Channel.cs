using System.Collections.Concurrent;
using CC.Common;

namespace CC.Server;

public sealed class Channel : IObservable<ChatMessage>, IDisposable
{
    public string Name { get; }
    public string Owner { get; }
    private readonly IDatabase _database;
    private List<ChatMessage> _messages = new();
    internal ConcurrentDictionary<string, IObserver<ChatMessage>> _observers = new();
    public sealed class SubscriptionHandle : IDisposable
    {
        public readonly Channel Chan;
        private readonly string _key;
        public SubscriptionHandle(Channel subscribeHolder, string key)
        {
            Chan = subscribeHolder;
            _key = key;
        }
        public void Dispose()
        {
            Chan._observers.TryRemove(_key, out var _);
        }
    }
    public Channel(IDatabase database, string name, string owner)
    {
        _database = database;
        Name = name;
        Owner = owner;
    }
    public async Task Initialize()
    {
        _messages = await _database.GetMessagesInChannel(Name);
    }
    public IDisposable Subscribe(IObserver<ChatMessage> observer)
    {
        if (observer is not User user)
            throw new ArgumentException("only users may subscribe to channel", nameof(observer));
        // already checked loggedin before
        _observers[user.Username!] = observer;

        foreach (var message in _messages)
        {
            observer.OnNext(message);
        }
        return new SubscriptionHandle(this, user.Username!);
    }
    public async Task SendMessageToChannel(ChatMessage message)
    {
        _messages.Add(message);
        foreach (var observer in _observers.Values)
        {
            observer.OnNext(message);
        }
        await _database.InsertMessage(Name, message);
    }
    public void Dispose()
    {
        foreach (var observer in _observers.Values)
        {
            observer.OnCompleted();
        }
    }
}