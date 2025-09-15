using ShopOnline.Models.Dtos;

namespace ShopOnline.Web.Services.Contracts
{
    public interface IUserService
    {

        Task<AuthResponseDto> LoginAsync(UserForAuthenticationDto loginModel);
        Task<int> RegisterAsync(UserRegistrationDto userRegistrationDto);
    }
}
