using System;
using System.Collections.Generic;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.Lahman;
using Microsoft.AspNetCore.Mvc;
using RDotNet;
using C = System.Console;


#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE1006
namespace BaseballScraper.Controllers.LahmanControllers
{
    [Route("api/lahman/[controller]")]
    [ApiController]
    public class LahmanMasterTableController : ControllerBase
    {
        private readonly Helpers _helpers;

        private readonly RdotNetConnector _r;

        public LahmanMasterTableController(Helpers helpers, RdotNetConnector r)
        {
            _helpers = helpers;
            _r = r;
        }

        public LahmanMasterTableController(){}



        /***********************************************************************************

            IMPORTANT: See 'RdotNetConnector.cs' for steps to initiate R environment
                * there are 3 steps to start the environment; all executed in the terminal
                * if you don't do this, the methods below will NOT work

        ************************************************************************************/



        /*
            https://127.0.0.1:5001/api/lahman/lahmanmastertable/test
        */
        [HttpGet("test")]
        public void TestLahmanMasterTableController()
        {
            _helpers.StartMethod();
            // GetLahmanMasterTable();
            GetPlayerFromLahmanMasterTable("Anthony", "Rizzo");
        }


        // STATUS [ July 9, 2019 ] : this works but is it needed?
        public void EvaluateAllRecordsInLahmanMasterTable()
        {
            var engine = _r.CreateNewREngine();
            engine.Evaluate("library(Lahman)");
            engine.Evaluate("data(Master)");
            engine.Evaluate("data(Batting)");
            engine.Evaluate("Master$name <- paste(Master$nameFirst, Master$nameLast, sep=' ')");
            var masterName = engine.Evaluate("Master$name <- paste(Master$nameFirst, Master$nameLast, sep=' ')");
        }


        #region GET PLAYER FROM LAHMAN MASTER TABLE - PRIMARY METHODS ------------------------------------------------------------


            // STATUS [ July 9, 2019 ] : this works but seems like it's not optimal
            public LahmanMasterTablePlayer GetPlayerFromLahmanMasterTable(string firstName, string lastName)
            {
                var engine = _r.CreateNewREngine();
                    engine.Evaluate("library(Lahman)");
                    engine.Evaluate("data(Master)");
                    engine.Evaluate("data(Batting)");

                firstName = FormatSearchStringsForR(firstName);
                lastName = FormatSearchStringsForR(lastName);
                var statementToEvaluate = $"subset(Master, nameLast=={lastName} & nameFirst=={firstName})";
                engine.Evaluate(statementToEvaluate);

                var evaluationDataFrame = engine.Evaluate(statementToEvaluate).AsDataFrame();
                    var columnCount = evaluationDataFrame.ColumnCount;
                    var columnHeaders = GetColumnNames(evaluationDataFrame);

                Dictionary<string, object> keysAndValues = new Dictionary<string, object>();

                for(var columnCounter = 0; columnCounter <= columnCount - 1; columnCounter++)
                {
                    var playerDataValue = evaluationDataFrame[columnCounter].AsCharacter().ToArray().GetValue(0);
                    var columnName = columnHeaders[columnCounter];
                    keysAndValues.Add(columnName, playerDataValue);
                }
                var player = CreateLahmanMasterTablePlayerInstance(keysAndValues);
                return player;
            }



