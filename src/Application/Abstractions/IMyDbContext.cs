using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyWebApi.Domain.Entities;

namespace MyWebApi.Application.Abstractions;

public interface IMyDbContext
{
    DbSet<Language> Languages { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
