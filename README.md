# Simple Chat
Create a client/server chat application in C#. You'll be creating two console applications, one for the client and one for the server.

## Technology requirements:
- C# .NET
- Any network communication protocol.
- Use any database you want (communicate asynchronously if able).
- Ability to connect multiple clients to a single server.
- You can use any libraries you need (just not a chat library).

## Communication:
- Implement an abstraction layer for communication implementing the command pattern (https://www.dofactory.com/net/command-design-pattern).
- Create custom serializers for commands.

## Features:
The client has a list of possible actions he can take by using the command line (CLI). Do not confuse these commands with communication commands.

## User stories:
- Create a user with an user and pass (e.g. /create-user -user=david -pass=asd123).
- Login using user and pass (e.g. /login -user=david -pass=asd123).
- Create channel (e.g. /create-channel myChannel) (channels will have an owner).
- Join channel (e.g. /join-channel myChannel).
- Remove channel (e.g. /delete-channel myChannel) (Only the owner can remove channels!).
- Send messages to their current channel (e.g. /send my message).
- Receive a list of channels upon request (e.g. /list-channels).

### Note:
- Upon entering a channel, the user will receive the messages from the channel and will continue to receive messages so long as he is in that channel.
- Create the required commands (communication) to communicate between the clients and the server.

### Bonus:
- Use MongoBD as DB to store models (or any document based db).
- Be able to limit the number of messages we can store per room (up to a configurable amount or by date).
- Implement unit tests for serialization/deserialization of commands.