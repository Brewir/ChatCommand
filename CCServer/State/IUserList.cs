using CC.Common.Commands;

namespace CC.Server;

public interface IUserList
{
    Task CreateNewAccount(string name, string password);
    Task CheckPassword(string name, string password);
}
