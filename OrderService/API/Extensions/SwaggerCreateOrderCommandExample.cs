using OrderService.Application.Commands.CreateOrder;
using OrderService.Application.DTOs;
using Swashbuckle.AspNetCore.Filters;

namespace OrderService.API.Extensions
{
    public class SwaggerCreateOrderCommandExample : IExamplesProvider<CreateOrderCommand>
    {
        public CreateOrderCommand GetExamples()
        {
            return new CreateOrderCommand("100 Test Lane",
                "user@test.com",
                "4111-1111-1111-1111",
                new List<OrderItemRequestDto>
                {
                    new ( ProductId: "1", ProductName: "Product 1", ProductAmount: 2, ProductPrice: 10.0m)
                });
        }
    }
}
