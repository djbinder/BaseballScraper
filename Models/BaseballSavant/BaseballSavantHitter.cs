using System;
using System.ComponentModel.DataAnnotations;
using BaseballScraper.Models.Player;
using CsvHelper.Configuration;
using Ganss.Excel;

#pragma warning disable CS0649
namespace BaseballScraper.Models.BaseballSavant
{
    public class BaseballSavantHitter : BaseEntity
    {
        [Column("last_name")]
        public string LastName { get; set; }

        [Column("first_name")]
        public string FirstName { get; set; }

        [Key]
        [Column("player_id")]
        public int PlayerId { get; set; }

        public virtual SfbbPlayerBase SfbbPlayerBase { get; set; }
        public int MlbIdForeignKey { get; set; }
    }

    public class XstatsHitter : BaseballSavantHitter
    {
        public int Year { get; set; }
        public int PlateAppearances { get; set; }
        public int BallsInPlay { get; set; }
        public double BattingAverage { get; set; }
        public double ExpectedBattingAverage { get; set; }
        public double BattingAverageDifference { get; set; }
        public double SluggingPercentage { get; set; }
        public double ExpectedSluggingPercentage { get; set; }
        public double SluggingPercentageDifference { get; set; }
        public double Woba { get; set; }
        public double ExpectedWoba { get; set; }
        public double WobaDifference { get; set; }
    }


    public sealed class XstatsHitterClassMap: ClassMap<XstatsHitter>
    {
        public XstatsHitterClassMap()
        {
            Map(h => h.FirstName).Name("first_name");
            Map(h => h.LastName).Name("last_name");
            Map(h => h.PlayerId).Name("player_id");

            Map(h => h.Year).Name("year");
            Map(h => h.PlateAppearances).Name("pa");
            Map(h => h.BallsInPlay).Name("bip");
            Map(h => h.BattingAverage).Name("ba");
            Map(h => h.ExpectedBattingAverage).Name("est_ba");
            Map(h => h.BattingAverageDifference).Name("est_ba_minus_ba_diff");
            Map(h => h.SluggingPercentage).Name("slg");
            Map(h => h.ExpectedSluggingPercentage).Name("est_slg");
            Map(h => h.SluggingPercentageDifference).Name("est_slg_minus_slg_diff");
            Map(h => h.Woba).Name("woba");
            Map(h => h.ExpectedWoba).Name("est_woba");
            Map(h => h.WobaDifference).Name("est_woba_minus_woba_diff");
        }
    }





    public class ExitVelocityAndBarrelsHitter : BaseballSavantHitter
    {
        public int Attempts { get; set; }
        public double AverageHitAngle { get; set; }
        public double AngleSweetSpotPercent { get; set; }
        public double MaxExitVelocity { get; set; }
        public double AverageExitVelocity { get; set; }
        public double AverageExitVelocityFlyBallsAndLineDrives { get; set; }
        public double AverageExitVelocityGroundballs { get; set; }
        public int MaxDistance { get; set; }
        public int AverageDistance { get; set; }
        public int? AverageHomeRunDistance { get; set; }
        public int BallsHitHigherThan95mph { get; set; }
        public double PercentageBallsHitHigherThan95mph { get; set; }
        public int NumberOfBarrels { get; set; }
        public double BarrelsPerBattedBallEvent { get; set; }
        public double BarrelsPerPlateAppearance { get; set; }


    }


    public sealed class ExitVelocityAndBarrelsHitterClassMap: ClassMap<ExitVelocityAndBarrelsHitter>
    {
        public ExitVelocityAndBarrelsHitterClassMap()
        {
            Map(h => h.FirstName).Name("first_name");
            Map(h => h.LastName).Name("last_name");
            Map(h => h.PlayerId).Name("player_id");
            Map(h => h.Attempts).Name("attempts");
            Map(h => h.AverageHitAngle).Name("avg_hit_angle");
            Map(h => h.AngleSweetSpotPercent).Name("anglesweetspotpercent");
            Map(h => h.MaxExitVelocity).Name("max_hit_speed");
            Map(h => h.AverageExitVelocity).Name("avg_hit_speed");
            Map(h => h.AverageExitVelocityFlyBallsAndLineDrives).Name("fbld");
            Map(h => h.AverageExitVelocityGroundballs).Name("gb");
            Map(h => h.MaxDistance).Name("max_distance");
            Map(h => h.AverageDistance).Name("avg_distance");
            Map(h => h.AverageHomeRunDistance).Name("avg_hr_distance");
            Map(h => h.BallsHitHigherThan95mph).Name("ev95plus");
            Map(h => h.PercentageBallsHitHigherThan95mph).Name("ev95percent");
            Map(h => h.NumberOfBarrels).Name("barrels");
            Map(h => h.BarrelsPerBattedBallEvent).Name("brl_percent");
            Map(h => h.BarrelsPerPlateAppearance).Name("brl_pa");
        }
    }




}
