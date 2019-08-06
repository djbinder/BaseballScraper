using BaseballScraper.Models.Configuration;
using BaseballScraper.Models.Twitter;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

// https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-2.2&tabs=visual-studio-mac
#pragma warning disable CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Infrastructure
{
    // connects to 'MongoDbConfiguration' model
    // check startup.cs file ConfigureServices section to how it's enabled there
    // Configuration is set up in mongoDbConfiguration.json
    public class MongoDbServicer
    {
        private readonly Helpers _helpers;

        private readonly IMongoCollection<TwitterStatus> _twitterStatus;


        public MongoDbServicer(Helpers helpers, IMongoCollection<TwitterStatus> twitterStatus)
        {
            _helpers = helpers;
            _twitterStatus = twitterStatus;
        }

        public MongoDbServicer() {}


        // IMongoDbConfiguration instance is retrieved from DI via constructor injection
        // This allows access to mongoDbConfiguration.json MongoDb configuration values
            // 1) TweetsCollectionName 2) ConnectionString 3) DatabaseName
        public MongoDbServicer(IMongoDbConfiguration config)
        {
            // Reads the server instance for performing database operations
            // The constructor of this class is provided the MongoDB connection string
            var client = new MongoClient(config.ConnectionString);

            // Represents the Mongo database for performing operations
            var database = client.GetDatabase(config.DatabaseName);

            // Uses generic GetCollection<TDocument>(collection) method on the interface
            // * Enables access to data in a specific collection
            // Perform CRUD operations against the collection after this method is called
            // In the GetCollection<TDocument>(collection) method call:
            //      1) collection represents the collection name
            //      2) TDocument represents the CLR object type stored in the collection
            _twitterStatus = database.GetCollection<TwitterStatus>(config.TweetsCollectionName);
        }


        // GetCollection<TDocument>(collection) returns a MongoCollection object representing the collection
        public List<TwitterStatus> Get() =>
            _twitterStatus.Find(twitterStatus => true).ToList();


        // Find<TDocument>: returns all documents in the collection matching the provided search criteria
        public TwitterStatus Get(string id) =>
            _twitterStatus.Find(twitterStatus => twitterStatus.Id == id).FirstOrDefault();


        // // InsertOne: Inserts the provided object as a new document in the collection
        public TwitterStatus Create(TwitterStatus twitterStatus)
        {
            _twitterStatus.InsertOne(twitterStatus);
            return twitterStatus;
        }


        // ReplaceOne: Replaces the single document matching the provided search criteria with the provided object
        public void Update(string id, TwitterStatus twitterStatusToUpdate) =>
            _twitterStatus.ReplaceOne(twitterStatus => twitterStatus.Id == id, twitterStatusToUpdate);


        // DeleteOne: Deletes a single document matching the provided search criteria
        public void Remove(TwitterStatus twitterStatusToRemove) =>
            _twitterStatus.DeleteOne(twitterStatus => twitterStatus.Id == twitterStatusToRemove.Id);


        // DeleteOne: Deletes a single document matching the provided search criteria
        public void Remove(string id) =>
            _twitterStatus.DeleteOne(twitterStatus => twitterStatus.Id == id);
    }
}
