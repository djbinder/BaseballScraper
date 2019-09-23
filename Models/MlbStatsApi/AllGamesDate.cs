using System.Collections.Generic;
using System.Runtime.Serialization;



#pragma warning disable MA0016, MA0048
namespace BaseballScraper.Models.MlbStatsApi
{
    [DataContract]
    public partial class AllGamesForDate
    {
        [DataMember(Name="copyright")]
        public string Copyright { get; set; }

        [DataMember(Name="totalItems")]
        public int? TotalItems { get; set; }

        [DataMember(Name="totalEvents")]
        public int? TotalEvents { get; set; }

        [DataMember(Name="totalGames")]
        public int? TotalGames { get; set; }

        [DataMember(Name="totalGamesInProgress")]
        public int? TotalGamesInProgress { get; set; }

        [DataMember(Name="dates")]
        public List<Date> Dates { get; set; }
    }

    public partial class Date
    {
        [DataMember(Name="date")]
        public string DateDate { get; set; }

        [DataMember(Name="totalItems")]
        public int? TotalItems { get; set; }

        [DataMember(Name="totalEvents")]
        public int? TotalEvents { get; set; }

        [DataMember(Name="totalGames")]
        public int? TotalGames { get; set; }

        [DataMember(Name="totalGamesInProgress")]
        public int? TotalGamesInProgress { get; set; }

        [DataMember(Name="games")]
        public List<Game> Games { get; set; }

        [DataMember(Name="events")]
        public List<object> Events { get; set; }
    }

    public partial class Game
    {
        [DataMember(Name="gamePk")]
        public int? GamePk { get; set; }

        [DataMember(Name="link")]
        public string Link { get; set; }

        [DataMember(Name="gameType")]
        public string GameType { get; set; }

        [DataMember(Name="season")]
        // [JsonConverter(typeof(ParseStringConverter))]
        public int? Season { get; set; }

        [DataMember(Name="gameDate")]
        public string GameDate { get; set; }

        [DataMember(Name="status")]
        public Status Status { get; set; }

        [DataMember(Name="teams")]
        public Teams Teams { get; set; }

        [DataMember(Name="venue")]
        public Venue Venue { get; set; }

        [DataMember(Name="content")]
        public Content Content { get; set; }

        [DataMember(Name="isTie")]
        public bool? IsTie { get; set; }

        [DataMember(Name="gameNumber")]
        public int? GameNumber { get; set; }

        [DataMember(Name="publicFacing")]
        public bool PublicFacing { get; set; }

        [DataMember(Name="doubleHeader")]
        public string DoubleHeader { get; set; }

        [DataMember(Name="gamedayType")]
        public string GamedayType { get; set; }

        [DataMember(Name="tiebreaker")]
        public string Tiebreaker { get; set; }

        [DataMember(Name="calendarEventID")]
        public string CalendarEventId { get; set; }

        [DataMember(Name="seasonDisplay")]
        public int? SeasonDisplay { get; set; }

        [DataMember(Name="dayNight")]
        public string DayNight { get; set; }

        [DataMember(Name="scheduledInnings")]
        public int? ScheduledInnings { get; set; }

        [DataMember(Name="gamesInSeries")]
        public int? GamesInSeries { get; set; }

        [DataMember(Name="seriesGameNumber")]
        public int? SeriesGameNumber { get; set; }

        [DataMember(Name="seriesDescription")]
        public string SeriesDescription { get; set; }

        [DataMember(Name="recordSource")]
        public string RecordSource { get; set; }

        [DataMember(Name="ifNecessary")]
        public string IfNecessary { get; set; }

        [DataMember(Name="ifNecessaryDescription")]
        public string IfNecessaryDescription { get; set; }

        [DataMember(Name="rescheduleDate")]
        public string RescheduleDate { get; set; }
    }

    public partial class Content
    {
        [DataMember(Name="link")]
        public string Link { get; set; }
    }

    public partial class Status
    {
        [DataMember(Name="abstractGameState")]
        public string AbstractGameState { get; set; }

        [DataMember(Name="codedGameState")]
        public string CodedGameState { get; set; }

        [DataMember(Name="detailedState")]
        public string DetailedState { get; set; }

        [DataMember(Name="statusCode")]
        public string StatusCode { get; set; }

        [DataMember(Name="abstractGameCode")]
        public string AbstractGameCode { get; set; }

        [DataMember(Name="reason")]
        public string Reason { get; set; }
    }

    public partial class Teams
    {
        [DataMember(Name="away")]
        public Away Away { get; set; }

        [DataMember(Name="home")]
        public Away Home { get; set; }
    }

    public partial class Away
    {
        [DataMember(Name="leagueRecord")]
        public LeagueRecord LeagueRecord { get; set; }

        [DataMember(Name="score")]
        public int? Score { get; set; }

        [DataMember(Name="team")]
        public Venue Team { get; set; }

        [DataMember(Name="isWinner")]
        public bool? IsWinner { get; set; }

        [DataMember(Name="splitSquad")]
        public bool SplitSquad { get; set; }

        [DataMember(Name="seriesNumber")]
        public int? SeriesNumber { get; set; }
    }

    public partial class LeagueRecord
    {
        [DataMember(Name="wins")]
        public int? Wins { get; set; }

        [DataMember(Name="losses")]
        public int? Losses { get; set; }

        [DataMember(Name="pct")]
        public string Pct { get; set; }
    }

    public partial class Venue
    {
        [DataMember(Name="id")]
        public int? Id { get; set; }

        [DataMember(Name="name")]
        public string Name { get; set; }

        [DataMember(Name="link")]
        public string Link { get; set; }
    }
}
