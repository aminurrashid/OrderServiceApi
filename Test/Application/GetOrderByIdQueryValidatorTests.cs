using FluentAssertions;
using OrderService.Application.Queries.GetOrderById;
using Xunit;

namespace Test.Application
{
    public class GetOrderByIdQueryValidatorTests
    {
        private readonly GetOrderByIdQueryValidator _validator = new();

        [Fact]
        public void Should_Pass_When_OrderNumber_Is_Valid_Guid()
        {
            var query = new GetOrderByIdQuery(Guid.NewGuid().ToString());

            var result = _validator.Validate(query);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Should_Fail_When_OrderNumber_Is_Empty()
        {
            var query = new GetOrderByIdQuery(string.Empty);

            var result = _validator.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "OrderNumber");
        }

        [Fact]
        public void Should_Fail_When_OrderNumber_Is_Not_Guid()
        {
            var query = new GetOrderByIdQuery("not-a-guid");

            var result = _validator.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "OrderNumber");
        }
    }
}