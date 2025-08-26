namespace OrderService.Application.DTOs
{
    public record OrderDto(
        string OrderNumber,
        string InvoiceAddress,
        string InvoiceEmailAddress,
        string InvoiceCreditCardNumber,
        DateTime CreatedAt,
        List<OrderItemReponseDto> OrderItems 
    );
}
