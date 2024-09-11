using MyWebApi.Messages.Orders;

namespace MyWebApi.Domain.Saga;

public class SendMail : ISendMail
{
    public int OrderId { get; set; }
}
