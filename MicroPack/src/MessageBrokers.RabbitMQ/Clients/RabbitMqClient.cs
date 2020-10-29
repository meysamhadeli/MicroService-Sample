using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;


namespace MicroPack.MessageBrokers.RabbitMQ.Clients
{
    internal sealed class RabbitMqClient : IRabbitMqClient
    {
        private const string EmptyContext = "{}";
        private readonly IConnection _connection;
        private readonly IContextProvider _contextProvider;
        private readonly IRabbitMqSerializer _serializer;
        private readonly ILogger<RabbitMqClient> _logger;
        private readonly bool _contextEnabled;
        private readonly bool _loggerEnabled;
        private readonly string _spanContextHeader;

        public RabbitMqClient(IConnection connection, IContextProvider contextProvider, IRabbitMqSerializer serializer,
            RabbitMqOptions options, ILogger<RabbitMqClient> logger)
        {
            _connection = connection;
            _contextProvider = contextProvider;
            _serializer = serializer;
            _logger = logger;
            _contextEnabled = options.Context?.Enabled == true;
            _loggerEnabled = options.Logger?.Enabled ?? false;
            _spanContextHeader = options.GetSpanContextHeader();
        }

        public void Send(object message, IConventions conventions, string messageId = null, string correlationId = null,
            string spanContext = null, object messageContext = null, IDictionary<string, object> headers = null)
        {
            using var channel = _connection.CreateModel();
            var payload = _serializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(payload);
            var properties = channel.CreateBasicProperties();
            properties.MessageId = string.IsNullOrWhiteSpace(messageId)
                ? Guid.NewGuid().ToString("N")
                : messageId;
            properties.CorrelationId = string.IsNullOrWhiteSpace(correlationId)
                ? Guid.NewGuid().ToString("N")
                : correlationId;
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            properties.Headers = new Dictionary<string, object>();

            if (_contextEnabled)
            {
                IncludeMessageContext(messageContext, properties);
            }

            if (!string.IsNullOrWhiteSpace(spanContext))
            {
                properties.Headers.Add(_spanContextHeader, spanContext);
            }

            if (headers is {})
            {
                foreach (var header in headers)
                {
                    if (string.IsNullOrWhiteSpace(header.Key) || header.Value is null)
                    {
                        continue;
                    }

                    properties.Headers.Add(header.Key, header.Value);
                }
            }

            if (_loggerEnabled)
            {
                _logger.LogTrace($"Publishing a message with routing key: '{conventions.RoutingKey}' " +
                                 $"to exchange: '{conventions.Exchange}' " +
                                 $"[id: '{properties.MessageId}', correlation id: '{properties.CorrelationId}']");
            }

            channel.BasicPublish(conventions.Exchange, conventions.RoutingKey, properties, body);
        }

        private void IncludeMessageContext(object context, IBasicProperties properties)
        {
            if (context is {})
            {
                properties.Headers.Add(_contextProvider.HeaderName, _serializer.Serialize(context));
                return;
            }

            properties.Headers.Add(_contextProvider.HeaderName, EmptyContext);
        }
    }
}