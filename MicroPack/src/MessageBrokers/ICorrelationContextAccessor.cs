namespace MicroPack.MessageBrokers
{
    public interface ICorrelationContextAccessor
    {
        object CorrelationContext { get; set; }
    }
}