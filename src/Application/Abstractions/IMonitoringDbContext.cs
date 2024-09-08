using Microsoft.EntityFrameworkCore;
using MyWebApi.Domain.Entities.Saga;

namespace MyWebApi.Application.Abstractions;

public interface IMonitoringDbContext
{
    DbSet<Monitoring> Monitorings { get; set; }
}
