using MediatR;
using Order.API.DTOs;

namespace Order.API.Commands;

public record CreateOrderCommand(
    Guid UserId,
    List<OrderItemRequest> Items,
    AddressRequest ShippingAddress,
    AddressRequest BillingAddress,
    string? Notes = null) : IRequest<OrderDto>;
