using System;
using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration;
// using CsvHelper.Configuration;
// using Ganss.Excel;

namespace BaseballScraper.Models.BaseballSavant
{
    public class StartingPitcherCsw : BaseEntity
    {
        [Key]
        public int StartingPitcherCswId { get; set; }



        public string CswPitches { get; set; }

        public string PlayerId { get; set; }

        public string PlayerName { get; set; }

        public string TotalPitches { get; set; }

        public string PitchPercent { get; set; }

        public string Ba { get; set; }

        public string Iso { get; set; }

        public string Babip { get; set; }

        public string Slg { get; set; }

        public string Woba { get; set; }

        public string Xwoba { get; set; }

        public string Xba { get; set; }

        public string Hits { get; set; }

        public string Abs { get; set; }

        public string LaunchSpeed { get; set; }

        public string LaunchAngle { get; set; }

        public string SpinRate { get; set; }

        public string Velocity { get; set; }

        public string EffectiveSpeed { get; set; }

        public string Whiffs { get; set; }

        public string Swings { get; set; }

        public string Takes { get; set; }

        public string EffectiveMinVelocity { get; set; }

        public string ReleaseExtension { get; set; }

        public string Pos3IntStartDistance { get; set; }

        public string Pos4IntStartDistance { get; set; }

        public string Pos5IntStartDistance { get; set; }

        public string Pos6IntStartDistance { get; set; }

        public string Pos7IntStartDistance { get; set; }

        public string Pos8IntStartDistance { get; set; }

        public string Pos9IntStartDistance { get; set; }

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

            Map( sp => sp.Ba ).Name("ba");

            Map( sp => sp.Iso).Name("iso");

            Map( sp => sp.Babip).Name("babip");

            Map( sp => sp.Slg).Name("slg");

            Map( sp => sp.Woba).Name("woba");

            Map( sp => sp.Xwoba).Name("xwoba");

            Map( sp => sp.Xba).Name("xba");

            Map( sp => sp.Hits).Name("hits");

            Map( sp => sp.Abs).Name("abs");

            Map( sp => sp.LaunchSpeed).Name("launch_speed");

            Map( sp => sp.LaunchAngle).Name("launch_angle");

            Map( sp => sp.SpinRate).Name("spin_rate");

            Map( sp => sp.Velocity).Name("velocity");

            Map( sp => sp.EffectiveSpeed).Name("effective_speed");

            Map( sp => sp.Whiffs).Name("whiffs");

            Map( sp => sp.Swings).Name("swings");

            Map( sp => sp.Takes).Name("takes");

            Map( sp => sp.EffectiveMinVelocity).Name("eff_min_vel");

            Map( sp => sp.ReleaseExtension).Name("release_extension");

            Map( sp => sp.Pos3IntStartDistance).Name("pos3_int_start_distance");

            Map( sp => sp.Pos4IntStartDistance).Name("pos4_int_start_distance");

            Map( sp => sp.Pos5IntStartDistance).Name("pos5_int_start_distance");

            Map( sp => sp.Pos6IntStartDistance).Name("pos6_int_start_distance");

            Map( sp => sp.Pos7IntStartDistance).Name("pos7_int_start_distance");

            Map( sp => sp.Pos8IntStartDistance).Name("pos8_int_start_distance");

            Map( sp => sp.Pos9IntStartDistance).Name("pos9_int_start_distance");
        }



    }


    public class StartingPitcherCswSingleDay : StartingPitcherCsw
    {
        public DateTime DatePitched { get; set; }
    }

    public class StartingPitcherCswDateRange : StartingPitcherCsw
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
