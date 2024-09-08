using System.Collections.Generic;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using MyWebApi.Application.Abstractions;
using MyWebApi.Domain.Entities.Saga;
using MyWebApi.Infrastructure.Persistence.Mappings.Saga;

public partial class MonitoringDbContext : SagaDbContext, IMonitoringDbContext
{
    public MonitoringDbContext(DbContextOptions options) : base(options)
    {
    }

    public virtual DbSet<Monitoring> Monitorings { get; set; }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new MonitoringConfiguration(); }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("name=MyWeb");
        }
    }
}
