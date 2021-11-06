using System;

namespace ForgeSample.Application.DTOs
{
    /// <summary>
    /// Class Token.
    /// </summary>
    public struct Token
    {
        public int expires_in { get; set; }

        public string access_token { get; internal set; } 
    }
}
