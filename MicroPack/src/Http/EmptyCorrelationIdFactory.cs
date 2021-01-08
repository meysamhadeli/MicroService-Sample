namespace MicroPack.Http
{
    internal class EmptyCorrelationIdFactory : ICorrelationIdFactory
    {
        public string Create() => default;
    }
}