using System.Threading.Tasks;
using System.Threading;
using CSharpFunctionalExtensions;
using Xunit;
using MyWebApi.Application.Languages.Queries;
using MyWebApi.Domain.Common;
using Tests.Kernel;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Application.UT.Tests.Languages.Queries
{
    public sealed class GetLanguagesByIdTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _databaseFixture;

        public GetLanguagesByIdTests(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
        }

        [Theory]
        [InlineData("vi")]
        [InlineData("pt")]
        [InlineData("pl")]
        public async Task Handle_FoundResult_ShouldSuccess(string id)
        {
            // Arrange
            GetById.GetByIdHandler handler = _databaseFixture.Mocker.CreateInstance<GetById.GetByIdHandler>();
            // Act
            Result<Select2ItemResponse> result = await handler.Handle(new GetById.Request(id), CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(result.Value.Id, id);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("zz")]
        public async Task Handle_NotFoundResult_ShouldFailed(string id)
        {
            // Arrange
            GetById.GetByIdHandler handler = _databaseFixture.Mocker.CreateInstance<GetById.GetByIdHandler>();

            // Act
            Result<Select2ItemResponse> result = await handler.Handle(new GetById.Request(id), CancellationToken.None);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Contains(result.Error, "No data found");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task ValidateAsync_BadId_ShouldInvalid(string id)
        {
            // Arrange
            GetById.RequestValidator validationRules = _databaseFixture.Mocker.CreateInstance<GetById.RequestValidator>();

            // Act
            ValidationResult validated = await validationRules.ValidateAsync(new GetById.Request(id), CancellationToken.None);

            // Assert
            Assert.True(!validated.IsValid);
        }

        [Theory]
        [InlineData("vi")]
        [InlineData("pt")]
        [InlineData("pl")]
        public async Task ValidateAsync_GoodId_ShouldValid(string id)
        {
            // Arrange
            GetById.RequestValidator validationRules = _databaseFixture.Mocker.CreateInstance<GetById.RequestValidator>();

            // Act
            ValidationResult validated = await validationRules.ValidateAsync(new GetById.Request(id), CancellationToken.None);

            // Assert
            Assert.True(validated.IsValid);
        }
    }
}
