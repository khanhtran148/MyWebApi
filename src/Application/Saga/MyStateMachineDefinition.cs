using MassTransit;
using MyWebApi.Domain.Entities.Saga;

namespace MyWebApi.Application.Saga;

public class MyStateMachineDefinition : SagaDefinition<Monitoring>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<Monitoring> sagaConfigurator,
        IRegistrationContext context)
    {
        endpointConfigurator.UseInMemoryOutbox(context);
    }
}
