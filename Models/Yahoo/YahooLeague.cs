// DJB work in progress

using System.Xml.Serialization;

namespace BaseballScraper.Models.Yahoo
{
    [XmlRoot (ElementName = "league")]
    public class YahooLeague
    {
        [XmlElement (ElementName = "league_key")]
        public string LeagueKey { get; set; }

        [XmlElement (ElementName = "league_id")]
        public string LeagueId { get; set; }

        [XmlElement (ElementName = "name")]
        public string LeagueName { get; set; }

        [XmlElement (ElementName = "url")]
        public string LeagueUrl { get; set; }

        [XmlElement (ElementName = "logo_url")]
        public string LeagueLogoUrl { get; set; }

        [XmlElement (ElementName = "password")]
        public string LeaguePassword { get; set; }

        [XmlElement (ElementName = "edit_key")]
        public string EditKey { get; set; }

        [XmlElement (ElementName = "weekly_deadline")]
        public string WeeklyDeadline { get; set; }

        [XmlElement (ElementName = "league_update_timestamp")]
        public string LeagueUpdateTimeStamp { get; set; }

        [XmlElement (ElementName = "scoring_type")]
        public string ScoringType { get; set; }

        [XmlElement (ElementName = "league_type")]
        public string LeagueType { get; set; }

        [XmlElement (ElementName = "renew")]
        public string Renew { get; set; }

        [XmlElement (ElementName = "iris_group_chat_id")]
        public string IrisGroupChatId { get; set; }

        [XmlElement (ElementName = "short_invitation_url")]
        public string ShortInvitationUrl { get; set; }

        [XmlElement (ElementName = "allow_add_to_dl_extra_pos")]
        public string AllowAddToDlExtraPos { get; set; }

        [XmlElement (ElementName = "is_pro_league")]
        public string IsProLeague { get; set; }

        [XmlElement (ElementName = "is_cash_league")]
        public string IsCashLeague { get; set; }

        [XmlElement (ElementName = "current_week")]
        public string CurrentWeek { get; set; }

        [XmlElement (ElementName = "start_week")]
        public string StartWeek { get; set; }

        [XmlElement (ElementName = "start_date")]
        public string StartDate { get; set; }

        [XmlElement (ElementName = "end_week")]
        public string EndWeek { get; set; }

        [XmlElement (ElementName = "end_date")]
        public string EndDate { get; set; }

        [XmlElement (ElementName = "game_code")]
        public string GameCode { get; set; }

        [XmlElement (ElementName = "season")]
        public string Season { get; set; }




    }
}