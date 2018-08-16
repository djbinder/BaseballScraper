using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using BaseballScraper.Models;
using Newtonsoft.Json;

namespace BaseballScraper.Models.Yahoo
{

    // meta: /fantasy/v2/team/{team_key}/metadata
    public class YahooTeamBase
    {
        public int YahooTeamBaseId { get; set; }

        [JsonProperty("team_key")]
        [XmlElement (ElementName = "team_key")]
        public string Key { get; set; }

        [JsonProperty("team_name")]
        [XmlElement (ElementName = "team_name")]
        public string TeamName { get; set; }

        [JsonProperty("team_id")]
        [XmlElement (ElementName = "team_id")]
        public int? TeamId { get; set; }

        [XmlElement (ElementName = "is_owned_by_current_login")]
        public int? IsOwnedByCurrentLogin { get; set; }

        [XmlElement (ElementName = "url")]
        public string Url { get; set; }

        [XmlElement (ElementName = "team_logo")]
        public TeamLogo TeamLogo { get; set; }

        [XmlElement (ElementName = "waiver_priority")]
        public int? WaiverPriority { get; set; }

        [XmlElement (ElementName = "number_of_moves")]
        public int? NumberOfMoves { get; set; }

        [XmlElement (ElementName = "number_of_trades")]
        public int? NumberOfTrades { get; set; }

        [XmlElement (ElementName = "roster_adds")]
        public RosterAdds RosterAdds { get; set; }

        [XmlElement (ElementName = "league_scoring_type")]
        public string LeagueScoringType { get; set; }

        [XmlElement (ElementName = "has_draft_grade")]
        public string HasDraftGrade { get; set; }

        [XmlElement (ElementName = "managers")]
        public IList<TeamManagers> ManagersList { get; set; }

        public TeamManagers TeamManager { get; set; }

        public YahooTeamBase()
        {
            TeamLogo     = new TeamLogo();
            RosterAdds   = new RosterAdds();
            ManagersList = new List<TeamManagers>();
            TeamManager  = new TeamManagers();
        }
    }



    [XmlRoot (ElementName = "team_logo")]
    public class TeamLogo
    {
        public int TeamLogoId { get; set; }

        [XmlElement (ElementName = "size")]
        public string Size { get; set; }

        [XmlElement (ElementName = "url")]
        public string Url { get; set; }
    }


    [XmlRoot (ElementName = "roster_adds")]
    public class RosterAdds
    {
        public int RosterAddsId { get; set; }

        [XmlElement (ElementName = "coverage_type")]
        public string CoverageType { get; set; }

        [XmlElement (ElementName = "coverage_value")]
        public string CoverageValue { get; set; }

        [XmlElement (ElementName = "value")]
        public string Value { get; set; }

        // public int YahooTeamBaseId { get; set; }
        // public YahooTeamBase YahooTeamBase { get; set; }
    }


    public class TeamManagers
    {
        public int TeamManagersId { get; set; }

        [XmlElement (ElementName = "manager_id")]
        public string ManagerId { get; set; }

        [XmlElement (ElementName = "nickname")]
        public string NickName { get; set; }

        [XmlElement (ElementName = "guid")]
        public string Guid { get; set; }

        [XmlElement (ElementName = "is_commissioner")]
        public int? IsCommissioner { get; set; }

        [XmlElement (ElementName = "is_current_login")]
        public int? IsCurrentLogin { get; set; }

        [XmlElement (ElementName = "email")]
        public string Email { get; set; }

        [XmlElement (ElementName = "image_url")]
        public string ImageUrl { get; set; }
    }


    // Season stats: /fantasy/v2/team/{team_key}/stats
    public class YahooTeamStats: YahooTeamBase
    {
        public string Season { get; set; }

        [XmlElement (ElementName = "coverage_type")]
        public string StatCoverageType { get; set; }

        [XmlElement (ElementName = "week")]
        public string WeekNumber { get; set; }

        public string HitsAtBatsId { get; set; } = "60";
        public string HitsAtBatsTotal { get; set; }

        public string RunsId { get; set; } = "7";
        public string RunsTotal { get; set; }

        public string HomeRunsId { get; set; } = "12";
        public string HomeRunsTotal { get; set; }

        public string RbiId { get; set; } = "13";
        public string RbiTotal { get; set; }

        public string StolenBasesId { get; set; } = "16";
        public string StolenBasesTotal { get; set; }

        public string WalksId { get; set; } = "18";
        public string WalksTotal { get; set; }

        public string BattingAverageId { get; set; } = "3";
        public string BattingAverageTotal { get; set; }

        public string InningsPitchedId { get; set; } = "50";
        public string InningsPitchedTotal { get; set; }

        public string WinsId { get; set; } = "28";
        public string WinsTotal { get; set; }

