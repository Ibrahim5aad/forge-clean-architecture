using Autodesk.Forge;
using ForgeSample.Application.DTOs;
using ForgeSample.Application.Interfaces;
using System;
using System.Threading.Tasks;
using mUtils = ForgeSample.Utils.Utils;


namespace ForgeSample.Services
{

    /// <summary>
    /// Class TwoLeggedAuthenticationService.
    /// Implements the <see cref="ForgeSample.Application.Interfaces.I2leggedAuthenticationService" />
    /// </summary>
    /// <seealso cref="ForgeSample.Application.Interfaces.I2leggedAuthenticationService" />
    public class TwoLeggedAuthenticationService : I2leggedAuthenticationService
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


        /// <summary>
        /// Gets the expires at.
        /// </summary>
        /// <value>The expires at.</value>
        public DateTime ExpiresAt { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Get access token with public (viewables:read) scope
        /// </summary> 
        public async Task<Token> GetPublicTokenAsync()
        {
            if (PublicToken.access_token == null || ExpiresAt < DateTime.UtcNow)
            {
                PublicToken = await Get2LeggedTokenAsync(new Scope[] { Scope.ViewablesRead });
                ExpiresAt = DateTime.UtcNow.AddSeconds(PublicToken.expires_in);
            }
            return PublicToken;
        }


        /// <summary>
        /// Get access token with internal (write) scope
        /// </summary>
        public async Task<Token> GetInternalTokenAsync()
        {
            if (InternalToken.access_token == null || ExpiresAt < DateTime.UtcNow)
            {
                InternalToken = await Get2LeggedTokenAsync(new Scope[] {
                    Scope.BucketCreate,
                    Scope.BucketRead,
                    Scope.BucketDelete,
                    Scope.DataRead,
                    Scope.DataWrite,
                    Scope.DataCreate
                });
                ExpiresAt = DateTime.UtcNow.AddSeconds(InternalToken.expires_in);
            }
            return InternalToken;
        }


        /// <summary>
        /// Get the access token from Autodesk
        /// </summary>
        private async Task<Token> Get2LeggedTokenAsync(Scope[] scopes)
        {
            TwoLeggedApi oauth = new TwoLeggedApi();
            dynamic bearer = await oauth.AuthenticateAsync(
              mUtils.GetAppSetting("FORGE_CLIENT_ID"),
              mUtils.GetAppSetting("FORGE_CLIENT_SECRET"),
              oAuthConstants.CLIENT_CREDENTIALS,
              scopes);
             
            return new Token
            {
                access_token = bearer.access_token,
                expires_in = (int)bearer.expires_in
            };
        }


        #endregion

    }
}
