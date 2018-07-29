using System.Collections.Specialized;
using System.Threading.Tasks;

using BaseballScraper.Configuration;
using BaseballScraper.Models;

using Microsoft.Extensions.Options;

namespace BaseballScraper.Client
{
    public interface IYahooAuthClient
    {

        string GetLoginLinkUri ();

        AuthModel Auth { get; set; }

        UserInfo UserInfo { get; set; }


        IOptions<YahooConfiguration> Configuration { get; }

        Task<UserInfo> GetUserInfo (NameValueCollection parameters);



        Task<string> GetCurrentToken (string refreshToken = null, bool forceUpdate = false);


        void ClearAuth ();
    }
}