        public string StrikeoutsId { get; set; } = "32";
        public string StrikeoutsTotal { get; set; }

        public string SavesId { get; set; } = "42";
        public string SavesTotal { get; set; }

        public string HoldsId { get; set; } = "48";
        public string HoldsTotal { get; set; }

        public string EraId { get; set; } = "26";
        public string EraTotal { get; set; }

        public string WhipId { get; set; } = "27";
        public string WhipTotal { get; set; }

        public TeamWeekPoints TeamPoints { get; set; }

        public IList<TeamStats> TeamStats { get; set; }

        public YahooTeamStats()
        {
            TeamStats  = new List<TeamStats>();
            TeamPoints = new TeamWeekPoints();
        }
    }


    [XmlRoot (ElementName = "team_stats")]
    public class TeamStats
    {
        [XmlElement (ElementName = "coverage_type")]
        public string CoverageType { get; set; }

        [XmlElement (ElementName = "week")]
        public int Week { get; set; }

        [XmlElement (ElementName = "stats")]
        public YahooStats Stats { get; set; }

        public TeamStats()
        {
            Stats = new YahooStats();
        }
    }


    [XmlRoot (ElementName = "team_points")]
    public class TeamWeekPoints
    {
        [XmlElement (ElementName = "coverage_type")]
        public string CoverageType { get; set; }

        [XmlElement (ElementName = "week")]
        public int? Week { get; set; }

        [XmlElement (ElementName = "total")]
        public double? Total { get; set; }
    }


    // /fantasy/v2/team/{team_key}/standings
    public class YahooTeamStandings: YahooTeamBase
    {
        [XmlElement (ElementName = "rank")]
        public string Rank { get; set; }

        [XmlElement (ElementName = "playoff_seed")]
        public string PlayoffSeed { get; set; }

        [XmlElement (ElementName = "outcome_totals")]
        public OutcomeTotals OutcomeTotals { get; set; }

        [XmlElement (ElementName = "games_back")]
        public string GamesBack { get; set; }

        public YahooTeamStandings()
        {
            OutcomeTotals = new OutcomeTotals();
        }
    }



    [XmlRoot (ElementName = "outcome_totals")]
    public class OutcomeTotals
    {
        [XmlElement (ElementName = "wins")]
        public string Wins { get; set; }

        [XmlElement (ElementName = "losses")]
        public string Losses { get; set; }

        [XmlElement (ElementName = "ties")]
        public string Ties { get; set; }

        [XmlElement (ElementName = "percentage")]
        public string Percentage { get; set; }
    }



    // Roster for a particular week: /fantasy/v2/team/{team_key}/roster;week={week}
    public class YahooTeamRoster: YahooTeamBase
    {
        [XmlElement (ElementName = "coverage_type")]
        public string RosterCoverageType { get ; set; }


        [XmlElement (ElementName = "data")]
        public string Date { get ; set; }


        [XmlElement (ElementName = "is_editable")]
        public string IsEditable { get ; set; }


        [XmlElement (ElementName = "@count")]
        public string Count { get ; set; }


        public IList<YahooTeamRosterPlayer> Players { get; set; }

        public YahooTeamRoster ()
        {
            Players = new List<YahooTeamRosterPlayer>();
        }
    }


    public class YahooTeamRosterPlayer
    {
        [XmlElement (ElementName = "player_key")]
        public string PlayerKey { get; set; }


        [XmlElement (ElementName = "player_id")]
        public string PlayerId { get; set; }


        [XmlElement (ElementName = "name")]
        public PlayerName PlayerName { get; set; }


        [XmlElement (ElementName = "editorial_player_key")]
        public string EditorialPlayerKey { get; set; }


        [XmlElement (ElementName = "editorial_team_key")]
        public string EditorialTeamKey { get; set; }


        [XmlElement (ElementName = "editorial_team_full_name")]
        public string EditorialTeamFullName { get; set; }


        [XmlElement (ElementName = "editorial_team_abbr")]
        public string EditorialTeamAbbreviation { get; set; }


        [XmlElement (ElementName = "uniform_number")]
        public string UniformNumber { get; set; }


        [XmlElement (ElementName = "display_position")]
        public string DisplayPosition { get; set; }


