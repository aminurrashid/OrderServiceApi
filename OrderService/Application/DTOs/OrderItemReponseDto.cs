namespace OrderService.Application.DTOs
{
    public record OrderItemReponseDto(string Id,string ProductId,
        string ProductName,
        int ProductAmount,
        decimal ProductPrice) : OrderItemDto(
         ProductId,
         ProductName,
         ProductAmount,
         ProductPrice
    );
}
