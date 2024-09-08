using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using MassTransit;
using MyWebApi.Messages.Job;

namespace MyWebApi.Application.Consumer;

[ExcludeFromCodeCoverage]
public sealed class OnJobCreatedConsumer : IConsumer<JobCreated>
{
    public async Task Consume(ConsumeContext<JobCreated> context)
    {
        Console.WriteLine($"Processing JobId {context.Message.JobId}");
        await Task.Delay(TimeSpan.FromSeconds(10));
        Console.WriteLine($"Job just done JobId {context.Message.JobId}");
    }
}
