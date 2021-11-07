using Autodesk.Forge;
using ForgeSample.Application.DTOs;
using ForgeSample.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using mUtils = ForgeSample.Utils.Utils;


namespace ForgeSample.Services
{
    /// <summary>
    /// Class ThreeLeggedAuthenticationService.
    /// Implements the <see cref="ForgeSample.Application.Interfaces.I3leggedAuthenticationService" />
    /// </summary>
    /// <seealso cref="ForgeSample.Application.Interfaces.I3leggedAuthenticationService" />
    public class ThreeLeggedAuthenticationService : I3leggedAuthenticationService
    {

        #region Fields

        private IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreeLeggedAuthenticationService"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        public ThreeLeggedAuthenticationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion
         
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
        /// Gets or sets the time when the token expires.
        /// </summary>
        /// <value>The expiry time.</value>
        public DateTime ExpiresAt { get; set; }


        #endregion

        #region Methods

        /// <summary>
        /// Perform the OAuth authorization via code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<Token> CreateTokenFromCallbackCodeAsync(string code)
        {
            ThreeLeggedApi oauth = new ThreeLeggedApi();

            dynamic credentialInternal = await oauth.GettokenAsync(
              mUtils.GetAppSetting("FORGE_CLIENT_ID"), mUtils.GetAppSetting("FORGE_CLIENT_SECRET"),
              oAuthConstants.AUTHORIZATION_CODE, code, mUtils.GetAppSetting("FORGE_CALLBACK_URL"));

            dynamic credentialPublic = await oauth.RefreshtokenAsync(
              mUtils.GetAppSetting("FORGE_CLIENT_ID"), mUtils.GetAppSetting("FORGE_CLIENT_SECRET"),
              "refresh_token", credentialInternal.refresh_token, new Scope[] { Scope.ViewablesRead });

            InternalToken = new Token()
            {
                access_token = credentialInternal.access_token,
                refresh_token = credentialInternal.refresh_token
            };

            PublicToken = new Token()
            {
                access_token = credentialPublic.access_token,
                refresh_token = credentialPublic.refresh_token
            };

            ExpiresAt = DateTime.Now.AddSeconds(credentialInternal.expires_in);

            _httpContextAccessor.HttpContext.Session.SetString("ForgeToken", JsonConvert.SerializeObject(InternalToken));

            return InternalToken;
        }


        /// <summary>
        /// Restore the Token from the session object, refresh if needed
        /// </summary>
        /// <returns></returns>
        public async Task<Token> GetTokenFromSessionAsync()
        {
            if (_httpContextAccessor.HttpContext.Session == null || _httpContextAccessor.HttpContext.Session.GetString("ForgeToken") == null)
                return null;

            Token Token = JsonConvert.DeserializeObject<Token>
                        (_httpContextAccessor.HttpContext.Session.GetString("ForgeToken"));
            if (ExpiresAt < DateTime.Now)
                await RefreshAsync();
            return Token;
        }


        /// <summary>
        /// Refresh the Token (internal & external)
        /// </summary>
        /// <returns></returns>
        private async Task RefreshAsync()
        {
            ThreeLeggedApi oauth = new ThreeLeggedApi();

            dynamic credentialInternal = await oauth.RefreshtokenAsync(
              mUtils.GetAppSetting("FORGE_CLIENT_ID"), mUtils.GetAppSetting("FORGE_CLIENT_SECRET"),
              "refresh_token", PublicToken.refresh_token, new Scope[] { Scope.DataRead, Scope.ViewablesRead });

            dynamic credentialPublic = await oauth.RefreshtokenAsync(
              mUtils.GetAppSetting("FORGE_CLIENT_ID"), mUtils.GetAppSetting("FORGE_CLIENT_SECRET"),
              "refresh_token", credentialInternal.refresh_token, new Scope[] { Scope.ViewablesRead });

            InternalToken.access_token = credentialInternal.access_token;
            PublicToken.access_token = credentialPublic.access_token;
            PublicToken.refresh_token = credentialPublic.refresh_token;
            ExpiresAt = DateTime.Now.AddSeconds(credentialInternal.expires_in);
        }

        #endregion

    }
}
