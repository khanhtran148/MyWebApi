namespace MyWebApi.Domain.Configurations;

public sealed class MessageBrokerOptions
{
    public static readonly string BindLocator = "MessageBrokers";
    public string Host { get; set; }
    public int Port { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
    public string VirtualHost { get; set; }
}
