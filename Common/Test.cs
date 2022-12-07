using CC.Common.Commands;
using Microsoft.Toolkit.HighPerformance;
using NUnit.Framework;
namespace CC.Common;

internal class MockReceiver : ICommandsReceiver
{
    public Task ChannelCreate(ChannelCreate command)
    {
        throw new NotImplementedException();
    }

    public Task ChannelJoin(ChannelJoin command)
    {
        throw new NotImplementedException();
    }

    public Task ChannelList(ChannelList command)
    {
        throw new NotImplementedException();
    }

    public Task ChannelRemove(ChannelRemove command)
    {
        throw new NotImplementedException();
    }

    public Task ChannelSendMessage(ChannelSendMessage command)
    {
        throw new NotImplementedException();
    }

    public Task ClientError(ClientError command)
    {
        throw new NotImplementedException();
    }

    public Task ClientReceiveMessage(ClientReceiveMessage command)
    {
        throw new NotImplementedException();
    }

    public Task ClientSuccess(ClientSuccess command)
    {
        throw new NotImplementedException();
    }

    public Task UserCreate(UserCreate command)
    {
        throw new NotImplementedException();
    }

    public Task UserLogin(UserLogin command)
    {
        throw new NotImplementedException();
    }
}
[TestFixture]
public class TestCommands
{
    const string channelName = "test";
    const string username = "toto";
    const string password = "password";
    const string message = "This is a test";
    ICommandsReceiver receiver = new MockReceiver();

    [Test]
    public void TestChannelCreate()
    {
        var command = new ChannelCreate(channelName);
        var data = AbstractCommand.SerializeCommand(command);
        var res = AbstractCommand.DeserializeCommand(receiver, data) as ChannelCreate;
        Assert.NotNull(res);
        Assert.AreEqual(channelName, res!.ChannelName);
    }
    [Test]
    public void TestChannelJoin()
    {
        var command = new ChannelJoin(channelName);
        var data = AbstractCommand.SerializeCommand(command);
        var res = AbstractCommand.DeserializeCommand(receiver, data) as ChannelJoin;
        Assert.NotNull(res);
        Assert.AreEqual(channelName, res!.ChannelName);
    }
    [Test]
    public void TestChannelList()
    {
        var command = new ChannelList();
        var data = AbstractCommand.SerializeCommand(command);
        var res = AbstractCommand.DeserializeCommand(receiver, data) as ChannelList;
        Assert.NotNull(res);
    }
    [Test]
    public void TestChannelRemove()
    {
        var chancreate = new ChannelRemove(channelName);
        var data = AbstractCommand.SerializeCommand(chancreate);
        var res = AbstractCommand.DeserializeCommand(receiver, data) as ChannelRemove;
        Assert.NotNull(res);
        Assert.AreEqual(channelName, res!.ChannelName);
    }
    [Test]
    public void TestChannelSendMessage()
    {
        var chancreate = new ChannelSendMessage(message);
        var data = AbstractCommand.SerializeCommand(chancreate);
        var res = AbstractCommand.DeserializeCommand(receiver, data) as ChannelSendMessage;
        Assert.NotNull(res);
        Assert.AreEqual(message, res!.Message);
    }
    [Test]
    public void TestClientError()
    {
        var chancreate = new ClientError(message);
        var data = AbstractCommand.SerializeCommand(chancreate);
        var res = AbstractCommand.DeserializeCommand(receiver, data) as ClientError;
        Assert.NotNull(res);
        Assert.AreEqual(message, res!.Message);
    }
    [Test]
    public void TestClientReceiveMessage()
    {
        var chancreate = new ClientReceiveMessage(username, message);
        var data = AbstractCommand.SerializeCommand(chancreate);
        var res = AbstractCommand.DeserializeCommand(receiver, data) as ClientReceiveMessage;
        Assert.NotNull(res);
        Assert.AreEqual(username, res!.Message.Sender);
        Assert.AreEqual(message, res!.Message.Message);
    }
    [Test]
    public void TestClientSuccess()
    {
        var chancreate = new ClientSuccess();
        var data = AbstractCommand.SerializeCommand(chancreate);
        var res = AbstractCommand.DeserializeCommand(receiver, data) as ClientSuccess;
        Assert.NotNull(res);
    }
    [Test]
    public void TestUserCreate()
    {
        var chancreate = new UserCreate(username, password);
        var data = AbstractCommand.SerializeCommand(chancreate);
        var res = AbstractCommand.DeserializeCommand(receiver, data) as UserCreate;
        Assert.NotNull(res);
        Assert.AreEqual(username, res!.Login);
        Assert.AreEqual(password, res!.Password);
    }
    [Test]
    public void TestUserLogin()
    {
        var chancreate = new UserLogin(username, password);
        var data = AbstractCommand.SerializeCommand(chancreate);
        var res = AbstractCommand.DeserializeCommand(receiver, data) as UserLogin;
        Assert.NotNull(res);
        Assert.AreEqual(username, res!.Login);
        Assert.AreEqual(password, res!.Password);
    }
}