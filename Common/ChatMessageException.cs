namespace CC.Common;

public class ChatMessageException : Exception
{
    public ChatMessageException(string? message)
        : base(message)
    {

    }
}