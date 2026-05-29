using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace BuildingBlocks.Shared.Messaging;

public class EventBusRabbitMQ : IEventBus, IDisposable
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ILogger<EventBusRabbitMQ> _logger;
    private readonly IConfiguration _configuration;
    private readonly Dictionary<string, Type> _eventTypes = [];
    private readonly Dictionary<string, object> _handlers = [];
    private IConnection? _connection;
    private IChannel? _channel;
    private readonly string _exchangeName;
    private readonly string _queueName;
    private readonly int _retryCount;
    private bool _disposed;

    public EventBusRabbitMQ(
        IConnectionFactory connectionFactory,
        ILogger<EventBusRabbitMQ> logger,
        IConfiguration configuration)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
        _configuration = configuration;
        _exchangeName = configuration.GetValue<string>("EventBus:ExchangeName") ?? "ecommerce_exchange";
        _queueName = configuration.GetValue<string>("EventBus:QueueName") ?? "ecommerce_queue";
        _retryCount = configuration.GetValue<int>("EventBus:RetryCount", 5);
    }

    private async Task EnsureConnectionAsync()
    {
        if (_connection is { IsOpen: true })
            return;

        var policy = Policy.Handle<BrokerUnreachableException>()
            .Or<IOException>()
            .WaitAndRetryAsync(
                _retryCount,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (ex, time) =>
                {
                    _logger.LogWarning(ex, "Could not connect to RabbitMQ after {TimeOut}s", $"{time.TotalSeconds:N1}");
                });

        _connection = await policy.ExecuteAsync(async () =>
        {
            var connection = await _connectionFactory.CreateConnectionAsync();
            return connection;
        });

        _channel = await _connection.CreateChannelAsync();
        await _channel.ExchangeDeclareAsync(_exchangeName, ExchangeType.Direct, durable: true);
        await _channel.QueueDeclareAsync(_queueName, durable: true, exclusive: false, autoDelete: false);
        await _channel.QueueBindAsync(_queueName, _exchangeName, _queueName);
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IntegrationEvent
    {
        await EnsureConnectionAsync();

        var policy = Policy.Handle<BrokerUnreachableException>()
            .Or<IOException>()
            .WaitAndRetryAsync(_retryCount, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        await policy.ExecuteAsync(async () =>
        {
            var body = JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType());
            var properties = new BasicProperties
            {
                Persistent = true,
                Type = @event.EventType
            };

            if (_channel is not null)
            {
                await _channel.BasicPublishAsync(
                    exchange: _exchangeName,
                    routingKey: _queueName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body,
                    cancellationToken: cancellationToken);
            }
        });
    }

    public async Task SubscribeAsync<T, THandler>()
        where T : IntegrationEvent
        where THandler : IIntegrationEventHandler<T>
    {
        await EnsureConnectionAsync();

        var eventName = typeof(T).Name;
        _eventTypes[eventName] = typeof(T);

        if (_channel is not null)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var eventNameReceived = ea.BasicProperties.Type ?? string.Empty;
                var body = ea.Body.ToArray();

                if (_eventTypes.TryGetValue(eventNameReceived, out var eventType))
                {
                    var integrationEvent = JsonSerializer.Deserialize(body, eventType) as T;
                    if (integrationEvent is not null && _handlers.TryGetValue(eventNameReceived, out var handlerObj))
                    {
                        try
                        {
                            if (handlerObj is IIntegrationEventHandler<T> handler)
                            {
                                await handler.HandleAsync(integrationEvent);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error handling event {EventName}", eventNameReceived);
                        }
                    }
                }

                if (_channel is not null)
                {
                    await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };

            await _channel.BasicConsumeAsync(_queueName, autoAck: false, consumer: consumer);
        }
    }

    public void RegisterHandler<T>(IIntegrationEventHandler<T> handler) where T : IntegrationEvent
    {
        var eventName = typeof(T).Name;
        _handlers[eventName] = handler;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _channel?.CloseAsync();
        _connection?.CloseAsync();
    }
}
