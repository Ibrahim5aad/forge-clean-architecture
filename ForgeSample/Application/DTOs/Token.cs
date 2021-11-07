namespace ForgeSample.Application.DTOs
{
    /// <summary>
    /// Class Token.
    /// </summary>
    public class Token
    {
        public int expires_in { get; set; }

        public string access_token { get; internal set; }

        public string refresh_token { get; internal set; }
    }
}
