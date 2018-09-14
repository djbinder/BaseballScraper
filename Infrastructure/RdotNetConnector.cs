using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RDotNet;

namespace BaseballScraper.Infrastructure
{
    // TODO: everything needs to be converted to async
    /// <summary> Includes various R functions / actions </summary>
    ///
    /// <list> RESOURCES
        /// <item> https://billpetti.github.io/baseballr/data-acquisition-functions/ </item>
        /// <item> https://pitchrx.cpsievert.me </item>
        /// <item> Book: Analyzing Baseball Data With R </item>
        /// <item> http://lahman.r-forge.r-project.org/doc/ </item>
    /// </list>
    ///
    /// <list> PROCESS TO START: three steps needed to get csharp and R to work together; enter these in terminal in succession before attempting to run any of the below
        /// <item> (1) export LD_LIBRARY_PATH=/Library/Frameworks/R.framework/Libraries/:$LD_LIBRARY_PATH </item>
        /// <item> (2) export PATH=/Library/Frameworks/R.framework/Libraries/:$PATH </item>
        /// <item> (3) export R_HOME=/Library/Frameworks/R.framework/Resources </item>
    /// </list>
    ///
    /// <list> REQUIRED R PACKAGES
        /// <item> install.packages("pitchRx") </item>
        /// <item> install.packages("baseballr") </item>
        /// <item> install.packages("Lahman") </item>
    /// </list>


    // R PACKAGES LOCATION:
        // /Library/Frameworks/R.framework/Versions/3.4/Resources/library


    public class RdotNetConnector
    {
        private readonly Helpers _h = new Helpers();


        #region  R SETUP ------------------------------------------------------------

            // STATUS: this works
            /// <summary> Creates a new engine that drives other R functions </summary>
            /// <returns> A new R Engine </returns>
            public REngine CreateNewREngine()
            {
                REngine.SetEnvironmentVariables ();
                REngine engine = REngine.GetInstance ();
                return engine;
            }

            // STATUS: this works
            /// <summary> Imports the baseballr library </summary>
            /// <reference> https://billpetti.github.io/baseballr/data-acquisition-functions/ </reference>
            public void ImportBaseballR()
            {
                REngine engine = CreateNewREngine();
                engine.Evaluate("library(baseballr)");
            }

            public void ImportPitchRx()
            {
                REngine engine = CreateNewREngine();
                engine.Evaluate("library(pitchRx)");
            }

            public void ImportDplyr()
            {
                REngine engine = CreateNewREngine();
                engine.Evaluate("library(dplyr)");
            }

        #endregion  R SETUP ------------------------------------------------------------



        #region LAHMAN ------------------------------------------------------------

            // STATUS: in progress
            // TODO: currently calls to 'Evaluates' in the function; the first is needed for the second to run correctly; need to figure out how to decouple them
            /// <summary> </summary>
            /// <reference> http://lahman.r-forge.r-project.org/doc/ </reference>
            public void GetLahmanBattingStats()
            {
                REngine engine = CreateNewREngine();
                engine.Evaluate("library(plyr)");
                engine.Evaluate("library(Lahman)");

                // get all batting stats for everyone forever
                engine.Evaluate("batting <- battingStats()");
                // get batting stats for a subset
                engine.Evaluate("eligibleHitters <- subset(batting, yearID >=1900 & PA > 450)");
            }

            // STATUS: this works
            /// <summary> Get Mlb player's Player Id, First Name, Last Name</summary>
            /// <reference> http://lahman.r-forge.r-project.org/doc/ </reference>
            public void GetLahmanPlayerInfo(string lastName)
            {
                // for testing
                // string mookieBettsId = "bettsmo01";

                REngine engine = CreateNewREngine();
                engine.Evaluate("library(Lahman)");

                CharacterVector lastNameVector = CreateCharVect(engine, lastName);
                engine.SetSymbol("lastNameVector", lastNameVector);

                // CharacterVector playerIdVector = CreateCharVect(engine, mookieBettsId);
                // engine.SetSymbol("playerIdVector", playerIdVector);

                engine.Evaluate("playerInfo(lastNameVector)");
            }







