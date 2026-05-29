using MassTransit;
using Order.API.IntegrationEvents;
using BuildingBlocks.Shared.Messaging.SagaMessages;

namespace Order.API.Sagas;

public class OrderSagaStateMachine : MassTransitStateMachine<OrderSagaState>
{
    public State Pending { get; set; } = null!;
    public State StockReserving { get; set; } = null!;
    public State Completed { get; set; } = null!;
    public State Failed { get; set; } = null!;

    public Event<OrderCreatedIntegrationEvent> OrderCreated { get; set; } = null!;
    public Event<StockReservedEvent> StockReserved { get; set; } = null!;
    public Event<StockReservationFailedEvent> StockReservationFailed { get; set; } = null!;

    public OrderSagaStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderCreated, e => e.CorrelateById(context => context.Message.OrderId));
        Event(() => StockReserved, e => e.CorrelateById(context => context.Message.OrderId));
        Event(() => StockReservationFailed, e => e.CorrelateById(context => context.Message.OrderId));

        Initially(
            When(OrderCreated)
                .Then(context =>
                {
                    context.Saga.OrderId = context.Message.OrderId;
                    context.Saga.UserId = context.Message.UserId;
                    context.Saga.OrderNumber = context.Message.OrderNumber;
                    context.Saga.CreatedAt = DateTime.UtcNow;
                })
                .Send(context => new Uri("queue:reserve-stock"), context => new ReserveStockCommand
                {
                    OrderId = context.Message.OrderId,
                    Items = context.Message.Items.Select(i => new StockItemRequest
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity
                    }).ToList()
                })
                .TransitionTo(StockReserving)
        );

        During(StockReserving,
            When(StockReserved)
                .Then(context =>
                {
                    Console.WriteLine($"Stock reserved for order {context.Saga.OrderNumber}");
                })
                .TransitionTo(Completed)
                .Finalize(),
            When(StockReservationFailed)
                .Then(context =>
                {
                    context.Saga.FailureReason = context.Message.Reason;
                    Console.WriteLine($"Stock reservation failed for order {context.Saga.OrderNumber}: {context.Message.Reason}");
                })
                .Send(context => new Uri("queue:cancel-order"), context => new CancelOrderSagaCommand(context.Saga.OrderId))
                .TransitionTo(Failed)
                .Finalize()
        );

        SetCompletedWhenFinalized();
    }
}
