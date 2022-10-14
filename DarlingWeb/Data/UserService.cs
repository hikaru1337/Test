using DarlingWeb.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DarlingWeb.Data
{
    public class UserService
    {
        private static HttpClient client = new HttpClient();

        /// <summary>
        /// Parses the user's discord claim for their `identify` information
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public DiscordUser GetInfo(HttpContext httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
                return null;

            var claims = httpContext.User.Claims;
            bool? verified = null;
            if (bool.TryParse(claims.FirstOrDefault(x => x.Type == "urn:discord:verified")?.Value, out var _verified))
                verified = _verified;

            var userClaim = new DiscordUser
            {
                UserId = ulong.Parse(claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value),
                Name = claims.First(x => x.Type == ClaimTypes.Name).Value,
                Discriminator = claims.First(x => x.Type == "urn:discord:discriminator").Value,
                Avatar = claims.First(x => x.Type == "urn:discord:avatar").Value,
                Email = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value,
                Verified = verified
            };

            return userClaim;
        }

        /// <summary>
        /// Gets the user's discord oauth2 access token
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task<string> GetUserTokenAsync(HttpContext httpContext)
        {
            string tk = null;
            if (httpContext.User.Identity.IsAuthenticated)
                tk = await httpContext.GetTokenAsync("Discord", "access_token");

            return tk;
        }







        /// <summary>
        /// Gets a list of the user's guilds, Requires `Guilds` scope
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task<List<Guild>> GetBotGuildsAsync(HttpContext httpContext)
        {
            List<Guild> guilds = null;
            if (httpContext.User.Identity.IsAuthenticated)
            {
                var Endpoint = Discord.OAuth2.DiscordDefaults.DiscordApi + "/users/@me/guilds";
                var Content = await ResponseContent(httpContext, Endpoint, HttpMethod.Get);
                guilds = Guild.ListFromJson(Content);
            }
            return guilds;
        }

        public async Task<List<Channel>> GetChannelsGuildsAsync(HttpContext httpContext,ulong GuildId)
        {
            List<Channel> Channels = null;
            if (httpContext.User.Identity.IsAuthenticated)
            {
                var Endpoint = Discord.OAuth2.DiscordDefaults.DiscordApi + $"/guilds/{GuildId}/channels";
                var Content = await ResponseContent(httpContext, Endpoint, HttpMethod.Get);
                Channels = Channel.ListFromJson(Content);
            }
            return Channels;
        }


        private async Task<string> ResponseContent(HttpContext httpContext, string EndPoint, HttpMethod Method)
        {
            using (var request = new HttpRequestMessage(Method, EndPoint))
            {
                string Content = null;
                request.Headers.Authorization = new AuthenticationHeaderValue("Bot", "NzM4OTQwNTg0MzQwNzUwNDQ4.XyTODA.BatsOIwPJVpVg_0SkLKJ6Er8Zcc");
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                    Content = await response.Content.ReadAsStringAsync();

                return Content;
            }
        }

        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
                {
                    new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
                },
        };


    }
}