            public void GetLahmanTeamInfo(string lastName)
            {
                REngine engine = CreateNewREngine();
                engine.Evaluate("library(Lahman)");

                CharacterVector lastNameVector = CreateCharVect(engine, lastName);
                engine.SetSymbol("lastNameVector", lastNameVector);

                engine.Evaluate("teamInfo('CH', extra='park')");
                // engine.Evaluate("playerInfo(lastNameVector)");
            }



        #endregion LAHMAN ------------------------------------------------------------





        // TODO: continue working through available functions within the PitchRx package
        #region  PITCH RX ------------------------------------------------------------

            // STATUS: this works to a point, returns a lot of stuff but then breaks; seems to be an issue with some of the data being returned
                // TODO: test this with more dates to figure out why it breaks; figure out of if there are more things returned if it doesnt' break
            /// <summary> Scrapes and returns a significant amount of data about each individual at bat within a given date range </summary>
            /// <example> _r.GetPitchRxData("2013-06-01", "2013-06-01"); </example>
            /// <param name="startDate"> First date of the range you are looking for (e.g, "2016-04-06") </param>
            /// <param name="endDate"> Last date of the range you are looking for (e.g, "2016-06-21") </param>
            /// <returns>
                /// <list>
                    /// <item> AT BAT: pitcher (id), batter (id), num (pitch number?), b, s, o, start_tfs, start_tfs_zulu, stand, b_height, p_throws, atbat_des, home_team_runs, away_team_runs, url (for XML), inning_side(top OR bottom), inning, next_, event2, batter_name, pitcher_name, gameday_link, date </item>
                    /// <item> ACTION: b, s, o, des, event, tfs_zulu, player (id), pitch (number), event_num, home_team_runs, away_team_runs, url (for XML), inning_side (top OR bottom), inning, next_, num, score, event2, gameday_link </item>
                    /// <item> PITCH: des, id, type (B, S, X), tfs, tfs_zulu, x, y, event_num, sv_id, play_guid, start_speed, end_speed, sz_top, sz_top, pfx_x, pfx_z, px, pz, x0, y0, z0, vx0, vy0, vz0, ax, ay, az, break_y </item>
                /// </list>
            /// </returns>
            /// <reference> https://pitchrx.cpsievert.me </reference>
            public void GetPitchRxData(string startDate, string endDate)
            {
                REngine engine = CreateNewREngine();
                ImportPitchRx();

                CharacterVector startDateVector = CreateCharVect(engine, startDate);
                CharacterVector endDateVector   = CreateCharVect(engine, endDate);

                engine.SetSymbol("startDateVector", startDateVector);
                engine.SetSymbol("endDateVector", endDateVector);

                engine.Evaluate("dat <- scrape(start = startDateVector, end = endDateVector)");
            }

        #endregion  PITCH RX ------------------------------------------------------------



        #region  BASEBALL SAVANT ------------------------------------------------------------

            // STATUS: this works
            /// <summary> Get statcast data for individual games between a start and end date </summary>
            /// <example> _r.ScrapeBaseballSavantStatcast("2016-04-06", "2016-04-15", 592789); </example>
            /// <param name="startDate"> First date of the range you are looking for (e.g, "2016-04-06") </param>
            /// <param name="endDate"> Last date of the range you are looking for (e.g, "2016-06-21") </param>
            /// <param name="playerId"> The player's MLBAMID you are looking for </param>
            ///     <see cref="RdotNetConnector.SearchForPlayer(string)"/> to get a player's playerId
            /// <returns> pitch_type, game_date, release_speed, release_pos_x, release_pos_z, player_name, and many more </returns>
            /// <reference> https://billpetti.github.io/baseballr/data-acquisition-functions/ </reference>
            public void ScrapeBaseballSavantStatcast(string startDate, string endDate, int playerId)
            {
                // for testing purposes
                // int correaId      = 621043;
                // int syndergaardId = 592789;

                Console.WriteLine($"Start: {startDate}");
                Console.WriteLine($"End: {endDate}");
                Console.WriteLine($"Player Id: {playerId}");

                REngine engine = CreateNewREngine();
                ImportBaseballR();

                CharacterVector startDateVector = CreateCharVect(engine, startDate);
                CharacterVector endDateVector   = CreateCharVect(engine, endDate);
                NumericVector   playerIdVector  = CreateNumVect(engine, playerId);

                engine.SetSymbol("startDateVector", startDateVector);
                engine.SetSymbol("endDateVector", endDateVector);
                engine.SetSymbol("playerIdVector", playerIdVector);

                engine.Evaluate("scrapeSavant <- scrape_statcast_savant(start_date = startDateVector, end_date = endDateVector, playerid = playerIdVector)");
            }

