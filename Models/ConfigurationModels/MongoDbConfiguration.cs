using System.Runtime.Serialization;

namespace BaseballScraper.Models.ConfigurationModels
{
    [DataContract]
    public class MongoDbConfiguration : IMongoDbConfiguration
    {

        // connects to 'mongoDbConfiguration.json' file in Configuration folder
        [DataMember(Name="TweetsCollectionName")]
        public string TweetsCollectionName { get; set; }


        // connects to 'mongoDbConfiguration.json' file in Configuration folder
        [DataMember(Name="ConnectionString")]
        public string ConnectionString { get; set; }


        // connects to 'mongoDbConfiguration.json' file in Configuration folder
        [DataMember(Name="DatabaseName")]
        public string DatabaseName { get; set; }
    }


    public interface IMongoDbConfiguration
    {
        // connects to 'mongoDbConfiguration.json' file in Configuration folder
        string TweetsCollectionName { get; set; }

        // connects to 'mongoDbConfiguration.json' file in Configuration folder
        string ConnectionString { get; set; }

        // connects to 'mongoDbConfiguration.json' file in Configuration folder
        string DatabaseName { get; set; }
    }
}


// https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-2.2&tabs=visual-studio-mac
// MongoDb Commands:

    // Switch to or create new Db
        // use <name_of_new_database>
        // if it already exists, will switch to it
        // if it doesn't exist, will create it

    // Create collection in current db
        // db.createCollection('<collection_name>')
            // note that single quotes should be included on both sides of collection name
