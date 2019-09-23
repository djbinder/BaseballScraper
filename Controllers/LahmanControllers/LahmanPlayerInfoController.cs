using System;
using System.Collections.Generic;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.Lahman;
using Microsoft.AspNetCore.Mvc;
using RDotNet;
using C = System.Console;


#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE1006, MA0016
namespace BaseballScraper.Controllers.LahmanControllers
{
    [Route("api/lahman/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class LahmanPlayerInfoController : ControllerBase
    {
        private readonly Helpers _h;

        private readonly RdotNetConnector _r;

        public LahmanPlayerInfoController(Helpers h, RdotNetConnector r)
        {
            _h = h;
            _r = r;
        }

        public LahmanPlayerInfoController(){}



        /***********************************************************************************

            IMPORTANT: See 'RdotNetConnector.cs' for steps to initiate R environment
                * there are 3 steps to start the environment; all executed in the terminal
                * if you don't do this, the methods below will NOT work

        ************************************************************************************/



        /*
            https://127.0.0.1:5001/api/lahman/lahmanplayerinfo/test
        */
        [HttpGet("test")]
        public void TestLahmanController()
        {
            _h.StartMethod();
            var playerInfoList = GetPlayerInfoForAllPlayersWithLastName("judge");
        }




        // STATUS [ July 9, 2019 ] : this works
        /// <summary>
        ///     Get list of Mlb player's Player Id, First Name, Last Name
        /// </summary>
        /// <remarks>
        ///     See: 'playerInfo' section @ http://lahman.r-forge.r-project.org/doc/
        ///     See: https://analyticsrusers.blog/2018/05/31/leverage-r-code-within-net-environments-running-a-cvar-model-in-a-c-applications/
        /// </remarks>
        /// <example>
        ///     var playerInfoList = GetPlayerInfoForAllPlayersWithLastName("rizzo");
        /// </example>
        /// <returns>
        ///     List of LahmanPlayerInfo that includes: Lahman Id, First Name, Last Name
        /// </returns>
        public List<LahmanPlayerInfo> GetPlayerInfoForAllPlayersWithLastName(string lastName)
        {
            _h.StartMethod();

            var engine = _r.CreateNewREngine();
            engine.Evaluate("library(Lahman)");

            CharacterVector lastNameVector = _r.CreateCharVect(engine, lastName);
            engine.SetSymbol("lastNameVector", lastNameVector);
            lastNameVector.Dispose();

            SymbolicExpression lastEval = engine.Evaluate("playerInfo(lastNameVector)");
            DataFrame dataFrame = lastEval.AsDataFrame();

            var rowCount = dataFrame.RowCount;
            List<LahmanPlayerInfo> playerinfoList = new List<LahmanPlayerInfo>();
            for(var indexer = 0; indexer <= rowCount - 1; indexer++)
            {
                var playerInfo = CreateLahmanPlayerInfoInstance(dataFrame, indexer, lastName);
                playerinfoList.Add(playerInfo);
            }
            _h.Dig(playerinfoList);
            // engine.Evaluate("playerInfo(lastNameVector)");
            return playerinfoList;
        }


        // STATUS [ July 9, 2019 ] : this works
        /// <summary>
        ///     Instantiate instance of LahmanPlayerInfo
        /// </summary>
        public LahmanPlayerInfo CreateLahmanPlayerInfoInstance(DataFrame dataFrame, int indexer, string lastName)
        {
            var playerInfo = new LahmanPlayerInfo();

            DynamicVector lahmanIdVector = dataFrame[0];
                string[] lahmanIdVectorArray = lahmanIdVector.AsCharacter().ToArray();
                playerInfo.LahmanId = lahmanIdVectorArray[indexer];

            DynamicVector firstNameVector = dataFrame[1];
                string[] firstNameVectorArray = firstNameVector.AsCharacter().ToArray();
                playerInfo.FirstName = firstNameVectorArray[indexer];

            playerInfo.LastName = lastName;
            return playerInfo;
        }
    }
}


// // string mookieBettsId = "bettsmo01";
// CharacterVector playerIdVector = CreateCharVect(engine, mookieBettsId);
// engine.SetSymbol("playerIdVector", playerIdVector);
