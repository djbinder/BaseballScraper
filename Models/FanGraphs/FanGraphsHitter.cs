/*

https://www.fangraphs.com/leaders.aspx?pos=all&stats=bat&lg=all&qual=y&type=c%2c3%2c4%2c5%2c6%2c-1%2c12%2c11%2c13%2c21%2c14%2c23%2c-1%2c34%2c35%2c36%2c40%2c41%2c-1%2c45%2c206%2c209%2c211%2c61%2c-1%2c102%2c106%2c110%2c-1%2c289%2c290%2c291%2c292%2c293%2c294%2c295%2c296%2c297%2c298%2c299%2c300%2c301%2c302%2c303%2c304&season=2019&month=0&season1=2019&ind=0&team=0&rost=0&age=0&filter=&players=0&startdate=&enddate=&page=1_50

OR

"https://www.fangraphs.com/leaders.aspx?pos=all&stats=bat&lg=all&qual=y&type=8&season=2018&month=0&season1=2018&ind=0&page=1_50"

*/


using System;
using System.Collections.Generic;

#pragma warning disable CS0219, CS0414, IDE0044, IDE0051, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Models.FanGraphs
{
    public class FanGraphsHitter
    {
        public string FanGraphsId { get; set; }

        public int FanGraphsRowNumber { get; set; }

        public string FanGraphsName { get; set; }

        public string FanGraphsTeam { get; set; }

        public int Age { get; set; }

        public int GamesPlayed { get; set; }

        public int AtBats { get; set; }

        public int PlateAppearances { get; set; }

        public int Runs { get; set; }

        public int HomeRuns { get; set; }

        public int RunsBattedIn { get; set; }

        public int StolenBases { get; set; }

        public int Walks { get; set; }

        public decimal BattingAverage { get; set; }



        public decimal WalkPercentage { get; set; }

        public decimal StrikeoutPercentage { get; set; }

        public decimal WalksPerStrikeout { get; set; }

        public decimal Iso { get; set; }

        public decimal Babip { get; set; }



        public decimal FlyBallPercentage { get; set; }

        public decimal PullPercentage { get; set; }

        public decimal SoftPercentage { get; set; }

        public decimal HardPercentage { get; set; }

        public int wRcPlus { get; set; }



        public decimal OSwingPercentage { get; set; }

        public decimal ZContactPercentage { get; set; }

        public decimal SwingingStrikePercentage { get; set; }



        public int WalkPercentagePlus { get; set; }

        public int StrikeoutPercentagePlus { get; set; }

        public int OnBasePercentagePlus { get; set; }

        public int SluggingPercentagePlus { get; set; }

        public int IsoPlus { get; set; }

        public int BabipPlus { get; set; }

        public int LinedrivePercentagePlus { get; set; }

        public int GroundBallPercentagePlus { get; set; }

        public int FlyBallPercentagePlus { get; set; }

        public int HomeRunPerFlyBallPercentagePlus { get; set; }

        public int PullPercentagePlus { get; set; }

        public int CenterPercentagePlus { get; set; }

        public int OppoPercentagePlus { get; set; }

        public int SoftPercentagePlus { get; set; }

        public int MediumPercentagePlus { get; set; }

        public int HardPercentagePlus { get; set; }





        public decimal OnBasePercentage { get; set; }
        public decimal SluggingPercentage { get; set; }
        public decimal wOba { get; set; }
        public decimal BaseRunningRunsAboveReplacement { get; set; }
        public decimal Offense { get; set; }
        public decimal Defense { get; set; }
        public decimal WinsAboveReplacement { get; set; }

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
