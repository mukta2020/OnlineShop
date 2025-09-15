using Microsoft.AspNetCore.Components;

namespace ShopOnline.Web.Pages.Account
{
    public class RedirectToLogin : ComponentBase
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        protected override void OnInitialized()
        {
            NavigationManager.NavigateTo("/login");
        }
    }
}
