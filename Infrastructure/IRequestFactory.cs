using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace BaseballScraper.Infrastructure
{
    /// <summary>
    /// Intended for REST client/request creation.
    /// </summary>
    public interface IRequestFactory
    {

        HttpClient CreateClient (AuthenticationHeaderValue auth = null);


        HttpRequestMessage CreateRequest ();
    }

    public class RequestFactory: IRequestFactory
    {

        public HttpClient CreateClient (AuthenticationHeaderValue auth = null)
        {
            return new HttpClient ()
            {
            DefaultRequestHeaders = { Authorization = auth }
            };
        }


        public HttpRequestMessage CreateRequest ()
        {
            return new HttpRequestMessage ();
        }
    }
}
