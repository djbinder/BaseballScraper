using System;

namespace BaseballScraper.Models
{
    public abstract class BaseEntity
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