            // STATUS [ July 9, 2019 ] : this works
            public LahmanMasterTablePlayer CreateLahmanMasterTablePlayerInstance(Dictionary<string, object> keysAndValues)
            {
                var player = new LahmanMasterTablePlayer
                {
                    LahmanPlayerId            =keysAndValues["playerID"].ToString(),
                    BirthYear                 =Convert.ToInt32(keysAndValues["birthYear"]),
                    BirthMonth                =Convert.ToInt32(keysAndValues["birthMonth"]),
                    BirthDay                  =Convert.ToInt32(keysAndValues["birthDay"]),
                    BirthCountry              =keysAndValues["birthCountry"].ToString(),
                    BirthState                =keysAndValues["birthState"].ToString(),
                    BirthCity                 =keysAndValues["birthCity"].ToString(),
                    DeathYear                 =ManageNullInt(keysAndValues["deathYear"]),
                    DeathMonth                =ManageNullInt(keysAndValues["deathMonth"]),
                    DeathDay                  =ManageNullInt(keysAndValues["deathDay"]),
                    DeathCountry              =ManageNullString(keysAndValues["deathCountry"]),
                    DeathState                =ManageNullString(keysAndValues["deathState"]),
                    DeathCity                 =ManageNullString(keysAndValues["deathCity"]),
                    FirstName                 =keysAndValues["nameFirst"].ToString(),
                    LastName                  =keysAndValues["nameLast"].ToString(),
                    NameFirstLast             =keysAndValues["nameGiven"].ToString(),
                    Weight                    =Convert.ToInt32(keysAndValues["weight"]),
                    Height                    =Convert.ToInt32(keysAndValues["height"]),
                    Bats                      =keysAndValues["bats"].ToString(),
                    Throws                    =keysAndValues["throws"].ToString(),
                    Debut                     =DateTime.Parse(keysAndValues["debut"].ToString()),
                    FinalGame                 =DateTime.Parse(keysAndValues["finalGame"].ToString()),
                    RetroPlayerId             =keysAndValues["retroID"].ToString(),
                    BaseballReferencePlayerId =keysAndValues["bbrefID"].ToString(),
                    // DeathDate                 =DateTime.Parse(keysAndValues["deathDate"].ToString()),
                    // BirthDate                 =DateTime.Parse(keysAndValues["birthDate"].ToString()),
                };
                return player;
            }

        #endregion GET PLAYER FROM LAHMAN MASTER TABLE - PRIMARY METHODS ------------------------------------------------------------





        #region GET PLAYER FROM LAHMAN MASTER TABLE - SUPPORT METHODS ------------------------------------------------------------

            // STATUS [ July 9, 2019 ] : this works
            // if null string = "NULL"; if truly null otherwise and returns an error
            public string ManageNullString(object obj)
            {
                string str = string.Empty;
                if(obj == null)
                    str = "NULL";
                else
                    str = obj.ToString();

                return str;
            }


            // STATUS [ July 9, 2019 ] : this works
            // if null int = 0; if truly null otherwise and returns an error
            public int ManageNullInt(object obj)
            {
                int numCheck;
                if(obj == null)
                    numCheck = 0;
                else
                    numCheck = Convert.ToInt32(obj);

                return numCheck;
            }


            // STATUS [ July 9, 2019 ] : this works
            // first letter in first or last name must be uppercase
            // there must be quotes around first and last name when added to evaluation string
            public string FormatSearchStringsForR(string str)
            {
                var isFirstLetterCapitalized = char.IsUpper(str, 0);
                if(isFirstLetterCapitalized == false) { str = char.ToUpper(str[0]) + str.Substring(1); }
                str = $"\"{str}\"";
                return str;
            }


            // STATUS [ July 9, 2019 ] : this works
            public string[] GetColumnNames(DataFrame df)
            {
                var columnHeaders = df.ColumnNames;
                return columnHeaders;
            }


        #endregion GET PLAYER FROM LAHMAN MASTER TABLE - SUPPORT METHODS ------------------------------------------------------------





        #region PRINTING PRESS ------------------------------------------------------------

            public void PrintDataFramesColumnsAndRows(DataFrame df)
            {
                var rowCount = df.RowCount;
                var columnCount = df.ColumnCount;
                C.WriteLine($"\nrowCount: {rowCount}\t columnCount: {columnCount}\n");
            }

            public void PrintPlayerSearchQueryInfo(string firstName, string lastName, string statement)
            {
                C.WriteLine($"\n[ SEARCH PARAMETERS ]\n");
                C.WriteLine($"FIRST: {firstName}\t LAST: {lastName}");
                C.WriteLine($"STATEMENT TO EVALUTE: {statement}\n");
            }

            public void PrintDictionaryKeyValuePairs(Dictionary<string, object> keysAndValues)
            {
                foreach (var kvp in keysAndValues)
                {
                    C.WriteLine($"{kvp.Key} -->  {kvp.Value}");
                }
            }

        #endregion PRINTING PRESS ------------------------------------------------------------






    }
}



