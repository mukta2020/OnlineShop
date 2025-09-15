using ShopOnline.Models.Dtos;

namespace ShopOnline.Web.Services.Contracts
{
    public interface IShoppingCartService
    {
        Task<List<CartItemDto>> GetItems(int userId);
        Task<CartItemDto> AddItem(CartItemToAddDto cartItemToAddDto);

        Task<CartItemDto> UpdateQty(CartItemQtyUpdateDto cartItemQtyUpdateDto);
        Task<CartItemDto> DeleteItem(int id);

        event Action<int> OnShoppingCartChanged;
        void RaiseEventOnShopppingCartChanged(int toalQty);
    }
}
