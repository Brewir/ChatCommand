using System.ComponentModel.DataAnnotations;
using CC.Common.Commands;
using Sharprompt;


namespace CC.Client;

public class CommandLineInterface
{
    private static readonly CommandKind[] Available = new[]
    {
        CommandKind.ChannelSendMessage,
        CommandKind.UserCreate,
        CommandKind.UserLogin,
        CommandKind.ChannelCreate,
        CommandKind.ChannelJoin,
        CommandKind.ChannelRemove,
        CommandKind.ChannelList,
    };
    public static CommandKind CommandChoice()
    {
        return Prompt.Select("What do you want ?", items: Available, textSelector: kind => kind.ToString());
    }
#nullable disable
    private class UsernamePassword
    {
        [Display(Name = "Enter your username")]
        [Required]
        public string Username { get; set; }

        [Display(Name = "Enter password")]
        [DataType(DataType.Password)]
        [Required]
        [MinLength(8)]
        public string Password { get; set; }
    }
#nullable restore
    public static AbstractCommand CreateCommand(CommandKind kind)
    {
        switch (kind)
        {
            case CommandKind.UserCreate:
                {
                    var user = Prompt.Bind<UsernamePassword>();
                    var confPassword = Prompt.Password("Confirm password", validators: new[] { Validators.Required(), Validators.RegularExpression(user.Password) });
                    return new UserCreate(user.Username, user.Password);
                }
            case CommandKind.UserLogin:
                {
                    var user = Prompt.Bind<UsernamePassword>();
                    return new UserLogin(user.Username, user.Password);
                }
            case CommandKind.ChannelCreate:
                {
                    var channel = Prompt.Input<string>("Channel name");
                    return new ChannelCreate(channel);
                }
            case CommandKind.ChannelJoin:
                {
                    var channel = Prompt.Input<string>("Channel name");
                    return new ChannelJoin(channel);
                }
            case CommandKind.ChannelRemove:
                {
                    var channel = Prompt.Input<string>("Channel name");
                    return new ChannelRemove(channel);
                }
            case CommandKind.ChannelSendMessage:
                {
                    var message = Prompt.Input<string>("message");
                    return new ChannelSendMessage(message);
                }
            case CommandKind.ChannelList:
                {
                    return new ChannelList();
                }
            default:
                // disallowed command
                throw new NotImplementedException();
        }
    }
}