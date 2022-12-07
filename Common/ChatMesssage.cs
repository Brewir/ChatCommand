namespace CC.Common;

public class ChatMessage
{
    public string Sender = "";
    public string Message = "";
    public void Serialize(BinaryWriter data)
    {
        data.Write(Sender);
        data.Write(Message);
    }
    public static ChatMessage Deserialize(BinaryReader data)
    {
        return new ChatMessage
        {
            Sender = data.ReadString(),
            Message = data.ReadString(),
        };
    }
}