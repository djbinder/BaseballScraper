using System;

#pragma warning disable CS1998, CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE1006, MA0048
namespace BaseballScraper.Models
{
    public interface IBaseEntity
    {
        DateTime DateCreated { get; set; }
        DateTime DateUpdated { get; set; }
    }

    public class BaseEntity
    {
        private DateTime _dateTime;
        public DateTime CreatedAt
        {
            get => DateTime.Now;
            set => _dateTime = value;
        }

        public DateTime UpdatedAt
        {
            get => DateTime.Now;
            set => _dateTime = value;
        }


        public BaseEntity ()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }

    public class CurrentDateTime : IDateTime
    {
        public DateTime Now => DateTime.Now;
        public int CurrentYear => DateTime.Now.Year;
        public int CurrentMonth => DateTime.Now.Month;
        public int CurrentDay => DateTime.Now.Day;
    }

    public interface IDateTime
    {
        DateTime Now { get; }
    }
}
