using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using CsvHelper.Configuration;

#pragma warning disable CS0649, MA0048
namespace BaseballScraper.Models.BaseballSavant
{
    public class StartingPitcherCswFullSeason : StartingPitcherCsw
    {
        public int Season { get; set; }
        new public DateTime DateCreated { get; set; }  // from IBaseEntity interface
        new public DateTime DateUpdated { get; set; }  // from IBaseEntity interface
    }


    public class StartingPitcherCswSingleDay : StartingPitcherCsw
    {
        public DateTime DatePitched { get; set; }
        // public virtual StartingPitcherCsw StartingPitcherCsw { get; set; }
    }

    public class StartingPitcherCswDateRange : StartingPitcherCsw
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        new public DateTime DateCreated { get; set; }  // from IBaseEntity interface
        new public DateTime DateUpdated { get; set; }  // from IBaseEntity interface
    }



    public class StartingPitcherCsw : IBaseEntity
    {
        public DateTime DateCreated { get; set; }  // from IBaseEntity interface
        public DateTime DateUpdated { get; set; }  // from IBaseEntity interface



        public string PlayerName        { get; set; }
        public int? PlayerId            { get; set; }
        public int? CswPitches          { get; set; }
        public int? TotalPitches        { get; set; }
        public decimal? CswPitchPercent { get; set; }


        // sometimes, At-Bats is 'null' in a csv so the private and public strings are needed
        private readonly string abs;
        public string Abs => abs ?? "NA";

        [NotMapped]
        private object SpinRateObj { get; set; }

        public int? SpinRate
        {
            get
            {
                // * if SpinRateOjb = string, 0; else...
                return SpinRateObj is string ? 0 : Int32.Parse(SpinRateObj.ToString(), NumberStyles.None, CultureInfo.InvariantCulture);
            }
        }


        // public int? SpinRate { get; set; }

        public decimal? Velocity { get; set; }

        public decimal? EffectiveSpeed { get; set; }

        public int? Whiffs { get; set; }

        public int? Swings { get; set; }

        public int? Takes { get; set; }

        public decimal? EffectiveMinVelocity { get; set; }

        public decimal? ReleaseExtension { get; set; }

        public int? Pos3IntStartDistance { get; set; }

        public int? Pos4IntStartDistance { get; set; }

        public int? Pos5IntStartDistance { get; set; }

        public int? Pos6IntStartDistance { get; set; }

        public int? Pos7IntStartDistance { get; set; }

        public int? Pos8IntStartDistance { get; set; }

        public int? Pos9IntStartDistance { get; set; }

    }


    public sealed class StartingPitcherCswClassMap: ClassMap<StartingPitcherCsw>
    {
        public StartingPitcherCswClassMap()
        {
            Map( sp => sp.CswPitches).Name("pitches");

            Map( sp => sp.PlayerId).Name("player_id");

            Map( sp => sp.PlayerName).Name("player_name");

            Map( sp => sp.TotalPitches).Name("total_pitches");

            Map( sp => sp.CswPitchPercent).Name("pitch_percent");

            Map( sp => sp.Abs).Name("abs");

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



}
