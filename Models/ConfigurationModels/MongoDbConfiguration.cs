using System.Runtime.Serialization;

namespace BaseballScraper.Models.ConfigurationModels
{
    // * These connect to 'mongoDbConfiguration.json' file in Configuration folder
    [DataContract]
    public class MongoDbConfiguration : IMongoDbConfiguration
    {
        [DataMember(Name="TweetsCollectionName")]
        public string TweetsCollectionName          { get; set; }

        [DataMember(Name="ConnectionString")]
        public string ConnectionString              { get; set; }

        [DataMember(Name="DatabaseName")]
        public string DatabaseName                  { get; set; }
    }


    // * These connect to 'mongoDbConfiguration.json' file in Configuration folder
    public interface IMongoDbConfiguration
    {
        string TweetsCollectionName { get; set; }
        string ConnectionString     { get; set; }
        string DatabaseName         { get; set; }
    }
}


// See: https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-2.2&tabs=visual-studio-mac
// MongoDb Commands:
    // Switch to or create new Db
        // use <name_of_new_database>
        // if it already exists, will switch to it
        // if it doesn't exist, will create it

    // Create collection in current db
        // db.createCollection('<collection_name>')
            // note that single quotes should be included on both sides of collection name
