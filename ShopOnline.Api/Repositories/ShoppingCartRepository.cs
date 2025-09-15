using Microsoft.EntityFrameworkCore;
using ShopOnline.Api.Data;
using ShopOnline.Api.Entities;
using ShopOnline.Api.Repositories.Contracts;
using ShopOnline.Models.Dtos;

namespace ShopOnline.Api.Repositories
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly ShopOnlineDbContext context;

        public ShoppingCartRepository(ShopOnlineDbContext context)
        {
            this.context=context;
        }
        private async Task<bool> CartItemExists(int cartId, int productId)
        {
            return await context.CartItems.AnyAsync(c=> c.CartId == cartId && c.ProductId == productId);
        }
        public async Task<CartItem> AddItem(CartItemDto cartItemDto)
        {
            if (await CartItemExists(cartItemDto.CartId, cartItemDto.ProductId)==false)
            {
                var item = await (from product in context.Products
                                  where product.Id == cartItemDto.ProductId
                                  select new CartItem
                                  {
                                      CartId = cartItemDto.CartId,
                                      ProductId = cartItemDto.ProductId,
                                      Qty = cartItemDto.Qty,
                                  }).SingleOrDefaultAsync();
                if (item != null)
                {
                    var result = await context.CartItems.AddAsync(item);
                    await context.SaveChangesAsync();
                    return result.Entity;
                }
            }
            return null;
        }

        public async Task<CartItem> DeleteItem(int id)
        {
            var item = await context.CartItems.FindAsync(id);
            if(item != null)
            {
                context.CartItems.Remove(item);
                await context.SaveChangesAsync();
            }
            return item;
        }

        public async Task<CartItem> GetItem(int id)
        {
            return await (from cart in context.Carts
                          join cartItem in context.CartItems
                          on cart.Id equals cartItem.CartId
                          where cartItem.Id == id
                          select new CartItem()
                          {
                              CartId = cartItem.CartId,
                              ProductId = cartItem.ProductId,
                              Id = cartItem.Id,
                              Qty = cartItem.Qty,
                          }).SingleAsync();
        }

        public async Task<IEnumerable<CartItem>> GetItems(int userId)
        {
            return await (from cart in context.Carts
                          join cartItem in context.CartItems on cart.Id equals cartItem.CartId
                          where cart.UserId == userId
                          select new CartItem
                          {
                              Id = cartItem.Id,
                              ProductId = cartItem.ProductId,
                              Qty = cartItem.Qty

                          }).ToListAsync();
        }

        public async Task<CartItem> UpdateQty(int id, CartItemQtyUpdateDto cartItemDto)
        {
            var item = await context.CartItems.FindAsync(id);
            if(item != null)
            {
                item.Qty = cartItemDto.Qty;
                await context.SaveChangesAsync();
                return item;
            }
            return null;
        }
    }
}
