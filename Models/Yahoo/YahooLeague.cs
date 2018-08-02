using System.Collections.Generic;
using System.Xml.Serialization;

namespace BaseballScraper.Models.Yahoo
{
    public class YahooLeague
    {

        [XmlElement (ElementName = "league_key", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string LeagueKey { get; set; } //


        [XmlElement (ElementName = "league_id", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string LeagueId { get; set; } //


        [XmlElement (ElementName = "league_type", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string LeagueType { get; set; }


        [XmlElement (ElementName = "game_code", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string GameCode { get; set; }


        [XmlElement (ElementName = "season", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string Season { get; set; }


        [XmlElement (ElementName = "name", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string LeagueName { get; set; } //


        [XmlElement (ElementName = "url", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string LeagueUrl { get; set; } //


        [XmlElement (ElementName = "current_week", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public int? CurrentWeek { get; set; }


        [XmlElement (ElementName = "start_week", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public int? StartWeek { get; set; }


        [XmlElement (ElementName = "start_date", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string StartDate { get; set; }


        [XmlElement (ElementName = "end_week", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public int? EndWeek { get; set; }


        [XmlElement (ElementName = "end_date", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string EndDate { get; set; }


        [XmlElement (ElementName = "draft_status", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string DraftStatus { get; set; }//


        [XmlElement (ElementName = "num_teams", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public int? NumTeams { get; set; }//


        [XmlElement (ElementName = "settings", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public LeagueSettings LeagueSettings { get; set; } //


        [XmlElement (ElementName = "standings", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public Standings Standings { get; set; } //


        [XmlElement (ElementName = "scoreboard", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public Scoreboard Scoreboard { get; set; }//


        [XmlElement (ElementName = "players", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public List<YahooPlayer> PlayerList { get; set; } //


        [XmlElement (ElementName = "teams", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public List<YahooTeam> TeamsList { get; set; } //


        [XmlElement (ElementName = "draft_results")]
        public DraftResults DraftResults { get; set; } //
    }



    [XmlRoot (ElementName = "settings", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public class LeagueSettings
    {
        [XmlElement (ElementName = "draft_type", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string DraftType { get; set; } //


        [XmlElement (ElementName = "scoring_type", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string ScoringType { get; set; } //


        [XmlElement (ElementName = "custom_league_url", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string CustomLeagueUrl { get; set; } //


        [XmlElement (ElementName = "draft_time", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string DraftTime { get; set; } //


        [XmlElement (ElementName = "is_cash_league", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string IsCashLeague { get; set; } //


        [XmlElement (ElementName = "keeper_settings", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string KeeperSettings { get; set; } //


        [XmlElement (ElementName = "max_teams", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string MaxTeams { get; set; } //


        [XmlElement (ElementName = "player_universe", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string PlayerUniverse { get; set; } //


        [XmlElement (ElementName = "max_acquisitions", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public int? MaxAcquisitions { get; set; } //


        [XmlElement (ElementName = "max_trades", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public int? MaxTrades { get; set; } //


        [XmlElement (ElementName = "trade_end_date", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string TradeEndDate { get; set; } //


        [XmlElement (ElementName = "can_trade_draft_picks", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string CanTradeDraftPicks { get; set; } //


        [XmlElement (ElementName = "waiver_time", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string WaiverTime { get; set; } //


        [XmlElement (ElementName = "waiver_type", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string WaiverType { get; set; } //


        [XmlElement (ElementName = "waiver_rule", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string WaiverRule { get; set; } //


        [XmlElement (ElementName = "add_direct_to_DL", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string AddDirectToDL { get; set; } //


        [XmlElement (ElementName = "cant_cut_list", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string CantCutList { get; set; } //


        [XmlElement (ElementName = "post_draft_players", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string PostDraftPlayers { get; set; } //


        [XmlElement (ElementName = "max_acquisitions_per_week", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string MaxAcquisitionsPerWeek { get; set; } //


        [XmlElement (ElementName = "trade_review_type", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string TradeReviewType { get; set; } //


        [XmlElement (ElementName = "trade_reject_time", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string TradeRejectTime { get; set; } //


        [XmlElement (ElementName = "max_innings_pitched", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string MaxInningsPitched { get; set; } //


        [XmlElement (ElementName = "weekly_deadline", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string WeeklyDeadline { get; set; } //


        [XmlElement (ElementName = "start_scoring_on", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string StartScoringOn { get; set; } //


        [XmlElement (ElementName = "playoffs", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string Playoffs { get; set; }//


        [XmlElement (ElementName = "playoff_tiebreaker", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string PlayoffTieBreaker { get; set; } //


        [XmlElement (ElementName = "playoff_start_week", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string PlayoffStartWeek { get; set; } //


        [XmlElement (ElementName = "playoff_end_week", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string PlayoffEndWeek { get; set; } //


        [XmlElement (ElementName = "uses_playoff_reseeding", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string UsesPlayoffReseeding { get; set; } //


        [XmlElement (ElementName = "uses_lock_eliminated_teams", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string UsesLockEliminatedTeams { get; set; } //


        [XmlElement (ElementName = "num_playoff_teams", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string NumPlayoffTeams { get; set; } //


        [XmlElement (ElementName = "uses_divisions", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string UsesDivisions { get; set; } //


        [XmlElement (ElementName = "is_publicly_viewable", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string IsPubliclyViewable { get; set; } //


        [XmlElement (ElementName = "roster_positions", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public RosterPositions RosterPositions { get; set; } //


        [XmlElement (ElementName = "batter_stat_categories", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public List<string> HitterStatCategories { get; set; } //


        [XmlElement (ElementName = "pitcher_stat_categories", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public List<string> PitcherStatCategories { get; set; } //
    }

    [XmlRoot (ElementName = "draft_result")]
    public class DraftResult
    {
        [XmlElement (ElementName = "pick")]
        public int Pick { get; set; }

        [XmlElement (ElementName = "round")]
        public int Round { get; set; }

        [XmlElement (ElementName = "team_key")]
        public string TeamKey { get; set; }

        [XmlElement (ElementName = "player_key")]
        public string PlayerKey { get; set; }
    }

    [XmlRoot (ElementName = "draft_results")]
    public class DraftResults
    {
        [XmlElement (ElementName = "draft_result")]
        public List<DraftResult> DraftResult { get; set; }

        [XmlAttribute (AttributeName = "count")]
        public string Count { get; set; }
    }
}