using BaseballScraper.Infrastructure;

namespace BaseballScraper.Controllers.YahooControllers.Collections
{
    public class YahooTransactionsCollectionController
    {
        // private readonly Helpers _helpers;

        // public YahooTransactionsCollectionController(Helpers helpers)
        // {
        //     _helpers = helpers;
        // }

        // Yahoo Doc Link: https://developer.yahoo.com/fantasysports/guide/#transactions-collection
        // Transactions Collection description (from Yahoo):
        //   * Transactions API allows you to GET from a collection of transactions simultaneously.
        //   * Qualified in the URI by a particular league.
        //   * Each element beneath will be a Transaction Resource
        //   * You can POST to do things like adding or dropping players or proposing trades.
        // URIs
        //   * /transactions/{sub_resource}
        //   * /transactions;transaction_keys={transaction_key1},{transaction_key2}/{sub_resource}
        //   * /transactions;out={sub_resource_1},{sub_resource_2}
        //   * /transactions;transaction_keys={transaction_key1},{transaction_key2};out={sub_resource_1},{sub_resource_2}
    }
}
