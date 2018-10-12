using System.ComponentModel.DataAnnotations;

namespace BaseballScraper.Models
{
    public class PlayerNote: BaseEntity
    {
        [Key]
        public int idPlayerNotes { get; set; }

        [Display(Name="Player Name")]
        public string PlayerName { get; set; }


        [Display(Name="Position")]
        public string Position { get; set; }


        [Display(Name="Position Type")]
        public string PositionType { get; set; }


        [Display(Name="Note Text")]
        public string Note { get; set; }


        [Display(Name="Note Tone")]
        public string NoteTone { get; set; }

        // news, opinions, stats & analysis, mixed


        [Display(Name="Source Site")]
        public string SourceSite { get; set; }


        [Display(Name="Note Text Writer")]
        public string NoteWriter { get; set; }

        // pre-season, in-season

        [Display(Name="Calendar Year")]
        public int? CalendarYear { get; set; }

        [Display(Name="Mlb Season Year")]
        public int? Season { get; set; }


        public PlayerNote() {}

    }
}