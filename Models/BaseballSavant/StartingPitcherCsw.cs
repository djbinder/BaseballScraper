using System;
using CsvHelper.Configuration;
// using CsvHelper.Configuration;
using Ganss.Excel;

namespace BaseballScraper.Models.BaseballSavant
{
    public class StartingPitcherCsw
    {
        // [Column("pitches")]
        public string CswPitches { get; set; }


        // [Column("player_id")]
        public string PlayerId { get; set; }


        // [Column("player_name")]
        public string PlayerName { get; set; }
        public string TotalPitches { get; set; }
        public string PitchPercent { get; set; }

    }



    public sealed class StartingPitcherCswClassMap: ClassMap<StartingPitcherCsw>
    {
        public StartingPitcherCswClassMap()
        {
            Map( sp => sp.CswPitches).Name("pitches");
            Map( sp => sp.PlayerId).Name("player_id");
            Map( sp => sp.PlayerName).Name("player_name");
            Map( sp => sp.TotalPitches).Name("total_pitches");
            Map( sp => sp.PitchPercent).Name("pitch_percent");
        }
    }
}