        #endregion  BASEBALL SAVANT ------------------------------------------------------------



        #region  FANGRAPHS HITTER LEADERBOARD ------------------------------------------------------------

            // STATUS: this works
            /// <summary> OPTION 1A --> Retrieve FanGraphs hitter leader board for:  SINGLE SEASON | ALL MLB | QUALIFIED or UNQUALIFIED </summary>
            /// <example> _r.GetFanGraphsHitterLeaderboard(2017, "y"); </example>
            /// <param name="year"> The year of the season you want to get the leaders for </param>
            /// <param name="qual"> Whether to include only batters that were qualified. Defaults to 'y'. Alternatively, you can pass a minimum number of plate appearances to restrict the data to. </param>
            /// <reference> https://billpetti.github.io/baseballr/data-acquisition-functions/ </reference>
            public void GetFanGraphsHitterLeaderboard(int year, string qual)
            {
                REngine engine = CreateNewREngine();
                ImportBaseballR();

                NumericVector   yearVector       = CreateNumVect(engine, year);
                CharacterVector qualifyingVector = CreateCharVect(engine, qual);

                engine.SetSymbol("yearVector", yearVector);
                engine.SetSymbol("qualifyingVector", qualifyingVector);

                engine.Evaluate("fgHittersLeaderBoard <- fg_bat_leaders(x = yearVector, y = yearVector, league = 'all', qual = qualifyingVector, ind = 0)");
            }

            // STATUS: this works
            /// <summary> OPTION 1B --> Retrieve FanGraphs hitter leader board for:  SINGLE SEASON | ALL MLB | players with PLATE APPEARANCES greater than defined # </summary>
            /// <example> _r.GetFanGraphsHitterLeaderboard(2018, 200); </example>
            /// <param name="year"> The year of the season you want to get the leaders for </param>
            /// <param name="minPlateAppearances"> Number of plate appearances a hitter needs to show up in the results. </param>
            /// <reference> https://billpetti.github.io/baseballr/data-acquisition-functions/ </reference>
            public void GetFanGraphsHitterLeaderboard(int year, int minPlateAppearances)
            {
                REngine engine = CreateNewREngine();
                ImportBaseballR();

                NumericVector yearVector                = CreateNumVect(engine, year);
                NumericVector minPlateAppearancesVector = CreateNumVect(engine, minPlateAppearances);

                engine.SetSymbol("yearVector", yearVector);
                engine.SetSymbol("minPlateAppearancesVector", minPlateAppearancesVector);

                engine.Evaluate("fgHittersLeaderBoard <- fg_bat_leaders(x = yearVector, y = yearVector, league = 'all', qual = minPlateAppearancesVector, ind = 0)");
            }

