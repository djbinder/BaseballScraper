using System.ComponentModel.DataAnnotations;

#pragma warning disable CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Models.Player
{
    public class PlayerNote: BaseEntity
    {
        [Key]
        public int PlayerNoteId { get; set; }

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
