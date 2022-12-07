namespace CC.Common;
public class Config
{
    static readonly Config _instance = new();
    public static Config Instance { get => _instance; }
}
