// https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-2.2&tabs=visual-studio-mac

using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BaseballScraper.Models.Twitter
{
    // LinqToTwitter.Status
    // connects to 'MongoDbServicer.cs' and "MongoDbConfiguration' model
    public class TwitterStatus
    {


        // Is annotated with [BsonId] to designate this property as the document's primary key
        // Annotated with [BsonRepresentation(BsonType.ObjectId)]
        // * This allows passing the parameter as a string instead of an ObjectId structure
        // Mongo handles the conversion from string to ObjectId
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }


        // [BsonElement] identifies the properties name in the MongoDB collection
        [BsonElement("StatusType")]
        public int? StatusType { get; set; }


        [BsonElement("StatusId")]
        public int? StatusId { get; set; }


        [BsonElement("UserId")]
        public int? UserId { get; set; }


        [BsonElement("ScreenName")]
        public string ScreenName { get; set; }


        [BsonElement("SinceId")]
        public int? SinceId { get; set; }


        [BsonElement("MaxId")]
        public int? MaxId { get; set; }


        [BsonElement("StatusCount")]
        public int? StatusCount { get; set; }


        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; }


        [BsonElement("StatusIdString")]
        public ulong StatusIdString { get; set; }


        [BsonElement("Text")]
        public string Text { get; set; }


        [BsonElement("FullText")]
        public string FullText { get; set; }


        public int? Cursor { get; set; }
        public bool IncludeRetweets { get; set; }
        public bool ExcludeReplies { get; set; }
        public bool IncludeEntities { get; set; }
        public bool IncludeUserEntities { get; set; }
        public bool IncludeMyRetweet { get; set; }
        public bool IncludeAltText { get; set; }
        public string OEmbedUrl { get; set; }
        public int? OEmbedMaxWidth { get; set; }
        public bool OEmbedHideMedia { get; set; }
        public bool OEmbedHideThread { get; set; }
        public bool OEmbedOmitScript { get; set; }
        public int? OEmbedAlign { get; set; }
        public string OEmbedRelated { get; set; }
        public string OEmbedLanguage { get; set; }




    }
}

//   "IncludeAltText": false,
//   "OEmbedUrl": null,
//   "OEmbedMaxWidth": 0,
//   "OEmbedHideMedia": false,
//   "OEmbedHideThread": false,
//   "OEmbedOmitScript": false,
//   "OEmbedAlign": 0,
//   "OEmbedRelated": null,
//   "OEmbedLanguage": null,
//   "CreatedAt": "2018-07-30T21:11:29Z",
//   "StatusID": 1024039803791323138,
//   "Text": "RT @Cubs: Hendricks' seven innings, Zobrist's four hits lead #Cubs to win over Cardinals.\n\nRecap: https://t.co/vmIOMytDin #EverybodyIn http…",
//   "FullText": null,
