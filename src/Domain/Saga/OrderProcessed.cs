using MyWebApi.Messages.Orders;

namespace MyWebApi.Domain.Saga;

public class OrderProcessed : IOrderProcessed
{
    public int OrderId { get; set; }
}