        [XmlElement(ElementName = "headshot", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public Headshot Headshot { get; set; }


        [XmlElement (ElementName = "image_url")]
        public string ImageUrl { get; set; }


        [XmlElement (ElementName = "is_undroppable")]
        public string IsUndroppable { get; set; }


        [XmlElement (ElementName = "position_type")]
        public string PositionType { get; set; }


        [XmlElement (ElementName = "eligible_positions")]
        public string EligiblePositions { get; set; }


        [XmlElement (ElementName = "has_player_notes")]
        public string HasPlayerNotes { get; set; }


        [XmlElement (ElementName = "selected_position")]
        public SelectedPosition SelectedPosition { get; set; }


        [XmlElement (ElementName = "coverage_type")]
        public string StartingStatusCoverageType { get; set; }


        [XmlElement (ElementName = "date")]
        public string StartingStatusDate { get; set; }


        [XmlElement (ElementName = "is_starting")]
        public string IsStartingToday { get; set; }


        [XmlElement (ElementName = "order_num")]
        public string BattingOrderNumber { get; set; }


        [XmlElement (ElementName = "is_editable")]
        public string IsEditable { get; set; }


        public YahooTeamRosterPlayer ()
        {
            PlayerName       = new PlayerName();
            Headshot         = new Headshot();
            SelectedPosition = new SelectedPosition();
        }
    }



    [XmlRoot (ElementName = "name")]
    public class PlayerName
    {
        [XmlElement (ElementName = "full")]
        public string FullName { get; set; }

        [XmlElement (ElementName = "first")]
        public string FirstName { get; set; }

        [XmlElement (ElementName = "last")]
        public string LastName { get; set; }

        [XmlElement (ElementName = "ascii_first")]
        public string AsciiFirstName { get; set; }

        [XmlElement (ElementName = "ascii_last")]
        public string AsciiLastName { get; set; }
    }


    [XmlRoot(ElementName = "headshot", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
    public class Headshot
    {
        [XmlElement(ElementName = "url", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string Url { get; set; }

        [XmlElement(ElementName = "size", Namespace = "http://fantasysports.yahooapis.com/fantasy/v2/base.rng")]
        public string Size { get; set; }
    }


    [XmlRoot(ElementName = "selected_position")]
    public class YahooSelectedPosition
    {
        [XmlElement(ElementName = "coverage_type")]
        public string CoverageType { get; set; }

        [XmlElement(ElementName = "date")]
        public string Date { get; set; }

        [XmlElement(ElementName = "position")]
        public string Position { get; set; }
    }


    // /fantasy/v2/team/{team_key}/draftresults
    public class YahooTeamDraftResult: YahooTeamBase
    {
        [XmlElement (ElementName = "@count")]
        public int? NumberOfPlayersDrafted { get; set; }

        public IList<YahooTeamDraftPick> DraftPicks { get; set; }

        public YahooTeamDraftResult()
        {
            DraftPicks = new List<YahooTeamDraftPick>();
        }
    }



    public class YahooTeamDraftPick
    {
        [XmlElement (ElementName = "pick")]
        public string PickNumber { get; set; }

        [XmlElement (ElementName = "round")]
        public string RoundNumber { get; set; }

        [XmlElement (ElementName = "team_key")]
        public string DraftingTeamsKey { get; set; }

        [XmlElement (ElementName = "player_key")]
        public string PlayerKey { get; set; }
    }



    [XmlRoot (ElementName = "matchups")]
    public class Matchups
    {
        [XmlAttribute (AttributeName = "count")]
        public int Count { get; set; }

        [XmlElement (ElementName = "matchup")]
        public List<Matchup> MatchupsList { get; set; }

        public Matchups()
        {
            MatchupsList = new List<Matchup>();
        }
    }




    // All matchups: /fantasy/v2/team/{team_key}/matchups
    public class Matchup: YahooTeamBase
    {
        [XmlElement (ElementName = "week")]
        public string WeekNumber { get; set; }


        [XmlElement (ElementName = "week_start")]
        public string WeekStart { get; set; }


        [XmlElement (ElementName = "week_end")]
        public string WeekEnd { get; set; }


        [XmlElement (ElementName = "status")]
        public string Status { get; set; }


        [XmlElement (ElementName = "is_playoffs")]
        public string IsPlayoffs { get; set; }


        [XmlElement (ElementName = "is_consolation")]
        public string IsConsolation { get; set; }


        [XmlElement (ElementName = "is_tied")]
        public string IsTied { get; set; }


        [XmlElement (ElementName = "winner_team_key")]
        public string WinnerTeamKey { get; set; }


        [XmlElement (ElementName = "stat_winner")]
        public List<StatWinner> StatWinner { get; set; }


        [XmlElement (ElementName = "stat")]
        public List<YahooTeamStats> Stats { get; set; }


        public Matchup()
        {
            StatWinner = new List<StatWinner>();
            Stats      = new List<YahooTeamStats>();
        }
    }


    [XmlRoot (ElementName = "stat_winner")]
    public class StatWinner
    {
        [XmlElement (ElementName = "stat_id")]
        public string StatId { get; set; }


        [XmlElement (ElementName = "winner_team_key")]
        public string WinnerTeamKey { get; set; }
    }




}
