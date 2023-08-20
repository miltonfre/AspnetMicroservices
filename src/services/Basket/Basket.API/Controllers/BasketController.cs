using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using EventBus.Messages.Events;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using AutoMapper;
using MassTransit;
using static StackExchange.Redis.Role;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController:ControllerBase
    {
        private readonly IBasketRepository _repository;
        private readonly DiscountGrpcService _discountGrpcService; 
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IMapper _mapper;
        public BasketController(IBasketRepository repository, DiscountGrpcService discountGrpcService, IMapper mapper)
        {
            _repository=repository;
            _discountGrpcService = discountGrpcService;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("{userName}",Name="GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
        {
            var basket=await _repository.GetBasket(userName);    
            return Ok(basket??new ShoppingCart(userName));
        }
       
        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket(ShoppingCart basket)
        {
            //Connect or consume GRPC method
            //TODO: communicate with discount.Grpc 
            //and calculate latest prices of product into shopping car

            //Consume Disconnt Grpc
            foreach (var item in basket.Items)
            {
               var coupon= await _discountGrpcService.GetDiscount(item.ProductName);
                item.Price = item.Price - coupon.Amount;
            }
            return Ok(await _repository.UpdateBasket(basket));
        }

        [HttpDelete("{userName}", Name = "DeleteBasket")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            await _repository.DeleteBasket(userName);
            return Ok();
        }


        [Route("[action]")]// if specify CheckOut in the post sera llamado este metodo, por default para este controller es UpdateBasket
        [HttpPost]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.Accepted)]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CheckOut([FromBody] BasketCheckout basketCheckout)
        {
            //get existing basket with total price
            var basket = await _repository.GetBasket(basketCheckout.UserName);
           if(basket == null)
            {
                return BadRequest();
            }

            //create basketCheckoutEvent -- Set totalprice on basketCheckout eventMessage

            //Send checkout event to rabbitMQ
            var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
            eventMessage.TotalPrice = basket.TotalPrice;
            await _publishEndpoint.Publish(eventMessage);

            //Remove basket on the redis database
            await _repository.DeleteBasket(basket.UserName);

            return Accepted();
        }

    }
}