            // STATUS: this works
            /// <summary> OPTION 2 --> Retrieve FanGraphs hitter leader board for:  SINGLE SEASON | ALL or AL or NL | QUALIFIED or UNQUALIFIED </summary>
            /// <example> _r.GetFanGraphsHitterLeaderboard(2017, "nl", "y"); </example>
            /// <param name="year"> The year of the season you want to get the leaders for </param>
            /// <param name="league">  You can get records for all of MLB ('all'), the National League ('nl'), or the American League ('al') </param>
            /// <param name="qual"> Whether to include only batters that were qualified. Defaults to 'y'. Alternatively, you can pass a minimum number of plate appearances to restrict the data to. </param>
            /// <reference> https://billpetti.github.io/baseballr/data-acquisition-functions/ </reference>
            public void GetFanGraphsHitterLeaderboard(int year, string league, string qual)
            {
                REngine engine = CreateNewREngine();
                ImportBaseballR();
                NumericVector   yearVector       = CreateNumVect(engine, year);
                CharacterVector leagueVector     = CreateCharVect(engine, league);
                CharacterVector qualifyingVector = CreateCharVect(engine, qual);

                engine.SetSymbol("yearVector", yearVector);
                engine.SetSymbol("leagueVector", leagueVector);
                engine.SetSymbol("qualifyingVector", qualifyingVector);

                engine.Evaluate("fgHittersLeaderBoard <- fg_bat_leaders(x = yearVector, y = yearVector, league = leagueVector, qual = qualifyingVector, ind = 0)");
            }

            // STATUS: this works
            /// <summary> OPTION 3A --> Retrieve FanGraphs hitter leader board for:  RANGE OF SEASON | ALL or AL or NL | QUALIFIED or UNQUALIFIED | display in AGGREGATE or by SEASON </summary>
            /// </example> _r.GetFanGraphsHitterLeaderboard(2014, 2017, "nl", "y", "season"); </example>
            /// <param name="startYear"> The first year / season you want to get the leaders for </param>
            /// <param name="endYear"> The last year / season you want to get the leaders for </param>
            /// <param name="league">  You can get records for all of MLB ('all'), the National League ('nl'), or the American League ('al') </param>
            /// <param name="qual"> Whether to include only batters that were qualified. Defaults to 'y'. Alternatively, you can pass a minimum number of plate appearances to restrict the data to. </param>
            /// <param name="statDisplayType"> Whether to split the data by batter and individual season, or to simply aggregate by batter across the seasons selected. Defaults to aggregating (ind = 0). To split by season, use ind = 1 </param>
            /// <reference> https://billpetti.github.io/baseballr/data-acquisition-functions/ </reference>
            public void GetFanGraphsHitterLeaderboard(int startYear, int endYear, string league, string qual, string statDisplayType)
            {
                REngine engine = CreateNewREngine();
                ImportBaseballR();
                NumericVector   startYearVector  = CreateNumVect(engine, startYear);
                NumericVector   endYearVector    = CreateNumVect(engine, endYear);
                CharacterVector leagueVector     = CreateCharVect(engine, league);
                CharacterVector qualifyingVector = CreateCharVect(engine, qual);

                if(statDisplayType == "aggregate")
                {
                    int           aggregateInt      = 0;
                    NumericVector statDisplayVector = CreateNumVect(engine, aggregateInt);
                    engine.SetSymbol("statDisplayType", statDisplayVector);
                }

                if(statDisplayType == "season")
                {
                    int           aggregateInt      = 1;
                    NumericVector statDisplayVector = CreateNumVect(engine, aggregateInt);
                    engine.SetSymbol("statDisplayType", statDisplayVector);
                }

                engine.SetSymbol("startYearVector", startYearVector);
                engine.SetSymbol("endYearVector", endYearVector);
                engine.SetSymbol("leagueVector", leagueVector);
                engine.SetSymbol("qualifyingVector", qualifyingVector);

                engine.Evaluate("fgHittersLeaderBoard <- fg_bat_leaders(x = startYearVector, y = endYearVector, league = leagueVector, qual = qualifyingVector, ind = statDisplayType)");
            }

