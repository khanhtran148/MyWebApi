using Microsoft.EntityFrameworkCore;
using MyWebApi.Application.Abstractions;
using MyWebApi.Domain.Constants;
using MyWebApi.Domain.Entities;

namespace MyWebApi.Infrastructure.Persistence.Contexts
{
    public partial class MyDbContext : DbContext, IMyDbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Language> Languages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("name=MyWeb");
                //optionsBuilder.UseNpgsql("name=MyWeb");
                optionsBuilder.UseLazyLoadingProxies();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(MyConstants.DefaultSchema);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyDbContext).Assembly);

            modelBuilder.UseIdentityByDefaultColumns();

            OnModelCreatingGeneratedProcedures(modelBuilder);
        }

        private void OnModelCreatingGeneratedProcedures(ModelBuilder modelBuilder)
        {
            // modelBuilder.Entity<GetConsultantsResult>().HasNoKey().ToView(null);
        }
    }
}
