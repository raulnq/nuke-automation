using Core.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Application.Products;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("products")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public Task<RegisterProduct.Result> RegisterProduct([FromBody] RegisterProduct.Command command) => _mediator.Send(command);

        [HttpGet]
        public Task<ListResults<ListProducts.Result>> ListProducts([FromQuery] ListProducts.Query query) => _mediator.Send(query);
    }
}