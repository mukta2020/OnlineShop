using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using ShopOnline.Models.Dtos;
using ShopOnline.Web.Services.Contracts;
using System.Security.Claims;

namespace ShopOnline.Web.Pages.Account
{
    public class LoginBase : ComponentBase
    {
        protected UserForAuthenticationDto loginModel = new UserForAuthenticationDto();
        [Inject]
        public IUserService UserService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject] 
        protected ToastService ToastService { get; set; }
        public string ErrorMessage { get; set; }
        protected async Task HandleLogin()
        {
            try{

                // Call your authentication service or API to perform user login
                var result = await UserService.LoginAsync(loginModel);

                if (result.IsAuthSuccessful)
                {
                    ToastService.Notify(new(ToastType.Success, $"Employee details saved successfully."));
                    NavigationManager.NavigateTo("/");
                }
                else
                {

                 
                    // Display an error message to the user
                    ErrorMessage = "Invalid credentials. Please try again.";
                }
            }
            catch(Exception ex)
            {
                ErrorMessage = ex.Message;
                ToastService.Notify(new(ToastType.Danger, $"Error: {ex.Message}."));
                Console.WriteLine(ex.Message);
            }
            
        }

        private async Task Logout()
        {
            // await UserService.(loginModel);
            // Redirect to the login page or another appropriate page
            NavigationManager.NavigateTo("/account/login");
        }



        protected List<ToastMessage> messages = new List<ToastMessage>();

        protected void ShowMessage(ToastType toastType) => messages.Add(CreateToastMessage(toastType));

        private ToastMessage CreateToastMessage(ToastType toastType)
        => new ToastMessage
        {
            Type = toastType,
            Title = "Blazor Bootstrap",
            HelpText = $"{DateTime.Now}",
            Message = $"Hello, world! This is a toast message. DateTime: {DateTime.Now}",
        };
    



}
}
