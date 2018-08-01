namespace BaseballScraper.Models.FanGraphs
{
    public class FanGraphsPitcher: Player
    {
        private string _fanGraphsId;
        private string _fanGraphsName;
        private string _fanGraphsTeam;
        private string _sFanGraphsAge;
        private string _sGS;
        private string _sIP;
        private string _sTBF;
        private string _sW;
        private string _sSV;
        private string _sSO;
        private string _sHLD;
        private string _sERA;
        private string _sWHIP;
        private string _sKPercent;
        private string _sBbPercent;
        private string _sKPercentMinusBbPercent;
        private string _sK9;
        private string _sBB9;
        private string _sK9DividedByBB9;
        private string _sBalls;
        private string _sStrikes;
        private string _sPitches;
        private string _sHR9;
        private string _sGbPercent;
        private string _sLdPercent;
        private string _sFbPercent;
        private string _sIfFbPercent;
        private string _sOSwingPercent;
        private string _sOSwingPercentPfx;
        private string _sOSwingPercentPi;
        private string _sZContactPercent;
        private string _sZContactPercentPfx;
        private string _sZContactPercentPi;
        private string _sContactPercent;
        private string _sContactPercentPfx;
        private string _sContactPercentPi;
        private string _sZonePercent;
        private string _sZonePercentPfx;
        private string _sZonePercentPi;
        private string _sFStrikePercent;
        private string _sSwStrPercent;
        private string _sPullPercent;
        private string _sSoftPercent;
        private string _sHardPercent;
        private string _sBABIP;
        private string _sLobPercent;
        private string _sHrDividedByFb;
        private string _sFastballVelocityPfx;
        private string _sEraMinus;
        private string _sFip;
        private string _sFipMinus;
        private string _sEraMinusFip;
        private string _sXFip;
        private string _sXFipMinus;
        private string _sSiera;
        private string _sRsPer9;

        public string FanGraphsId { get => _fanGraphsId; set => _fanGraphsId = value; }
        public string FanGraphsName { get => _fanGraphsName; set => _fanGraphsName = value; }
        public string FanGraphsTeam { get => _fanGraphsTeam; set => _fanGraphsTeam = value; }
        public string sFanGraphsAge { get => _sFanGraphsAge; set => _sFanGraphsAge = value; }
        public string sGS { get => _sGS; set => _sGS = value; }
        public string sIP { get => _sIP; set => _sIP = value; }
        public string sTBF { get => _sTBF; set => _sTBF = value; }
        public string sW { get => _sW; set => _sW = value; }
        public string sSV { get => _sSV; set => _sSV = value; }
        public string sSO { get => _sSO; set => _sSO = value; }
        public string sHLD { get => _sHLD; set => _sHLD = value; }
        public string sERA { get => _sERA; set => _sERA = value; }
        public string sWHIP { get => _sWHIP; set => _sWHIP = value; }
        public string sKPercent { get => _sKPercent; set => _sKPercent = value; }
        public string sBbPercent { get => _sBbPercent; set => _sBbPercent = value; }
        public string sKPercentMinusBbPercent { get => _sKPercentMinusBbPercent; set => _sKPercentMinusBbPercent = value; }
        public string sK9 { get => _sK9; set => _sK9 = value; }
        public string sBB9 { get => _sBB9; set => _sBB9 = value; }
        public string sK9DividedByBB9 { get => _sK9DividedByBB9; set => _sK9DividedByBB9 = value; }
        public string sBalls { get => _sBalls; set => _sBalls = value; }
        public string sStrikes { get => _sStrikes; set => _sStrikes = value; }
        public string sPitches { get => _sPitches; set => _sPitches = value; }
        public string sHR9 { get => _sHR9; set => _sHR9 = value; }
        public string sGbPercent { get => _sGbPercent; set => _sGbPercent = value; }
        public string sLdPercent { get => _sLdPercent; set => _sLdPercent = value; }
        public string sFbPercent { get => _sFbPercent; set => _sFbPercent = value; }
        public string sIfFbPercent { get => _sIfFbPercent; set => _sIfFbPercent = value; }
        public string sOSwingPercent { get => _sOSwingPercent; set => _sOSwingPercent = value; }
        public string sOSwingPercentPfx { get => _sOSwingPercentPfx; set => _sOSwingPercentPfx = value; }
        public string sOSwingPercentPi { get => _sOSwingPercentPi; set => _sOSwingPercentPi = value; }
        public string sZContactPercent { get => _sZContactPercent; set => _sZContactPercent = value; }
        public string sZContactPercentPfx { get => _sZContactPercentPfx; set => _sZContactPercentPfx = value; }
        public string sZContactPercentPi { get => _sZContactPercentPi; set => _sZContactPercentPi = value; }
        public string sContactPercent { get => _sContactPercent; set => _sContactPercent = value; }
        public string sContactPercentPfx { get => _sContactPercentPfx; set => _sContactPercentPfx = value; }
        public string sContactPercentPi { get => _sContactPercentPi; set => _sContactPercentPi = value; }
        public string sZonePercent { get => _sZonePercent; set => _sZonePercent = value; }
        public string sZonePercentPfx { get => _sZonePercentPfx; set => _sZonePercentPfx = value; }
        public string sZonePercentPi { get => _sZonePercentPi; set => _sZonePercentPi = value; }
        public string sFStrikePercent { get => _sFStrikePercent; set => _sFStrikePercent = value; }
        public string sSwStrPercent { get => _sSwStrPercent; set => _sSwStrPercent = value; }
        public string sPullPercent { get => _sPullPercent; set => _sPullPercent = value; }
        public string sSoftPercent { get => _sSoftPercent; set => _sSoftPercent = value; }
        public string sHardPercent { get => _sHardPercent; set => _sHardPercent = value; }
        public string sBABIP { get => _sBABIP; set => _sBABIP = value; }
        public string sLobPercent { get => _sLobPercent; set => _sLobPercent = value; }
        public string sHrDividedByFb { get => _sHrDividedByFb; set => _sHrDividedByFb = value; }
        public string sFastballVelocityPfx { get => _sFastballVelocityPfx; set => _sFastballVelocityPfx = value; }
        public string sEraMinus { get => _sEraMinus; set => _sEraMinus = value; }
        public string sFip { get => _sFip; set => _sFip = value; }
        public string sFipMinus { get => _sFipMinus; set => _sFipMinus = value; }
        public string sEraMinusFip { get => _sEraMinusFip; set => _sEraMinusFip = value; }
        public string sXFip { get => _sXFip; set => _sXFip = value; }
        public string sXFipMinus { get => _sXFipMinus; set => _sXFipMinus = value; }
        public string sSiera { get => _sSiera; set => _sSiera = value; }
        public string sRsPer9 { get => _sRsPer9; set => _sRsPer9 = value; }


    }
}
