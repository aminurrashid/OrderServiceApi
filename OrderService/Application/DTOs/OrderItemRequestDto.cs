
namespace OrderService.Application.DTOs
{
    public record OrderItemRequestDto(string ProductId,
        string ProductName,
        int ProductAmount,
        decimal ProductPrice) : OrderItemDto(
         ProductId,
         ProductName,
         ProductAmount,
         ProductPrice
    );
}
