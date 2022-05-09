using System.Reflection;
using EasyNetQ;
using Micro.Abstractions;
using Micro.Attributes;
using Micro.Handlers;
using Micro.Messaging.RabbitMQ.Internals;
using Micro.Messaging.Subscribers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using IMessage = Micro.Abstractions.IMessage;

namespace Micro.Messaging.RabbitMQ;

internal sealed class RabbitMQMessageSubscriber : IMessageSubscriber
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessageTypeRegistry _messageTypeRegistry;
    private readonly IBus _bus;
    private readonly ILogger<RabbitMQMessageSubscriber> _logger;

    public RabbitMQMessageSubscriber(IServiceProvider serviceProvider, IMessageTypeRegistry messageTypeRegistry,
        IBus bus, ILogger<RabbitMQMessageSubscriber> logger)
    {
        _serviceProvider = serviceProvider;
        _messageTypeRegistry = messageTypeRegistry;
        _bus = bus;
        _logger = logger;
    }

    public IMessageSubscriber Command<T>() where T : class, ICommand
        => Message<T>((serviceProvider, command, cancellationToken) =>
            serviceProvider.GetRequiredService<IDispatcher>().SendAsync(command, cancellationToken));

    public IMessageSubscriber Event<T>() where T : class, IEvent
        => Message<T>((serviceProvider, @event, cancellationToken) =>
            serviceProvider.GetRequiredService<IDispatcher>().PublishAsync(@event, cancellationToken));

    public IMessageSubscriber Message<T>(Func<IServiceProvider, T, CancellationToken, Task> handler)
        where T : class, IMessage
    {
        _messageTypeRegistry.Register<T>();
        var messageAttribute = typeof(T).GetCustomAttribute<MessageAttribute>() ?? new MessageAttribute();

        _bus.PubSub.SubscribeAsync<T>(messageAttribute.SubscriptionId,
            async (message, cancellationToken) =>
            {
                try
                {
                    await handler(_serviceProvider, message, cancellationToken);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, exception.Message);
                    throw;
                }
            },
            configuration =>
            {
                if (!string.IsNullOrWhiteSpace(messageAttribute.Topic))
                {
                    configuration.WithTopic(messageAttribute.Topic);
                }
            });

        return this;
    }
}