            // STATUS: this works
            /// <summary> OPTION 3B --> Retrieve FanGraphs hitter leader board for:  RANGE OF SEASON | ALL or AL or NL | players with PLATE APPEARANCES greater than defined # | display in AGGREGATE or by SEASON </summary>
            /// </example> _r.GetFanGraphsHitterLeaderboard(2014, 2017, "all", 200, "aggregate"); </example>
            /// <param name="startYear"> The first year / season you want to get the leaders for </param>
            /// <param name="endYear"> The last year / season you want to get the leaders for </param>
            /// <param name="league">  You can get records for all of MLB ('all'), the National League ('nl'), or the American League ('al') </param>
            /// <param name="minPlateAppearances"> Whether to include only batters that were qualified. Defaults to 'y'. Alternatively, you can pass a minimum number of plate appearances to restrict the data to. </param>
            /// <param name="statDisplayType"> Whether to split the data by batter and individual season, or to simply aggregate by batter across the seasons selected. Defaults to aggregating (ind = 0). To split by season, use ind = 1 </param>
            /// <reference> https://billpetti.github.io/baseballr/data-acquisition-functions/ </reference>
            public void GetFanGraphsHitterLeaderboard(int startYear, int endYear, string league, int minPlateAppearances, string statDisplayType)
            {
                REngine engine = CreateNewREngine();
                ImportBaseballR();
                NumericVector   startYearVector           = CreateNumVect(engine, startYear);
                NumericVector   endYearVector             = CreateNumVect(engine, endYear);
                CharacterVector leagueVector              = CreateCharVect(engine, league);
                NumericVector   minPlateAppearancesVector = CreateNumVect(engine, minPlateAppearances);

                if(statDisplayType == "aggregate")
                {
                    int           aggregateInt      = 0;
                    NumericVector statDisplayVector = CreateNumVect(engine, aggregateInt);
                    engine.SetSymbol("statDisplayType", statDisplayVector);
                }

                if(statDisplayType == "season")
                {
                    int           aggregateInt      = 1;
                    NumericVector statDisplayVector = CreateNumVect(engine, aggregateInt);
                    engine.SetSymbol("statDisplayType", statDisplayVector);
                }

                engine.SetSymbol("startYearVector", startYearVector);
                engine.SetSymbol("endYearVector", endYearVector);
                engine.SetSymbol("leagueVector", leagueVector);
                engine.SetSymbol("minPlateAppearancesVector", minPlateAppearancesVector);

                engine.Evaluate("fgHittersLeaderBoard <- fg_bat_leaders(x = startYearVector, y = endYearVector, league = leagueVector, qual = minPlateAppearancesVector, ind = statDisplayType)");
            }

        #endregion  FANGRAPHS HITTER LEADERBOARD ------------------------------------------------------------



        #region  FANGRAPHS CONSTANTS ------------------------------------------------------------

            // STATUS: this works
            /// <summary> Retrieves components and constants FanGraphs uses for calculating metrics such as wOBA and FIP</summary>
            /// <reference> https://www.fangraphs.com/guts.aspx?type=cn </reference>
            public void GetFanGraphsGuts()
            {
                REngine engine = CreateNewREngine();
                ImportBaseballR();
                engine.Evaluate("fgGuts <- fg_guts()");
            }

            // STATUS: this works
            /// <summary> Retrieves park factors for mlb stadiums for a given year </summary>
            /// <reference> https://www.fangraphs.com/guts.aspx?type=pf&teamid=0&season=2017 </reference>
            public void GetFanGraphsParkFactors(int year)
            {
                REngine engine = CreateNewREngine();
                ImportBaseballR();

                NumericVector yearVector = CreateNumVect(engine, year);
                engine.SetSymbol("year", yearVector);

                engine.Evaluate("fgParkFactors <- fg_park(year)");
            }

            // STATUS: this works
            /// <summary> Retrieves park factors for mlb stadiums for a given year and splits it between righties and lefties </summary>
            /// <remarks> Only available for years 2002 and later </remarks>
            /// <reference> https://www.fangraphs.com/guts.aspx?type=pfh&teamid=0&season=2017 </reference>
            public void GetFanGraphsParkFactorsByBatterHandedness(int year)
            {
                REngine engine = CreateNewREngine();
                ImportBaseballR();

                NumericVector yearVector = CreateNumVect(engine, year);
                engine.SetSymbol("year", yearVector);

                engine.Evaluate("fgParkFactorsByHand <- fg_park_hand(year)");
            }

