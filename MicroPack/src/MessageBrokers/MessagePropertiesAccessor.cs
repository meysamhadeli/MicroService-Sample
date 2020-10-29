using System.Threading;

namespace MicroPack.MessageBrokers
{
    public class MessagePropertiesAccessor : IMessagePropertiesAccessor
    {
        private static readonly AsyncLocal<MessageContextHolder>
            Holder = new AsyncLocal<MessageContextHolder>();

        public IMessageProperties MessageProperties
        {
            get => Holder.Value?.Properties;
            set
            {
                var holder = Holder.Value;
                if (holder != null)
                {
                    holder.Properties = null;
                }

                if (value != null)
                {
                    Holder.Value = new MessageContextHolder {Properties = value};
                }
            }
        }

        private class MessageContextHolder
        {
            public IMessageProperties Properties;
        }
    }
}