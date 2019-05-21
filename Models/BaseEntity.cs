using System;

namespace BaseballScraper.Models
{
    public abstract class BaseEntity
    {
        public DateTime CreatedAt {get;set;}
        public DateTime UpdatedAt {get;set;}


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