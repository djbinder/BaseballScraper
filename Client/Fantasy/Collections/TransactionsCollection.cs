using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

using BaseballScraper.Infrastructure;
using BaseballScraper.Models;

namespace BaseballScraper.Client
{
    /// <summary>
    /// https://developer.yahoo.com/fantasysports/guide/#transactions-collection
    /// With the Transactions API, you can obtain information via GET from a collection of transactions simultaneously.
    /// The transactions collection is qualified in the URI by a particular league.
    /// Each element beneath the Transactions Collection will be a Transaction Resource
    /// </summary>
    public class TransactionsCollectionManager
    {

        public async Task<List<Transaction>> GetTransactions (string[] transactionKeys, EndpointSubResourcesCollection subresources, string AccessToken)
        {
            return await Utils.GetCollection<Transaction> (ApiEndpoints.TransactionsEndPoint (transactionKeys, subresources), AccessToken, "transaction");
        }


        public async Task<List<Transaction>> GetTransactionsLeagues (string[] leagueKeys, EndpointSubResourcesCollection subresources, string AccessToken)
        {
            return await Utils.GetCollection<Transaction> (ApiEndpoints.TransactionsLeagueEndPoint (leagueKeys, subresources), AccessToken, "transaction");
        }

        public List<Transaction> AddPlayer (string[] gameKeys, EndpointSubResourcesCollection subresources, string AccessToken)
        {
            throw new NotImplementedException ();
        }
        // public async Task<List<Transaction>> AddPlayer (string[] gameKeys, EndpointSubResourcesCollection subresources, string AccessToken)
        // {
        //     throw new NotImplementedException ();
        // }

        public List<Transaction> DropPlayer (string AccessToken, string[] gameKeys = null, EndpointSubResourcesCollection subresources = null)
        {
            throw new NotImplementedException ();
        }
        // public async Task<List<Transaction>> DropPlayer (string AccessToken, string[] gameKeys = null, EndpointSubResourcesCollection subresources = null)
        // {
        //     throw new NotImplementedException ();
        // }

        public List<Transaction> AddDropPlayer (string AccessToken, string[] gameKeys = null, EndpointSubResourcesCollection subresources = null)
        {
            throw new NotImplementedException ();
        }
        // public async Task<List<Transaction>> AddDropPlayer (string AccessToken, string[] gameKeys = null, EndpointSubResourcesCollection subresources = null)
        // {
        //     throw new NotImplementedException ();
        // }
    }
}
