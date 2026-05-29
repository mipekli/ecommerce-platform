using MediatR;
using Order.Application.DTOs;

namespace Order.Application.Commands;

public record CreateOrderCommand(
    Guid UserId,
    List<OrderItemRequest> Items,
    AddressRequest ShippingAddress,
    AddressRequest BillingAddress,
    string? Notes = null) : IRequest<OrderDto>;
