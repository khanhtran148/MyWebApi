using MyWebApi.Messages.Orders;

namespace MyWebApi.Domain.Saga;

public class MailSent : IMailSent
{
    public int OrderId { get; set; }
}
