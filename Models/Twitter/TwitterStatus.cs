using System;

namespace BaseballScraper.Models.Twitter
{
    // LinqToTwitter.Status
    public class TwitterStatus
    {
        public int? StatusType { get; set; }
        public int? StatusId { get; set; }
        public int? UserId { get; set; }
        public string ScreenName { get; set; }
        public int? SinceId { get; set; }
        public int? MaxId { get; set; }
        public int? StatusCount { get; set; }
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
        public DateTime CreatedAt { get; set; }
        public ulong StatusIdString { get; set; }
        public string Text { get; set; }
        public string FullText { get; set; }
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