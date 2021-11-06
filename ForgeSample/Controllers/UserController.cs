using Autodesk.Forge;
using ForgeSample.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace ForgeSample.Controllers
{

    [ApiController]
    public class UserController : ControllerBase
    {
        private IAuthenticationService _authenticationService;


        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="authenticationService">The authentication service.</param>
        public UserController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }


        [HttpGet]
        [Route("api/forge/user/profile")]
        public async Task<JObject> GetUserProfileAsync()
        {
            UserProfileApi userApi = new UserProfileApi();
            var token = await _authenticationService.GetInternalTokenAsync();
            userApi.Configuration.AccessToken = token.access_token;
            dynamic userProfile = await userApi.GetUserProfileAsync();
            dynamic response = new JObject();
            response.name = string.Format("{0} {1}", userProfile.firstName, userProfile.lastName);
            response.picture = userProfile.profileImages.sizeX40;
            return response;
        }
    }
}
