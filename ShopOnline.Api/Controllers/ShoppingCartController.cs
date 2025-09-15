using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopOnline.Api.Extensions;
using ShopOnline.Api.Repositories.Contracts;
using ShopOnline.Models.Dtos;

namespace ShopOnline.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IProductRepository productRepository;
        private readonly IShoppingCartRepository shoppingCartRepository;

        public ShoppingCartController(IProductRepository productRepository, IShoppingCartRepository shoppingCartRepository)
        {
            this.productRepository=productRepository;
            this.shoppingCartRepository=shoppingCartRepository;
        }

        [HttpGet]
        [Route("{userId}/GetItems")]
        public async Task<ActionResult<IEnumerable<CartItemDto>>> GetItems(int userId)
        {
            try
            {
                var cartItems = await this.shoppingCartRepository.GetItems(userId);

                if (cartItems == null)
                {
                    return NoContent();
                }
                else
                {

                    var products = await productRepository.GetProducts();

                    if (products == null)
                    {
                        throw new Exception("No products exist in the system");
                    }
                    var cartItemsDto = cartItems.ConvertToDto(products);
                    return Ok(cartItemsDto);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>> GetItem(int id)
        {
            try
            {
                var cartItem = await this.shoppingCartRepository.GetItem(id);
                if (cartItem == null)
                {
                    return NotFound();
                }
                else
                {
                    var product = await this.productRepository.GetProduct(cartItem.ProductId);
                    if (product == null)
                    {
                        return NotFound();
                    }
                    var cartItemDto = cartItem.ConvertToDto(product);
                    return Ok(cartItemDto);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<CartItemDto>> PostItem([FromBody] CartItemDto cartItemToAddDto)
        {
            try
            {
                var newCartItem = await this.shoppingCartRepository.AddItem(cartItemToAddDto);
                if (newCartItem == null)
                {
                    return NoContent();
                }
                var product = await productRepository.GetProduct(newCartItem.ProductId);
                if (product == null)
                {
                    throw new Exception($"Something went wrong when attempting to retrieve product (productId:({cartItemToAddDto.ProductId}))");
                }
                var newCartItemDto = newCartItem.ConvertToDto(product);
                return CreatedAtAction(nameof(GetItem), new { id = newCartItemDto.Id }, newCartItemDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<CartItemDto>> DeleteItem(int id)
        {
            try
            {
                var cartItem = await shoppingCartRepository.DeleteItem(id);
                if (cartItem == null)
                {
                    return NotFound();
                }


                var product = await productRepository.GetProduct(cartItem.ProductId);


                if (product == null)
                    return NotFound();


                var cartItemDto = cartItem.ConvertToDto(product);
                return Ok(cartItemDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult<CartItemDto>> UpdateQty(int id, CartItemQtyUpdateDto cartItemQtyUpdateDto)
        {
            try
            {
                var cartItem = await shoppingCartRepository.UpdateQty(id, cartItemQtyUpdateDto);
                
                if (cartItem == null)
                    return NotFound();
                
                var product = await productRepository.GetProduct(cartItem.ProductId);

                var cartItemDto = cartItem.ConvertToDto(product);
                return Ok(cartItemDto);
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


    }
}
