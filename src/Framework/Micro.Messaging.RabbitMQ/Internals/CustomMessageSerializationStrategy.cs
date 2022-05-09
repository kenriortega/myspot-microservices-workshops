using System.Collections.Concurrent;
using System.Text;
using EasyNetQ;
using Micro.Contexts;
using Micro.Contexts.Accessors;

namespace Micro.Messaging.RabbitMQ.Internals;

internal sealed class CustomMessageSerializationStrategy : IMessageSerializationStrategy
{
    private const string ActivityIdKey = "activity-id";
    private const string CausationIdKey = "causation-id";
    private const string TraceIdKey = "trace-id";
    private const string UserIdKey = "user-id";
    private const string SpanKey = "span";
    
    private readonly ConcurrentDictionary<Type, string> _typeNames = new();
    private readonly IMessageTypeRegistry _messageTypeRegistry;
    private readonly ISerializer _serializer;
    private readonly IMessageContextAccessor _messageContextAccessor;
    private readonly IContextAccessor _contextAccessor;

    public CustomMessageSerializationStrategy(IMessageTypeRegistry messageTypeRegistry, ISerializer serializer,
        IMessageContextAccessor messageContextAccessor, IContextAccessor contextAccessor)
    {
        _messageTypeRegistry = messageTypeRegistry;
        _serializer = serializer;
        _messageContextAccessor = messageContextAccessor;
        _contextAccessor = contextAccessor;
    }

    public SerializedMessage SerializeMessage(IMessage message)
    {
        var messageContext = _messageContextAccessor.MessageContext;
        var messageBody = _serializer.MessageToBytes(message.MessageType, message.GetBody());
        var messageProperties = message.Properties;
        messageProperties.Type = _typeNames.GetOrAdd(message.MessageType, message.MessageType.Name.ToMessageKey());

        if (!string.IsNullOrWhiteSpace(messageContext?.MessageId))
        {
            messageProperties.MessageId = messageContext.MessageId;
        }

        if (!string.IsNullOrWhiteSpace(messageContext?.Context.CorrelationId))
        {
            messageProperties.CorrelationId = messageContext.Context.CorrelationId;
        }

        if (!string.IsNullOrWhiteSpace(messageContext?.Context.UserId))
        {
            messageProperties.Headers.TryAdd(UserIdKey, messageContext.Context.UserId);
        }

        if (!string.IsNullOrWhiteSpace(messageContext?.Context.ActivityId))
        {
            messageProperties.Headers.TryAdd(ActivityIdKey, messageContext.Context.ActivityId);
        }
        
        // Access the context for the external message (parent) that might be currently being handled
        if (!string.IsNullOrWhiteSpace(_contextAccessor.Context?.MessageId))
        {
            messageProperties.Headers.TryAdd(CausationIdKey, _contextAccessor.Context.MessageId);
        }

        if (!string.IsNullOrWhiteSpace(messageContext?.Context.TraceId))
        {
            messageProperties.Headers.TryAdd(TraceIdKey, messageContext.Context.TraceId);
        }
        
        if (!string.IsNullOrWhiteSpace(messageContext?.Context.Span))
        {
            messageProperties.Headers.TryAdd(SpanKey, messageContext.Context.Span);
        }

        return new SerializedMessage(messageProperties, messageBody);
    }

    public IMessage DeserializeMessage(MessageProperties properties, in ReadOnlyMemory<byte> body)
    {
        var type = _messageTypeRegistry.Resolve(properties.Type);
        if (type is null)
        {
            throw new Exception($"Message was not registered for type: '{properties.Type}'.");
        }

        var activityId = GetHeaderValue(properties, ActivityIdKey);
        var causationId = GetHeaderValue(properties, CausationIdKey);
        var traceId = GetHeaderValue(properties, TraceIdKey);
        var userId = GetHeaderValue(properties, UserIdKey);
        var span = GetHeaderValue(properties, SpanKey);
        
        _contextAccessor.Context = new Context(traceId, properties.CorrelationId,
            properties.MessageId, causationId, activityId, userId, span);

        var messageBody = _serializer.BytesToMessage(type, body);
        return MessageFactory.CreateInstance(type, messageBody, properties);
    }

    private static string GetHeaderValue(MessageProperties properties, string key)
        => properties.Headers.TryGetValue(key, out var bytes)
            ? Encoding.UTF8.GetString(bytes as byte[] ?? Array.Empty<byte>())
            : string.Empty;
}