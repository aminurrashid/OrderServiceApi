using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.API.Extensions;
using OrderService.Application.Commands.CreateOrder;
using OrderService.Application.Queries.GetOrderById;
using Swashbuckle.AspNetCore.Filters;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [SwaggerRequestExample(typeof(CreateOrderCommand), typeof(SwaggerCreateOrderCommandExample))]
        public async Task<IActionResult> CreateOrder(CreateOrderCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetOrderById), new { orderNumber = id }, new { orderNumber = id });
        }

        [HttpGet("{orderNumber}")]
        public async Task<IActionResult> GetOrderById(string orderNumber)
        {
            var result = await _mediator.Send(new GetOrderByIdQuery(orderNumber));
            return result is not null ? Ok(result) : NotFound();
        }
    }
}