        #endregion  FANGRAPHS CONSTANTS ------------------------------------------------------------



        #region  BILL PETTI FUNCTIONS ------------------------------------------------------------

            // STATUS: this works
            /// <summary> Search the Chadwich Bureau Register for a mlb player based on their last name </summary>
            /// <example> _r.SearchForPlayer("Seager"); </example>
            /// <param name="lastName"> Mlb player's last name </param>
            /// <returns> first_name, last_name, given_name, name_suffix, nick_name, birth_year, mlb_played_first, mlbam_id, retrosheet_id, retrosheet_id, bbref_id, fangraphs_id
            /// <reference> https://billpetti.github.io/baseballr/data-acquisition-functions/ </reference>
            public void SearchForPlayer(string lastName)
            {
                REngine engine = CreateNewREngine();
                ImportBaseballR();

                CharacterVector cvLastName = CreateCharVect(engine, lastName);
                engine.SetSymbol("lastName", cvLastName);

                engine.Evaluate("player <- playerid_lookup(lastName)");
            }

            // STATUS: this works
            /// <summary> Get mlb standings on a specific date for a particular league (AL or NL) and division </summary>
            /// <remarks> Differs from 'GetMlbStandingsFromDateForward' in that the 'from' parameter is FALSE </remarks>
            /// <param name="mlbLeagueAndDivision"> A combination of league and division (e.g, "AL Central") </param>
            ///     <example> If you want league/division do something like "AL Central" </example>
            ///     <example> If you want overall league standings do "AL Overall" </example>
            /// <param name="standingsDate"> The date that you want to view standings for (e.g, "2015-08-01")</param>
            /// <example> _r.GetMlbStandingsOnDate("NL East", "2015-08-01"); </example>
            /// <returns> tm, W, L, W-L%, GB, RS, RA, pythW-L% </returns>
            /// <reference> https://billpetti.github.io/baseballr/data-acquisition-functions/ </reference>
            public void GetMlbStandingsOnDate(string mlbLeagueAndDivision, string standingsDate)
            {
                _h.StartMethod();

                Console.WriteLine($"League/Division: {mlbLeagueAndDivision}");
                Console.WriteLine($"On Date: {standingsDate}");

                REngine engine = CreateNewREngine();
                ImportBaseballR();

                var charVectLeagueAndDivision = CreateCharVect(engine, mlbLeagueAndDivision);
                engine.SetSymbol("leagueAndDivision", charVectLeagueAndDivision);

                var charVectStandingsDate = CreateCharVect(engine, standingsDate);
                engine.SetSymbol("standingsDate", charVectStandingsDate);

                engine.Evaluate("standings <- standings_on_date_bref(date = standingsDate, division = leagueAndDivision, from = FALSE)");
            }

            // STATUS: this works
            /// <summary> Get mlb standings from a specific date moving forward for a particular league (AL or NL) and division </summary>
            /// <remarks> Differs from 'GetMlbStandingsOnDate' in that the 'from' parameter is TRUE </remarks>
            /// <param name="mlbLeagueAndDivision"> A combination of league and division (e.g, "AL Central") </param>
            ///     <example> If you want league/division do something like "AL Central" </example>
            ///     <example> If you want overall league standings do "AL Overall" </example>
            /// <param name="standingsDate"> The date that you want to view standings forward from (e.g, "2015-08-01")</param>
            /// <example> _r.GetMlbStandingsFromDateForward("NL East", "2015-08-01"); </example>
            /// <returns> tm, W, L, W-L%, GB, RS, RA, pythW-L% </returns>
            /// <reference> https://billpetti.github.io/baseballr/data-acquisition-functions/ </reference>
            public void GetMlbStandingsFromDateForward(string mlbLeagueAndDivision, string standingsDate)
            {
                Console.WriteLine($"League/Division: {mlbLeagueAndDivision}");
                Console.WriteLine($"On Date: {standingsDate}");

                REngine engine = CreateNewREngine();
                ImportBaseballR();

                var charVectLeagueAndDivision = CreateCharVect(engine, mlbLeagueAndDivision);
                engine.SetSymbol("leagueAndDivision", charVectLeagueAndDivision);

                var charVectStandingsDate = CreateCharVect(engine, standingsDate);
                engine.SetSymbol("standingsDate", charVectStandingsDate);

                engine.Evaluate("standings <- standings_on_date_bref(date = standingsDate, division = leagueAndDivision, from = TRUE)");
            }

