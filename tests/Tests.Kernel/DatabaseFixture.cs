using System;
using System.Collections.Generic;
using Bogus;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Moq.AutoMock;
using MyWebApi.Application;
using MyWebApi.Domain.Entities;
using MyWebApi.Infrastructure.Persistence.Contexts;

namespace Tests.Kernel;

public class DatabaseFixture : IDisposable
{
    public AutoMocker Mocker { get; private set; }
    public Randomizer Randomizer { get; private set; }
    public IMapper Mapper { get; private set; }
    public MyDbContext ProductivityContext { get; private set; }

    public DatabaseFixture()
    {
        Randomizer = new Randomizer();
        Mocker = new AutoMocker();
        Mapper = new Mapper();
        Mapper.Config.Scan(
            typeof(ApplicationDependencyInjection).Assembly,
            typeof(MyWebApi.Infrastructure.InfrastructureDependencyInjection).Assembly);

        DbContextOptions<MyDbContext> contextOptions = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(Randomizer.Guid().ToString())
            .Options;
        ProductivityContext = new MyDbContext(contextOptions);

        InitializeMasterData();

        Mocker.Use(ProductivityContext);
        Mocker.Use(Mapper);
    }

    private void InitializeMasterData()
    {
        var languages = new List<Language>
        {
            new Language()
            {
                Id = "en",
                Label = "English"
            },
            new Language()
            {
                Id = "vi",
                Label = "Vietnamese"
            },
            new Language()
            {
                Id = "pt",
                Label = "Portuguese"
            },
            new Language()
            {
                Id = "pl",
                Label = "Polish"
            }
        };

        ProductivityContext.Languages.AddRange(languages);
        ProductivityContext.SaveChanges();
    }
    public void Dispose()
    {
        ProductivityContext.Dispose();
    }
}
