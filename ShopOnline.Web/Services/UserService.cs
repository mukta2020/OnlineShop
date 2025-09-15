using Microsoft.AspNetCore.Components.Authorization;
using ShopOnline.Models.Dtos;
using ShopOnline.Web.Services.Contracts;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;

namespace ShopOnline.Web.Services
{
    public class UserService:IUserService
    {
        private readonly HttpClient httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        public readonly ILocalStorageService localStorage;

        public UserService(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider, ILocalStorageService localStorage)
        {
            this.httpClient=httpClient;
            this._authenticationStateProvider=authenticationStateProvider;
            this.localStorage=localStorage;
        }

        public async Task<int> RegisterAsync(UserRegistrationDto userRegistrationDto)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync("api/Account/Register", userRegistrationDto);
                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return (int)response.StatusCode;
                    }
                    return (int)response.StatusCode;
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Http status:{response.StatusCode} Message -{message}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        public async Task<AuthResponseDto> LoginAsync(UserForAuthenticationDto loginModel)
        {
            var loginAsJson = JsonSerializer.Serialize(loginModel);
            var response = await httpClient.PostAsync("api/Account/Login", new StringContent(loginAsJson, Encoding.UTF8, "application/json"));
            var loginResult = JsonSerializer.Deserialize<AuthResponseDto>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (!response.IsSuccessStatusCode)
            {
                return loginResult!;
            }

            await localStorage.SaveStringAsync("authToken", loginResult!.AccessToken);
            ((CustomAuthStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(loginModel.Email!);
         //   httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult.AccessToken);
            return loginResult;
        }

        public async Task Logout()
        {
            await localStorage.RemoveAsync("authToken");
            ((CustomAuthStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
