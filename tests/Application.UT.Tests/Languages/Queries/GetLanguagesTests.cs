using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MyWebApi.Application.Languages.Queries;
using MyWebApi.Domain.Common;
using Tests.Kernel;
using Xunit;

namespace Application.UT.Tests.Languages.Queries
{
    public sealed class GetLanguagesTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _databaseFixture;

        public GetLanguagesTests(DatabaseFixture fixture)
        {
            _databaseFixture = fixture;
        }

        [Theory]
        [ClassData(typeof(GetLanguagesTestsBadData))]
        public async Task Handle_NotFoundCase_ShouldReturnNotFound(List<string> ids, string query, bool includeDisabled)
        {
            // Arrange
            GetLanguages.GetLanguagesHandler handler = _databaseFixture.Mocker.CreateInstance<GetLanguages.GetLanguagesHandler>();

            // Act
            Result<List<Select2ItemResponse>> result = await handler.Handle(new GetLanguages.Request()
            {
                Ids = ids,
                Query = query,
                IncludeDisabled = includeDisabled
            }, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Value.Count == 0);
        }

        [Theory]
        [ClassData(typeof(GetLanguagesTestsGoodData))]
        public async Task Handle_ValidFilter_ShouldSucceed(List<string> ids, string query, bool includeDisabled)
        {
            // Arrange
            GetLanguages.GetLanguagesHandler handler = _databaseFixture.Mocker.CreateInstance<GetLanguages.GetLanguagesHandler>();
            // Act
            Result<List<Select2ItemResponse>> result = await handler.Handle(new GetLanguages.Request()
            {
                Ids = ids,
                Query = query,
                IncludeDisabled = includeDisabled
            }, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Value.Any());
        }
    }
}
