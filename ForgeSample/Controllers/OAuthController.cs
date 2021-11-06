using ForgeSample.Application.DTOs;
using ForgeSample.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks; 

namespace ForgeSample.Controllers
{
     
    [ApiController]
    public class OAuthController : ControllerBase
    {

        private IMediator _mediator;
        private IAuthenticationService _authenticationService;


        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        public OAuthController(IMediator mediator, IAuthenticationService authenticationService)
        {
            _mediator = mediator;
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Get access token with public (viewables:read) scope
        /// </summary>
        [HttpGet]
        [Route("api/forge/oauth/token")]
        public async Task<Token> GetPublicAsync()
        {
            return await _authenticationService.GetPublicTokenAsync();
        }


    }
}