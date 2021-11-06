using ForgeSample.Application.DTOs;
using System.Threading.Tasks;

namespace ForgeSample.Application.Interfaces
{
    /// <summary>
    /// Interface IAuthenticationService
    /// </summary>
    public interface IAuthenticationService
    { 

        public Token InternalToken { get; set; }

        public Token PublicToken { get; set; }
         
        Task<Token> GetInternalTokenAsync();

        Task<Token> GetPublicTokenAsync();

    }
}
