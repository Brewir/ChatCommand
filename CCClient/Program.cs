using CC.Client;
using CC.Common;
using CC.Common.Commands;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sharprompt;

// var builder = WebApplication.CreateBuilder(args);
var services = new ServiceCollection();
ConfigureServices(services);
var provider = services.BuildServiceProvider();
var commandReceiver = provider.GetRequiredService<ICommandsReceiver>();
try
{
    Prompt.ThrowExceptionOnCancel = true;
    while (true)
    {
        await Task.Run(() => RunCommand(commandReceiver));
    }
}
catch (PromptCanceledException)
{
    if (commandReceiver is IAsyncDisposable disp)
        await disp.DisposeAsync();
    Console.WriteLine("Thanks for chatting with us");
}

static async Task RunCommand(ICommandsReceiver commandReceiver)
{
    var request = CommandLineInterface.CommandChoice();
    var command = CommandLineInterface.CreateCommand(request);
    command.SetReceiver(commandReceiver);
    try
    {
        await command.Execute();
    }
    catch (ChatMessageException e)
    {
        Console.WriteLine("Error:");
        Console.WriteLine(e.Message);
    }
}

static void ConfigureServices(IServiceCollection services)
{
    services.AddLogging();
    services.AddSingleton<ICommandsReceiver, ClientCommandReceiver>();
    services.AddGrpcClient<ChatCommands.ChatCommandsClient>(o =>
        {
            // TODO true config
            o.Address = new Uri($"http://localhost:5129");
        })
        .ConfigureChannel(o =>
        {
            // TODO secure channel
            o.Credentials = ChannelCredentials.Insecure;
        });
}
