using CC.Common;
using CC.Common.Commands;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.HighPerformance;
using Sharprompt;

namespace CC.Client;

public class ClientCommandReceiver : ICommandsReceiver, IAsyncDisposable
{
    private AsyncDuplexStreamingCall<Command, Command> _stream;
    private readonly ILogger<ClientCommandReceiver> _logger;
    private TaskCompletionSource? _pendingTask;

    public ClientCommandReceiver(ILogger<ClientCommandReceiver> logger, ChatCommands.ChatCommandsClient grpcClient)
    {
        _stream = grpcClient.Communicate();
        _logger = logger;
        var _ = HandleData();
    }
    private async Task HandleData()
    {
        _logger.LogDebug("Starting handling messages");
        await foreach (var command in _stream.ResponseStream.ReadAllAsync())
        {
            _logger.LogInformation("received message from server");
            using var reader = new BinaryReader(command.Data.Memory.AsStream());
            var com = AbstractCommand.DeserializeCommand(this, command.Data.Memory);
            _logger.LogTrace("received {command} from server", com);

            try
            {
                await com.Execute();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error execcuting command locally");
            }
        }
    }
    private Task SendToServer(IClientStreamWriter<Command> streamWriter, AbstractCommand request)
    {
        _logger.LogTrace("sending {command} to server", request);
        var command = AbstractCommand.SerializeProtobuf(request);
        return streamWriter.WriteAsync(command);
    }
    public async ValueTask DisposeAsync()
    {
        await _stream.RequestStream.CompleteAsync();
        _stream.Dispose();
    }

    public Task ClientError(ClientError command)
    {
        if (_pendingTask != null)
        {
            var tcs = Interlocked.Exchange(ref _pendingTask, null);
            tcs.SetException(new ChatMessageException(command.Message));
        }
        return Task.CompletedTask;
    }
    public Task ClientReceiveMessage(ClientReceiveMessage command)
    {
        Console.WriteLine($"[{command.Message.Sender}]: {command.Message.Message}");
        return Task.CompletedTask;
    }
    public Task ClientSuccess(ClientSuccess command)
    {
        if (_pendingTask != null)
        {
            var tcs = Interlocked.Exchange(ref _pendingTask, null);
            tcs.SetResult();
        }
        return Task.CompletedTask;
    }

    public Task ChannelCreate(ChannelCreate command)
    {
        Interlocked.Exchange(ref _pendingTask, new TaskCompletionSource());
        SendToServer(_stream.RequestStream, command);
        return _pendingTask.Task;
    }
    public Task ChannelJoin(ChannelJoin command)
    {
        Interlocked.Exchange(ref _pendingTask, new TaskCompletionSource());
        SendToServer(_stream.RequestStream, command);
        return _pendingTask.Task;
    }
    public Task ChannelList(ChannelList command)
    {
        Interlocked.Exchange(ref _pendingTask, new TaskCompletionSource());
        SendToServer(_stream.RequestStream, command);
        return _pendingTask.Task;
    }
    public Task ChannelRemove(ChannelRemove command)
    {
        Interlocked.Exchange(ref _pendingTask, new TaskCompletionSource());
        SendToServer(_stream.RequestStream, command);
        return _pendingTask.Task;
    }
    public Task ChannelSendMessage(ChannelSendMessage command)
    {
        Interlocked.Exchange(ref _pendingTask, new TaskCompletionSource());
        SendToServer(_stream.RequestStream, command);
        return _pendingTask.Task;
    }

    public Task UserCreate(UserCreate command)
    {
        Interlocked.Exchange(ref _pendingTask, new TaskCompletionSource());
        SendToServer(_stream.RequestStream, command);
        return _pendingTask.Task;
    }
    public Task UserLogin(UserLogin command)
    {
        Interlocked.Exchange(ref _pendingTask, new TaskCompletionSource());
        SendToServer(_stream.RequestStream, command);
        return _pendingTask.Task;
    }
}