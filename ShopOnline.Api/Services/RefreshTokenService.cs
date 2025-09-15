using Microsoft.Extensions.Caching.Memory;
using ShopOnline.Api.Services.Contracts;

namespace ShopOnline.Api.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _expirationTime = TimeSpan.FromDays(1); // Adjust as needed

        public RefreshTokenService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public string GenerateRefreshToken()
        {
            var refreshToken = Guid.NewGuid().ToString();
            return refreshToken;
        }

        public bool IsRefreshTokenValid(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return false;
            }

            // Check if the refresh token exists in the cache
            if (_cache.TryGetValue(refreshToken, out _))
            {
                return true;
            }

            return false;
        }

        public void StoreRefreshToken(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ArgumentNullException(nameof(refreshToken));
            }

            // Store the refresh token in the cache with an expiration time
            _cache.Set(refreshToken, refreshToken, _expirationTime);
        }

        public void RevokeRefreshToken(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ArgumentNullException(nameof(refreshToken));
            }

            // Remove the refresh token from the cache to revoke it
            _cache.Remove(refreshToken);
        }
    }
}
