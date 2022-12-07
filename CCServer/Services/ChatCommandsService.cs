using CC.Common;
using CC.Common.Commands;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Toolkit.HighPerformance;

namespace CC.Server.Services;

public class ChatCommandsService : ChatCommands.ChatCommandsBase
{
    private readonly ILogger<ChatCommandsService> _logger;
    private readonly IUserList _userList;
    private readonly IChannelList _channelList;

    public ChatCommandsService(ILogger<ChatCommandsService> logger, IUserList userList, IChannelList channelList)
    {
        _logger = logger;
        _userList = userList;
        _channelList = channelList;
    }

    public override async Task Communicate(IAsyncStreamReader<Command> requestStream, IServerStreamWriter<Command> responseStream, ServerCallContext context)
    {
        var connectionId = Guid.NewGuid();
        var user = new User(_userList, _channelList, command => DataSender(responseStream, connectionId, command));
        var commandsTask = HandleCommands(requestStream, connectionId, user);

        try
        {
            await Task.WhenAny(commandsTask, AwaitCompletion(context.CancellationToken));
        }
        finally
        {
            user.Dispose();
        }
    }
    private Task DataSender(IServerStreamWriter<Command> responseStream, Guid connectionId, AbstractCommand response)
    {
        _logger.LogInformation("sending {command} to {connection}", response, connectionId.ToString("D"));
        var command = AbstractCommand.SerializeProtobuf(response);
        return responseStream.WriteAsync(command);
    }
    private async Task HandleCommands(IAsyncStreamReader<Command> requestStream, Guid connectionId, ICommandsReceiver receiver)
    {
        await foreach (var command in requestStream.ReadAllAsync())
        {
            _logger.LogInformation("received a message by {connection}", connectionId.ToString("D"));
            var com = AbstractCommand.DeserializeCommand(receiver, command.Data.Memory);
            _logger.LogInformation("received {command} by {connection}", com, connectionId.ToString("D"));
            try
            {
                await com.Execute();
            }
            catch (Exception e)
            {
                await TranslateError(e, receiver);
            }
        }
    }
    private static Task AwaitCompletion(CancellationToken token)
    {
        var tcs = new TaskCompletionSource();
        token.Register(() => tcs.SetResult());
        return tcs.Task;
    }
    private Task TranslateError(Exception e, ICommandsReceiver receiver)
    {
        ClientError errorCommand;
        if (e is ChatMessageException cme)
        {
            _logger.LogDebug(cme, "Invalid message sent");
            errorCommand = new ClientError(receiver, cme.Message);
        }
        else
        {
            _logger.LogError(e, "Error in the server");
            errorCommand = new ClientError(receiver, $"server error: {e}");
        }
        return errorCommand.Execute();
    }
}
