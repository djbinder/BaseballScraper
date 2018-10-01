using System;
using System.Collections.Generic;

namespace BaseballScraper.Models.FanGraphs
{
    public class FanGraphsHitter
    {
        public string FanGraphsId { get; set; }
        public string FanGraphsName { get; set; }
        public string FanGraphsTeam { get; set; }
        public string GamesPlayed { get; set; }
        public string PlateAppearances { get; set; }
        public string HomeRuns { get; set; }
        public string Runs { get; set; }
        public string RunsBattedIn { get; set; }
        public string StolenBases { get; set; }
        public string WalkPercentage { get; set; }
        public string StrikeoutPercentage { get; set; }
        public string Iso { get; set; }
        public string Babip { get; set; }
        public string BattingAverage { get; set; }
        public string OnBasePercentage { get; set; }
        public string SluggingPercentage { get; set; }
        public string wOba { get; set; }
        public string wRcPlus { get; set; }
        public string BaseRunningRunsAboveReplacement { get; set; }
        public string Offense { get; set; }
        public string Defense { get; set; }
        public string WinsAboveReplacement { get; set; }

        public FanGraphsHitter () { }
    }

}

//FanGraphsHitter NewFanGraphsHitter = new FanGraphsHitter
// {
//     RowId = Int32.Parse (PlayerItems[0]),
//         Name = PlayerItems[1],
//         Team = PlayerItems[2],
//         GP = Int32.Parse (PlayerItems[3]),
//         PA = Int32.Parse (PlayerItems[4]),
//         HR = Int32.Parse (PlayerItems[5]),
//         R = Int32.Parse (PlayerItems[6]),
//         RBI = Int32.Parse (PlayerItems[7]),
//         SB = Int32.Parse (PlayerItems[8]),
//         BB_percent = Convert.ToDecimal (PlayerItems[9]),
//         K_percent = Convert.ToDecimal (PlayerItems[10]),
//         ISO = Convert.ToDecimal (PlayerItems[11]),
//         BABIP = Convert.ToDecimal (PlayerItems[12]),
//         AVG = Convert.ToDecimal (PlayerItems[13]),
//         OBP = Convert.ToDecimal (PlayerItems[14]),
//         SLG = Convert.ToDecimal (PlayerItems[15]),
//         wOBA = Convert.ToDecimal (PlayerItems[16]),
//         wRC_plus = Int32.Parse (PlayerItems[17]),
//         BsR = Int32.Parse (PlayerItems[18]),
//         Off = Int32.Parse (PlayerItems[19]),
//         Def = Int32.Parse (PlayerItems[20]),
//         WAR = Int32.Parse (PlayerItems[21]),
