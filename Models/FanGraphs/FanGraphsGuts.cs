namespace BaseballScraper.Models.FanGraphs
{
    // https://www.fangraphs.com/guts.aspx?type=pfh&teamid=0&season=2017
    public class FanGraphsGuts
    {
        public class wObaFipConstants
        {
            public int Season { get; set; }
            public int wOba { get; set; }
            public int wObaScale { get; set;}
            public int wBb { get; set; }
            public int wHbp { get; set; }
            public int w1B { get; set; }
            public int w2B { get; set; }
            public int w3B { get; set; }
            public int wHr { get; set; }
            public int RunSb { get; set; }
            public int RunCs { get; set; }
            public int RPa{ get; set; }
            public int RW { get; set; }
            public int cFip { get; set; }
        }


        public class ParkFactors
        {
            public int Season { get; set; }
            public string Team { get; set; }
            public int Basic5yr { get; set; }
            public int Yr3 { get; set; }
            public int Yr1 { get; set; }
            public int Pf1B { get; set; }
            public int Pf2B { get; set; }
            public int Pf3B { get; set; }
            public int PfHr { get; set; }
            public int PfSO { get; set; }
            public int PfBb { get; set; }
            public int PfGb { get; set; }
            public int PfFb { get; set; }
            public int PfLd { get; set; }
            public int PfIffb { get; set; }
            public int PfFip { get; set; }

        }


        public class HandedNessParkFactors
        {
            public int Season { get; set; }
            public string Team { get; set; }
            public int Hpf1BasL { get; set; }
            public int Hpf1BasR { get; set; }
            public int Hpf2BasL { get; set; }
            public int Hpf2BasR { get; set; }
            public int Hpf3BasL { get; set; }
            public int Hpf3BasR { get; set; }
            public int HpfHRasL { get; set; }
            public int HpfHRasR { get; set; }
        }
    }
}