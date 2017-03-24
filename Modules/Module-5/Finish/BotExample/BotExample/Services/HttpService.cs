using Microsoft.Bot.Connector;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BotExample.Services
{
    public class HttpService
    {
        public async Task<Stream> GetStreamAsync(Uri uri)
        {
            var httpClient = new HttpClient();

            // The Skype attachment URLs are secured by JwtToken,
            // you should set the JwtToken of your bot as the authorization header for the GET request 
            // your bot initiates to fetch the image.
            // https://github.com/Microsoft/BotBuilder/issues/662
            if (uri.Host.EndsWith("skype.com") && uri.Scheme == "https")
            {
                var token = await new MicrosoftAppCredentials().GetTokenAsync();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
            }

            return await httpClient.GetStreamAsync(uri);
        }
    }
}