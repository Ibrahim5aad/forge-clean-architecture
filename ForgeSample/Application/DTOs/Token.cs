using System;

namespace ForgeSample.Application.DTOs
{
    /// <summary>
    /// Class Token.
    /// </summary>
    public class Token
    {
        public double ExpiresIn { get; set; }

        public string AccessToken { get; internal set; }

        public string RefreshToken { get; internal set; }

        public DateTime ExpiresAt { get; internal set; }

    }
}
