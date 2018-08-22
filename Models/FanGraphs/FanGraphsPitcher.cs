namespace BaseballScraper.Models.FanGraphs
{
    public class FanGraphsPitcher
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
        public string FanGraphsAge { get => _sFanGraphsAge; set => _sFanGraphsAge = value; }
        public string GamesStarted { get => _sGS; set => _sGS = value; }
        public string InningsPitched { get => _sIP; set => _sIP = value; }
        public string TotalBattersFaced { get => _sTBF; set => _sTBF = value; }
        public string Wins { get => _sW; set => _sW = value; }
        public string Saves { get => _sSV; set => _sSV = value; }
        public string Ks { get => _sSO; set => _sSO = value; }
        public string Holds { get => _sHLD; set => _sHLD = value; }
        public string Era { get => _sERA; set => _sERA = value; }
        public string Whip { get => _sWHIP; set => _sWHIP = value; }
        public string StrikeoutPercentage { get => _sKPercent; set => _sKPercent = value; }
        public string WalkPercentage { get => _sBbPercent; set => _sBbPercent = value; }
        public string StrikeoutsMinusWalks { get => _sKPercentMinusBbPercent; set => _sKPercentMinusBbPercent = value; }
        public string StrikeoutsPer9 { get => _sK9; set => _sK9 = value; }
        public string WalksPer9 { get => _sBB9; set => _sBB9 = value; }
        public string StrikeoutsDividedByWalks { get => _sK9DividedByBB9; set => _sK9DividedByBB9 = value; }
        public string Balls { get => _sBalls; set => _sBalls = value; }
        public string Strikes { get => _sStrikes; set => _sStrikes = value; }
        public string Pitches { get => _sPitches; set => _sPitches = value; }
        public string HomeRunsPer9 { get => _sHR9; set => _sHR9 = value; }
        public string GroundballPercent { get => _sGbPercent; set => _sGbPercent = value; }
        public string LinedrivePercent { get => _sLdPercent; set => _sLdPercent = value; }
        public string FlyballPercent { get => _sFbPercent; set => _sFbPercent = value; }
        public string InfieldFlyballPercent { get => _sIfFbPercent; set => _sIfFbPercent = value; }
        public string OSwingPercent { get => _sOSwingPercent; set => _sOSwingPercent = value; }
        public string OSwingPercentPfx { get => _sOSwingPercentPfx; set => _sOSwingPercentPfx = value; }
        public string OSwingPercentPi { get => _sOSwingPercentPi; set => _sOSwingPercentPi = value; }
        public string ZContactPercent { get => _sZContactPercent; set => _sZContactPercent = value; }
        public string ZContactPercentPfx { get => _sZContactPercentPfx; set => _sZContactPercentPfx = value; }
        public string ZContactPercentPi { get => _sZContactPercentPi; set => _sZContactPercentPi = value; }
        public string ContactPercent { get => _sContactPercent; set => _sContactPercent = value; }
        public string ContactPercentPfx { get => _sContactPercentPfx; set => _sContactPercentPfx = value; }
        public string ContactPercentPi { get => _sContactPercentPi; set => _sContactPercentPi = value; }
        public string ZonePercent { get => _sZonePercent; set => _sZonePercent = value; }
        public string ZonePercentPfx { get => _sZonePercentPfx; set => _sZonePercentPfx = value; }
        public string ZonePercentPi { get => _sZonePercentPi; set => _sZonePercentPi = value; }
        public string FStrikePercent { get => _sFStrikePercent; set => _sFStrikePercent = value; }
        public string SwStrPercent { get => _sSwStrPercent; set => _sSwStrPercent = value; }
        public string PullPercent { get => _sPullPercent; set => _sPullPercent = value; }
        public string SoftPercent { get => _sSoftPercent; set => _sSoftPercent = value; }
        public string sHardPercent { get => _sHardPercent; set => _sHardPercent = value; }
        public string Babip { get => _sBABIP; set => _sBABIP = value; }
        public string LeftOnBasePercent { get => _sLobPercent; set => _sLobPercent = value; }
        public string HomerunsDividedByFlyballs { get => _sHrDividedByFb; set => _sHrDividedByFb = value; }
        public string FastballVelocityPfx { get => _sFastballVelocityPfx; set => _sFastballVelocityPfx = value; }
        public string EraMinus { get => _sEraMinus; set => _sEraMinus = value; }
        public string Fip { get => _sFip; set => _sFip = value; }
        public string FipMinus { get => _sFipMinus; set => _sFipMinus = value; }
        public string EraMinusFip { get => _sEraMinusFip; set => _sEraMinusFip = value; }
        public string XFip { get => _sXFip; set => _sXFip = value; }
        public string XFipMinus { get => _sXFipMinus; set => _sXFipMinus = value; }
        public string Siera { get => _sSiera; set => _sSiera = value; }
        public string RunsPer9 { get => _sRsPer9; set => _sRsPer9 = value; }

    }
}
