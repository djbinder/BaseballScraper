using System;
using System.ComponentModel.DataAnnotations;

// #pragma warning disable CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Models.Player
{
    public class PlayerNote : IBaseEntity
    {
        [Key]
        public int? PlayerNoteId { get; set; }

        [Display(Name="Player Name")]
        public string PlayerName { get; set; }


        [Display(Name="Position")]
        public string Position { get; set; }


        [Display(Name="Position Type")]
        public string PositionType { get; set; }


        [Display(Name="Note Text")]
        public string NoteText { get; set; }


        [Display(Name="Note Tone")]
        public string NoteTone { get; set; }


        [Display(Name="Source Site")]
        public string SourceSite { get; set; }


        [Display(Name="Note Text Writer")]
        public string NoteWriter { get; set; }


        [Display(Name="Calendar Year")]
        public int? CalendarYear { get; set; }


        [Display(Name="Mlb Season Year")]
        public int? Season { get; set; }

        public DateTime DateCreated
        {
            get => new DateTime();
            set => throw new NotSupportedException();
        }

        public DateTime DateUpdated
        {
            get => new DateTime();
            set => throw new NotSupportedException();
        }

        public PlayerNote() {}

    }
}



// public DateTime DateCreated { get; set; }  // from IBaseEntity interface
// public DateTime DateUpdated { get; set; }  // from IBaseEntity interface