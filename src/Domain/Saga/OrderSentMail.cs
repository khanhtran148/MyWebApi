using MyWebApi.Messages.Orders;

namespace MyWebApi.Domain.Saga;

public class OrderSentMail : IOrderSentMail
{
    public int OrderId { get; set; }
}
