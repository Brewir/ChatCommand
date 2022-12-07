namespace CC.Common.Commands;

public interface ICommandsProducer
{
    public delegate Task SendData(AbstractCommand command);
}