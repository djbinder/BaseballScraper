using System;
using System.Collections.Generic;

namespace BaseballScraper.Models
{
    public class FGHitter
    {
        public string Name { get; set; }
        public string Team { get; set; }
        public string GP { get; set; }
        public string PA { get; set; }

        public string HR { get; set; }
        public string R { get; set; }
        public string RBI { get; set; }
        public string SB { get; set; }
        public string BB_percent { get; set; }
        public string K_percent { get; set; }
        public string ISO { get; set; }
        public string BABIP { get; set; }
        public string AVG { get; set; }
        public string OBP { get; set; }
        public string SLG { get; set; }
        public string wOBA { get; set; }
        public string wRC_plus { get; set; }
        public string BsR { get; set; }
        public string Off { get; set; }
        public string Def { get; set; }
        public string WAR { get; set; }

        public FGHitter () { }
    }

    public class FanGraphsHitter
    {
        private int _rowId;
        private String _name;
        private String _team;
        private int _gP;
        private int _pA;
        private int _hR;
        private int _r;
        private int _rBI;
        private int _sB;
        private decimal _bB_percent;
        private decimal _k_percent;
        private decimal _iSO;
        private decimal _bABIP;
        private decimal _aVG;
        private decimal _oBP;
        private decimal _sLG;
        private decimal _wOBA;
        private int _wRC_plus;
        private float _bsR;
        private float _off;
        private float _def;
        private float _wAR;

        public int RowId { get => _rowId; set => _rowId = value; }
        public string Name { get => _name; set => _name = value; }
        public string Team { get => _team; set => _team = value; }
        public int GP { get => _gP; set => _gP = value; }
        public int PA { get => _pA; set => _pA = value; }
        public int HR { get => _hR; set => _hR = value; }
        public int R { get => _r; set => _r = value; }
        public int RBI { get => _rBI; set => _rBI = value; }
        public int SB { get => _sB; set => _sB = value; }
        public decimal BB_percent { get => _bB_percent; set => _bB_percent = value; }
        public decimal K_percent { get => _k_percent; set => _k_percent = value; }
        public decimal ISO { get => _iSO; set => _iSO = value; }
        public decimal BABIP { get => _bABIP; set => _bABIP = value; }
        public decimal AVG { get => _aVG; set => _aVG = value; }
        public decimal OBP { get => _oBP; set => _oBP = value; }
        public decimal SLG { get => _sLG; set => _sLG = value; }
        public decimal wOBA { get => _wOBA; set => _wOBA = value; }
        public int wRC_plus { get => _wRC_plus; set => _wRC_plus = value; }
        public float BsR { get => _bsR; set => _bsR = value; }
        public float Off { get => _off; set => _off = value; }
        public float Def { get => _def; set => _def = value; }
        public float WAR { get => _wAR; set => _wAR = value; }

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