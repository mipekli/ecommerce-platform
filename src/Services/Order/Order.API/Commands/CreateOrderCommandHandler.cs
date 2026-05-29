using MediatR;
using Order.API.Interfaces;
using Order.API.ValueObjects;
using Order.API.DTOs;
using OrderEntity = Order.API.Entities.Order;

namespace Order.API.Commands;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;

    public CreateOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var shippingAddress = new Address(
            request.ShippingAddress.Street,
            request.ShippingAddress.City,
            request.ShippingAddress.State,
            request.ShippingAddress.ZipCode,
            request.ShippingAddress.Country);

        var billingAddress = new Address(
            request.BillingAddress.Street,
            request.BillingAddress.City,
            request.BillingAddress.State,
            request.BillingAddress.ZipCode,
            request.BillingAddress.Country);

        var order = new OrderEntity(request.UserId, shippingAddress, billingAddress);

        foreach (var item in request.Items)
        {
            order.AddItem(item.ProductId, item.ProductName, item.UnitPrice, item.Quantity);
        }

        if (!string.IsNullOrWhiteSpace(request.Notes))
        {
            order.UpdateNotes(request.Notes);
        }

        await _orderRepository.AddAsync(order, cancellationToken);

        return MapToDto(order);
    }

    private static OrderDto MapToDto(OrderEntity order)
    {
        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Status = order.Status.ToString(),
            SubTotal = order.SubTotal,
            TaxAmount = order.TaxAmount,
            ShippingCost = order.ShippingCost,
            TotalAmount = order.TotalAmount,
            Notes = order.Notes,
            CreatedAt = order.CreatedAt,
            ShippingAddress = new AddressDto
            {
                Street = order.ShippingAddress.Street,
                City = order.ShippingAddress.City,
                State = order.ShippingAddress.State,
                ZipCode = order.ShippingAddress.ZipCode,
                Country = order.ShippingAddress.Country
            },
            BillingAddress = new AddressDto
            {
                Street = order.BillingAddress.Street,
                City = order.BillingAddress.City,
                State = order.BillingAddress.State,
                ZipCode = order.BillingAddress.ZipCode,
                Country = order.BillingAddress.Country
            },
            Items = order.Items.Select(i => new OrderItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                TotalPrice = i.TotalPrice
            }).ToList()
        };
    }
}
