// https://legacy.baseballprospectus.com/sortable/extras/dra_runs.php


using System.ComponentModel.DataAnnotations;

namespace BaseballScraper.Models.BaseballProspectus
{
    public class BpSpDraModel : BaseEntity
    {
        public string PitcherName { get; set; }

        public string TeamName { get; set; }

        [Key]
        public string BaseballProspectusId { get; set; }

        public decimal? InningsPitched { get; set; }

        public decimal? Dra { get; set; }

        public decimal? DraSd { get; set; }

        public decimal? DraMinus { get; set; }

        public decimal? Ra9 { get; set; }

        public decimal? DraMinusRa9 { get; set; }

        public decimal? NipRuns { get; set; }

        public decimal? HitRuns { get; set; }

        public decimal? OutRuns { get; set; }

        public decimal? Framing { get; set; }

        public decimal? Warp { get; set; }

        public decimal? WarpSd { get; set; }








    }
}
