using Microsoft.AspNetCore.Components;
using ShopOnline.Models.Dtos;
using ShopOnline.Web.Services;
using ShopOnline.Web.Services.Contracts;

namespace ShopOnline.Web.Pages.Account
{
    public class RegisterationBase:ComponentBase
    {
        protected UserRegistrationDto registrationModel = new UserRegistrationDto(); // Initialize a new RegistrationDto object
        protected string confirmPassword;
        [Inject]
        public IUserService UserService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        public string ErrorMessage { get; set; }
        protected async Task HandleRegistration()
        {
            var result = await UserService.RegisterAsync(registrationModel);

            if (result == 200)
            {
                NavigationManager.NavigateTo("/login");
            }
            else
            {
                ErrorMessage = "Registration failed. Please try again.";
            }
        }
    }
}
