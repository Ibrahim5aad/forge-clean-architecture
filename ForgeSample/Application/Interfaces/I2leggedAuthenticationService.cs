using ForgeSample.Application.DTOs;
using System;
using System.Threading.Tasks;

namespace ForgeSample.Application.Interfaces
{
    /// <summary>
    /// Interface I2leggedAuthenticationService
    /// </summary>
    public interface I2leggedAuthenticationService
    { 

        Token InternalToken { get; set; }

        Token PublicToken { get; set; }

        DateTime ExpiresAt { get; set; }

        Task<Token> GetInternalTokenAsync();

        Task<Token> GetPublicTokenAsync();

    }
}