            // STATUS: this works
            /// <summary> The edge_scrape() function allows the user to scrape PITCHf/x data from the GameDay application using Carson Sievert’s pitchRx package and to calculate metrics associated with Edge%. </summary>
            /// <example> _r.GetPlayerEdgePercentage("2015-04-06", "2015-04-07", "pitcher"); </example>
            /// <param name="startDate"> First date of the range you are looking for (e.g, "2016-04-06") </param>
            /// <param name="endDate"> Last date of the range you are looking for (e.g, "2016-06-21") </param>
            /// <param name="pitcherOrBatter"> Position type. Two options: 'pitcher' OR 'batter' </param>
            /// <reference> https://billpetti.github.io/baseballr/data-acquisition-functions/ </reference>
            public void GetPlayerEdgePercentage(string startDate, string endDate, string pitcherOrBatter)
            {
                REngine engine = CreateNewREngine();
                ImportBaseballR();

                CharacterVector startDateVector       = CreateCharVect(engine, startDate);
                CharacterVector endDateVector         = CreateCharVect(engine, endDate);
                CharacterVector pitcherOrBatterVector = CreateCharVect(engine, pitcherOrBatter);


                engine.SetSymbol("startDateVector", startDateVector);
                engine.SetSymbol("endDateVector", endDateVector);
                engine.SetSymbol("pitcherOrBatterVector", pitcherOrBatterVector);

                engine.Evaluate("edgeScrape <- edge_scrape(startDateVector, endDateVector, pitcherOrBatterVector) %>% .[, c(1:3,7:12)] %>% head(10)");
            }

        #endregion  BILL PETTI FUNCTIONS ------------------------------------------------------------



        #region  CALCULATING BASEBALL DATA WITH R ------------------------------------------------------------

            // STATUS: this works
            /// <summary> Get a single pitcher's winning percentage </summary>
            /// <reference> Calculating Baseball Data with R </reference>
            public void CalculatePitcherWinningPercentage()
            {
                REngine engine = CreateNewREngine();

                // PITCHER WINS --> each number represents number of wins for a single season
                NumericVector pitcherWins = engine.CreateNumericVector(new double[] {8, 21, 15, 21, 21, 22, 14});
                engine.SetSymbol("pitcherWins", pitcherWins);

                // PITCHER LOSSES --> each number represents number of losses for a single season
                NumericVector pitcherLosses = engine.CreateNumericVector(new double[] {5, 10, 12, 14, 17, 14, 19});
                engine.SetSymbol("pitcherLosses", pitcherLosses);

                // WINNING PERCENTAGE --> calculate the winning percentage for each single season
                NumericVector winningPercentage = engine.Evaluate("winningPercentage <- 100 * pitcherWins / (pitcherWins + pitcherLosses)").AsNumeric();
            }

        #endregion  CALCULATING BASEBALL DATA WITH R ------------------------------------------------------------



        #region HELPERS ------------------------------------------------------------
            public CharacterVector CreateCharVect(REngine engine, String str)
            {
                CharacterVector cV = engine.CreateCharacterVector(new [] { str });
                return cV;
            }
            public NumericVector CreateNumVect(REngine engine, int num)
            {
                NumericVector nV = engine.CreateNumericVector(new double [] { num });
                return nV;
            }
        #endregion HELPERS ------------------------------------------------------------


    }
}
