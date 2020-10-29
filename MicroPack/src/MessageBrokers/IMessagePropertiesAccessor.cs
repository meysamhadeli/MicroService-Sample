namespace MicroPack.MessageBrokers
{
    public interface IMessagePropertiesAccessor
    {
        IMessageProperties MessageProperties { get; set; }
    }
}