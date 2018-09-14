using System.ComponentModel.DataAnnotations;

namespace BaseballScraper.Models.Player
{
    public class PlayerNote: BaseEntity
    {
        [Key]
        public int idPlayerNotes { get; set; }

        // [Display(Name="Player Names")]
        public string PlayerName { get; set; }


        // [Display(Name="Position")]
        public string Position { get; set; }
        public string PositionType { get; set; }
        public string Note { get; set; }
        public string NoteTone { get; set; }

        // news, opinions, stats & analysis, mixed
        // public string NoteType { get; set; }
        public string SourceSite { get; set; }
        public string NoteWriter { get; set; }

        // pre-season, in-season
        // public string SeasonPhase { get; set; }
        public int? CalendarYear { get; set; }

        // the year
        public int? MlbSeason { get; set; }


        public PlayerNote() {}

    }
}