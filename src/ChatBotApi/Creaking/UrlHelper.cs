using System;

namespace ChatBotApi.Creaking
{
    public static class UrlHelper
    {
        public static string ExtractFileName(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
            {
                return Path.GetFileName(uri.LocalPath);
            }
            else
            {
                throw new ArgumentException("Invalid URL format", nameof(url));
            }
        }
    }
}
