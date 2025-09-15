namespace ShopOnline.Api.Services.Contracts
{
    public interface IRefreshTokenService
    { /// <summary>
      /// Generates a new refresh token.
      /// </summary>
      /// <returns>The generated refresh token.</returns>
        string GenerateRefreshToken();

        /// <summary>
        /// Checks if a refresh token is valid.
        /// </summary>
        /// <param name="refreshToken">The refresh token to validate.</param>
        /// <returns>True if the refresh token is valid; otherwise, false.</returns>
        bool IsRefreshTokenValid(string refreshToken);

        /// <summary>
        /// Stores a refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token to store.</param>
        void StoreRefreshToken(string refreshToken);

        /// <summary>
        /// Revokes (removes) a refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token to revoke.</param>
        void RevokeRefreshToken(string refreshToken);
    }
}
