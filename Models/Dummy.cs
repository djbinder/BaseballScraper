using System;

namespace BaseballScraper.Models
{
    public class Dummy
    {
        public int Id { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string DummyString { get; set; }

        public Dummy ()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }

}