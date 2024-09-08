using MyWebApi.Messages.Orders;

namespace MyWebApi.Domain.Saga;

public class OrderSubmitted : IOrderSubmitted
{
    public int OrderId { get; set; }
}
