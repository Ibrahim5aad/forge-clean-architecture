using Autodesk.Forge;
using ForgeSample.Application.DTOs;
using ForgeSample.Application.Interfaces;
using System;
using System.Threading.Tasks;
using mUtils = ForgeSample.Utils.Utils;


namespace ForgeSample.Services
{

    /// <summary>
    /// Class AuthenticationService.
    /// Implements the <see cref="ForgeSample.Application.Interfaces.IAuthenticationService" />
    /// </summary>
    /// <seealso cref="ForgeSample.Application.Interfaces.IAuthenticationService" />
    public class AuthenticationService : IAuthenticationService
    {

        #region Properties

        /// <summary>
        /// Gets or sets the internal token.
        /// </summary>
        /// <value>The internal token.</value>
        public Token InternalToken { get; set; }


        /// <summary>
        /// Gets or sets the public token.
        /// </summary>
        /// <value>The public token.</value>
        public Token PublicToken { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Get access token with public (viewables:read) scope
        /// </summary> 
        public async Task<Token> GetPublicTokenAsync()
        {
            if (PublicToken == null || PublicToken.ExpiresAt < DateTime.UtcNow)
            {
                PublicToken = await Get2LeggedTokenAsync(new Scope[] { Scope.ViewablesRead });
                PublicToken.ExpiresAt = DateTime.UtcNow.AddSeconds(PublicToken.ExpiresIn);
            }
            return PublicToken;
        }


        /// <summary>
        /// Get access token with internal (write) scope
        /// </summary>
        public async Task<Token> GetInternalTokenAsync()
        {
            if (InternalToken == null || InternalToken.ExpiresAt < DateTime.UtcNow)
            {
                InternalToken = await Get2LeggedTokenAsync(new Scope[] {
                    Scope.BucketCreate,
                    Scope.BucketRead,
                    Scope.BucketDelete,
                    Scope.DataRead,
                    Scope.DataWrite,
                    Scope.DataCreate
                });
                InternalToken.ExpiresAt = DateTime.UtcNow.AddSeconds(InternalToken.ExpiresIn);
            }
            return InternalToken;
        }


        /// <summary>
        /// Get the access token from Autodesk
        /// </summary>
        private async Task<Token> Get2LeggedTokenAsync(Scope[] scopes)
        {
            TwoLeggedApi oauth = new TwoLeggedApi();
            string grantType = "client_credentials";
            dynamic bearer = await oauth.AuthenticateAsync(
              mUtils.GetAppSetting("FORGE_CLIENT_ID"),
              mUtils.GetAppSetting("FORGE_CLIENT_SECRET"),
              grantType,
              scopes);
            return new Token
            {
                AccessToken = bearer.access_token,
                ExpiresIn = bearer.expires_in
            };
        }


        #endregion

    }
}
