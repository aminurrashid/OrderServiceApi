using FluentAssertions;
using OrderService.Application.Commands.CreateOrder;
using OrderService.Application.DTOs;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Test.IntegrationTests
{
    public class OrderApiTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public OrderApiTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task POST_orders_WithValidPayload_ReturnsCreatedAndLocation()
        {
            var cmd = new CreateOrderCommand(
                InvoiceAddress: "100 Test Lane",
                InvoiceEmailAddress: "user@test.com",
                InvoiceCreditCardNumber: "4111-1111-1111-1111",
                OrderItems: new List<OrderItemRequestDto>
                {
                    new ( ProductId: "1", ProductName: "Product 1", ProductAmount: 2, ProductPrice: 10.0m)
                }
            );

            var response = await _client.PostAsJsonAsync("/orders", cmd);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();

            var body = await response.Content.ReadFromJsonAsync<Dictionary<string, Guid>>();
            body.Should().ContainKey("orderNumber");
            body["orderNumber"].Should().NotBe(Guid.Empty);
        }
        
        [Fact]
        public async Task POST_orders_WithMissingFields_ReturnsBadRequest()
        {
            var cmd = new CreateOrderCommand(
                InvoiceAddress: "",
                InvoiceEmailAddress: "invalid@test.com",
                InvoiceCreditCardNumber: "",
                OrderItems: new List<OrderItemRequestDto>()
            );

            var response = await _client.PostAsJsonAsync("/orders", cmd);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var errorBody = await response.Content.ReadAsStringAsync();
            errorBody.Should().Contain("Invoice address is required")
                     .And.Contain("Credit card number is required")
                     .And.Contain("At least one product is required");
        }

        [Fact]
        public async Task POST_orders_WithInvalidEmail_ReturnsBadRequest()
        {
            var cmd = new CreateOrderCommand(
                InvoiceAddress: "123 Main St",
                InvoiceEmailAddress: "not-an-email",
                InvoiceCreditCardNumber: "4111-1111-1111-1111",
                OrderItems: new List<OrderItemRequestDto>
                {
                    new OrderItemRequestDto("1", "Product 1", 1, 10.0m)
                }
            );

            var response = await _client.PostAsJsonAsync("/orders", cmd);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var errorBody = await response.Content.ReadAsStringAsync();
            errorBody.Should().Contain("Must be a valid email");
        }

        [Fact]
        public async Task POST_orders_WithInvalidCreditCard_ReturnsBadRequest()
        {
            var cmd = new CreateOrderCommand(
                InvoiceAddress: "123 Main St",
                InvoiceEmailAddress: "user@test.com",
                InvoiceCreditCardNumber: "123",
                OrderItems: new List<OrderItemRequestDto>
                {
                    new ("1", "Product 1", 1, 10.0m)
                }
            );

            var response = await _client.PostAsJsonAsync("/orders", cmd);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var errorBody = await response.Content.ReadAsStringAsync();
            errorBody.Should().Contain("Invalid credit card number");
        }

        [Fact]
        public async Task POST_orders_WithZeroProductAmount_ReturnsBadRequest()
        {
            var cmd = new CreateOrderCommand(
                InvoiceAddress: "123 Main St",
                InvoiceEmailAddress: "user@test.com",
                InvoiceCreditCardNumber: "4111-1111-1111-1111",
                OrderItems: new List<OrderItemRequestDto>
                {
                    new OrderItemRequestDto("1", "Product 1", 0, 10.0m)
                }
            );

            var response = await _client.PostAsJsonAsync("/orders", cmd);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var errorBody = await response.Content.ReadAsStringAsync();
            errorBody.Should().Contain("Amount must be at least 1").And.Contain("ProductAmount");
        }
        

        [Fact]
        public async Task GET_orders_ByExistingId_ReturnsOrderDto()
        {
            var createCmd = new CreateOrderCommand(
                InvoiceAddress: "200 Test Blvd",
                InvoiceEmailAddress: "test2@example.com",
                InvoiceCreditCardNumber: "4111-1111-1111-1111",
                OrderItems: new List<OrderItemRequestDto>
                {
                    new OrderItemRequestDto( ProductId: "1", ProductName: "Product 1", ProductAmount: 2, ProductPrice: 10.0m)
                }
            );
            var postResp = await _client.PostAsJsonAsync("/orders", createCmd);
            postResp.EnsureSuccessStatusCode();
            var created = await postResp.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            var id = created!["orderNumber"];
        
            var getResp = await _client.GetAsync($"/orders/{id}");
            getResp.StatusCode.Should().Be(HttpStatusCode.OK);
        
            var dto = await getResp.Content.ReadFromJsonAsync<OrderDto>();
            dto.Should().NotBeNull();
            dto!.OrderNumber.Should().Be(id);
            dto.OrderItems.Should().HaveCount(1);
            dto.InvoiceEmailAddress.Should().Be("test2@example.com");
        }
        
        [Fact]
        public async Task GET_orders_ByUnknownId_ReturnsNotFound()
        {
            var unknownId = Guid.NewGuid().ToString();
            var response = await _client.GetAsync($"/orders/{unknownId}");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
