namespace OrderService.Application.DTOs
{
    public record OrderItemDto(
        string ProductId,
        string ProductName,
        int ProductAmount,
        decimal ProductPrice
    );
}
