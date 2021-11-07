using ForgeSample.Application.DTOs;
using System;
using System.Threading.Tasks;

namespace ForgeSample.Application.Interfaces
{
    /// <summary>
    /// Interface I3leggedAuthenticationService
    /// </summary>
    public interface I3leggedAuthenticationService
    {

        Token InternalToken { get; set; }

        Token PublicToken { get; set; }

        DateTime ExpiresAt { get; set; }

        Task<Token> CreateTokenFromCallbackCodeAsync(string code);

        Task<Token> GetTokenFromSessionAsync();
         

    }
}
