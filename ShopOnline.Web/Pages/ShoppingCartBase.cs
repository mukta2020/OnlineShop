using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ShopOnline.Models.Dtos;
using ShopOnline.Web.Services.Contracts;

namespace ShopOnline.Web.Pages
{
    public class ShoppingCartBase:ComponentBase
    {
        [Inject]
        public IShoppingCartService ShoppingCartService { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        public List<CartItemDto> ShoppingCartItems { get; set; }
        public string ErrorMessage { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                ShoppingCartItems  = await ShoppingCartService.GetItems(HardCoded.UserId);
                CartChanged();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
        public async void UpdateQty_Input(int id)
        {
           await MakeUpdateQtyButtonVisible(id, true);
        }

        public async Task MakeUpdateQtyButtonVisible(int id, bool visible)
        {
            await JSRuntime.InvokeVoidAsync("MakeUpdateQtyButtonVisible", id, visible);
        }
        public async void UpdateQtyCartItem_Click(int id, int qty)
        {
            try
            {
                if (qty>0)
                {
                    var updateItemDto = new CartItemQtyUpdateDto()
                    {
                        CartItemId = id,
                        Qty = qty
                    };

                    var returnUpdateItemDto = await ShoppingCartService.UpdateQty(updateItemDto);
                    UpdateItemTotalPrice(returnUpdateItemDto);
                    CartChanged();
                     await MakeUpdateQtyButtonVisible(id, false);
                }
                else
                {
                    var item = ShoppingCartItems.FirstOrDefault(i=>i.Id == id);

                    if(item != null)
                    {
                        item.Qty = 1;
                        item.TotalPrice = item.Price;
                    }
                }
            }
            catch (Exception)
            {

            }
        }
        protected async Task DeleteCartItem_Click(int id)
        {
            var cartItemDto = await ShoppingCartService.DeleteItem(id);
            RemoveCartItem(id);
            CalculateCartSummeryTotals();
        }

        private CartItemDto GetCartItems(int id)
        {
            return ShoppingCartItems.FirstOrDefault(i=>i.Id == id);
        }
        private void RemoveCartItem(int id)
        {
            var cartItmeDto = GetCartItems(id);
            ShoppingCartItems.Remove(cartItmeDto);

        }
        private void UpdateItemTotalPrice(CartItemDto cartItemDto)
        {
            var item = GetCartItems(cartItemDto.Id);
            if (item != null)
                item.TotalPrice = cartItemDto.Price * cartItemDto.Qty;

        }
        private void SetTotalPrice()
        {
            TotalPrice = this.ShoppingCartItems.Sum(x=>x.TotalPrice).ToString("C");
        }
        private void SetTotalQty()
        {
            TotalQuantity = ShoppingCartItems.Sum(x=>x.Qty);
        }
        private void CalculateCartSummeryTotals()
        {
            SetTotalPrice();
            SetTotalQty();
        }

        private void CartChanged()
        {
            CalculateCartSummeryTotals();
            ShoppingCartService.RaiseEventOnShopppingCartChanged(TotalQuantity);
        }
        public int TotalQuantity { get; set; }
        public string TotalPrice { get; set; }

    }
}
