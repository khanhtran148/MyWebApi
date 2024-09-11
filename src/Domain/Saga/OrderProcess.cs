using MyWebApi.Messages.Orders;

namespace MyWebApi.Domain.Saga;

public class OrderProcess : IOrderProcess
{
    public int OrderId { get; set; }
}
