using CC.Common;
using CC.Server;
using CC.Server.Services;

var builder = WebApplication.CreateBuilder(args);
ConfigureConfiguration(builder.Configuration);

ConfigureWebHost(builder.WebHost);

// Add services to the container.
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
ConfigureApplication(app, app.Services);
ConfigureEndpoints(app, app.Services);

await app.RunAsync();

void ConfigureConfiguration(ConfigurationManager config)
{
    // config.AddJsonFile("toto.json");
}
void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.Configure<Config>(configuration);
    services
        .AddSingleton<IChannelList, ChannelList>()
        .AddInitializableSingleton<IDatabase, Database>()
        .AddSingleton<IUserList, UserList>();
    services.AddGrpc();
}
void ConfigureWebHost(IWebHostBuilder webHost)
{
    // TODO configure listening
}
void ConfigureApplication(IApplicationBuilder app, IServiceProvider services)
{
    // TODO add metrics
}
void ConfigureEndpoints(IEndpointRouteBuilder app, IServiceProvider services)
{
    // TODO map metrics
    app.MapGrpcService<ChatCommandsService>();
    app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
}
