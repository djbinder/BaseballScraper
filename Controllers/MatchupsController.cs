using System;
using System.Collections.Generic;
using System.Linq;
using BaseballScraper.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML.Probabilistic.Distributions;
using Microsoft.ML.Probabilistic.Models;
using C = System.Console;

#pragma warning disable MA0048
namespace BaseballScraper
{
    [Route("infer")]
    public class MatchupsController : Controller
    {
        private readonly Helpers _helpers;


        public MatchupsController(Helpers helpers)
        {
            _helpers = helpers;
        }


        /*
            https://127.0.0.1:5001/infer/test
        */
        [HttpGet("test")]
        public void TestController()
        {
            _helpers.StartMethod();
            // RunBaseballInference();
            RunFootballInference();

        }


        public void RunBaseballInference()
        {
            int[] winnerData = BaseballMatchupWinners();
            int[] loserData  = BaseballMatchupLosers();
            RunInference(winnerData, loserData);
        }

        public void RunFootballInference()
        {
            int[] footballWinnerData = FootballMatchupWinners();
            int[] footballLoserData  = FootballMatchupLosers();
            RunInference(footballWinnerData, footballLoserData);
        }


        // STATUS [ July 24, 2019 ]: this works
        // * See: https://docs.microsoft.com/en-us/dotnet/machine-learning/how-to-guides/matchup-app-infer-net
        public void RunInference(int [] winnerData, int[] loserData)
        {
            // * Check if there are equal # of ints in 'winnerData' and 'loserData'
            ConfirmThatWinnerAndLoserLengthsMatch(winnerData, loserData);

            // * Define the statistical model as a probabilistic program
            // * game: the number of Games (i.e., matchups)
            // * player: the number of Players (i.e., managers)
            Range game   = new Range(winnerData.Length);
            Range player = new Range(winnerData.Concat(loserData).Max() + 1);
            PrintGameAndPlayerInfo(game, player);

            // * playerSkills[player] = vdouble[]0[index1]
            // * playerSkills[player].Definition: GaussianFromMeanAndVariance(vdouble1,vdouble2)
            VariableArray<double> playerSkills = Variable.Array<double>(player);
            playerSkills[player]               = Variable.GaussianFromMeanAndVariance(6, 9).ForEach(player);

            VariableArray<int> winners = Variable.Array<int>(game);     // * winners = vint[]0
            VariableArray<int> losers  = Variable.Array<int>(game);     // * losers = vint[]1

            using (Variable.ForEach(game))
            {
                // * The player performance is a noisy version of their skill
                // * winnerPerformance = vdouble8
                // * loserPerformance  = vdouble11
                Variable<double> winnerPerformance = Variable.GaussianFromMeanAndVariance(playerSkills[winners[game]], 1.0);
                Variable<double> loserPerformance  = Variable.GaussianFromMeanAndVariance(playerSkills[losers[game]], 1.0);

                // * The winner performed better in this game
                Variable.ConstrainTrue(winnerPerformance > loserPerformance);
            }

            // * Attach the data to the model
            // * winners.ObservedValue & losers.ObservedValue = System.Int32[]
            // * > Includes all #s in initial data set (e.g., 'winnerData)
            winners.ObservedValue = winnerData;
            losers.ObservedValue  = loserData;

            // * Run inference
            InferenceEngine inferenceEngine = new InferenceEngine();

            // * inferredSkills = Microsoft.ML.Probabilistic.Distributions.Gaussian[] with score for each manager
            // * E.g., 0: Gaussian(9.517, 3.926)
            Gaussian[] inferredSkills  = inferenceEngine.Infer<Gaussian[]>(playerSkills);

            // * The inferred skills are uncertain, which is captured in their variance
            // * orderedPlayerSkills is IOrderedEnumerable<<anonymous type: int Player, Gaussian Skill>>
            // * orderedPlayerSkills is System.Linq.OrderedEnumerable`2[<>f__AnonymousType0`2[System.Int32,Microsoft.ML.Probabilistic.Distributions.Gaussian],System.Double]
            var orderedPlayerSkills = inferredSkills
                .Select((s, i) => new { Player = i, Skill  = s, })
                .OrderByDescending(ps => ps.Skill.GetMean());

            // * playerSkill is <anonymous type: int Player, Gaussian Skill>>
            // * playerSkill.Player is player numbers (0, 1, 2, 3, etc)
            // * playerSkill.Skill is a skill for each player
            // * > E.g., Gaussian(9.517, 3.926), Gaussian(4.955, 3.503)
            orderedPlayerSkills.ToList().ForEach((playerSkill) => C.WriteLine($"Player {playerSkill.Player} skill: {playerSkill.Skill}"));
        }


        public void ConfirmThatWinnerAndLoserLengthsMatch(int[] winnerData, int[] loserData)
        {
            // * Check if there are equal # of ints in 'winnerData' and 'loserData'
            C.WriteLine($"winnerData.Length : {winnerData.Length}");
            C.WriteLine($"loserData.Length  : {loserData.Length}");

            // * Make sure the number of winning teams is same as number of losing teams
            // * Each matchup should have 1 winner and 1 loser
            if(winnerData.Length != loserData.Length)
            {
                C.ForegroundColor = ConsoleColor.Red;
                C.WriteLine($"\nLENGTH OF WINNER AND LOSER DATA DO NOT MATCH\n");
                C.ResetColor();
                throw new Exception();
            }
        }


        public void PrintGameAndPlayerInfo(Range game, Range player)
        {
            // * Equal to # of ints in 'winnerData'
            C.WriteLine($"game.SizeAsInt   : {game.SizeAsInt}");

            // * Equal to number of unique ints between 'winnerData' and 'loserData'
            // * Equal to # of managers + 1
            // * > the +1 is Player 0 (i.e., an average player or replacement level manager)
            C.WriteLine($"player.SizeAsInt : {player.SizeAsInt}");
        }


        public int[] BaseballMatchupWinners()
        {
            // * The team number of the winning team for each Yahoo weekly matchup (e.g., 10 = djb)
            return new[]
            {
                10, 5, 8, 2, 1, // Week 1
                10, 7, 4, 2, 1, // Week 2
                10, 5, 2, 8, 9, // Week 3
                2, 9, 1, 3, 10, // Week 4
                2, 6, 4, 9, 10, // Week 5
                7, 5, 9, 8, 10, // Week 6
                7, 1, 2, 9, 10, // Week 7
                7, 2, 9, 7, 5,  // Week 8
                6, 2, 10, 4, 6, // Week 9
                3, 9, 5, 9, 4,  // Week 10
                8, 1, 6, 7, 4,  // Week 11
                1, 3, 10, 7, 5, // Week 12
                3, 1, 7, 6, 4,  // Week 13
                9, 10, 1, 2, 6, // Week 14
                4, 8, 3, 5, 6,  // Week 15
                4, 10, 8, 3, 6, // Week 16
                10, 7, 8, 6, 1, // Week 17
                10, 5, 6, 3, 9, // Week 18
                5, 7, 6, 8, 1,  // Week 19
                6, 5, 2, 1, 9,  // Week 20
                2, 7, 9, 1, 3,  // Week 21
            };
        }


        public int[] BaseballMatchupLosers()
        {
            // * The team number of the losing team for each Yahoo weekly matchup (e.g., 10 = djb)
            return new[]
            {
                7, 4, 6, 3, 9,  // Week 1
                5, 9, 6, 8, 3,  // Week 2
                6, 7, 4, 1, 3,  // Week 3
                10, 5, 4, 8, 1, // Week 4
                7, 5, 3, 8, 3,  // Week 5
                1, 2, 6, 4, 8,  // Week 6
                3, 5, 6, 4, 4,  // Week 7
                8, 9, 10, 4, 8, // Week 8
                3, 1, 7, 5, 8,  // Week 9
                2, 1, 10, 7, 6, // Week 10
                2, 3, 10, 5, 2, // Week 11
                8, 9, 2, 6, 9,  // Week 12
                8, 10, 2, 5, 3, // Week 13
                8, 3, 7, 5, 9,  // Week 14
                8, 10, 7, 3, 6, // Week 15
                9, 4, 7, 5, 1,  // Week 16
                9, 4, 5, 3, 2,  // Week 17
                7, 4, 8, 2, 1,  // Week 18
                10, 9, 4, 2, 3, // Week 19
                10, 7, 4, 8, 3, // Week 20
                10, 6, 5, 4, 8, // Week 21
            };
        }



        // 1  : Dyatlov
        // 2  : Bang
        // 3  : Calcutta
        // 4  : Your
        // 5  : allmyfriends
        // 6  : Chubbed
        // 7  : Mitch
        // 8  : CB Worldwide
        // 9  : Attack Child
        // 10 : WellThat

        public int[] FootballMatchupWinners()
        {
            return new[]
            {
                2, 4, 5, 8, 10,
                1, 5, 10, 6, 9,
                1, 3, 5, 6, 8 ,
                1, 3, 2, 10, 6,
            };
        }


        public int[] FootballMatchupLosers()
        {
            return new[]
            {
                1, 3, 6, 7, 9 ,
                7, 3, 4, 2, 8 ,
                9, 2, 4, 7, 10,
                8, 7, 4, 5, 9 ,
            };
        }

    }
}





// // * playerSkill is <anonymous type: int Player, Gaussian Skill>>
// // * playerSkill.Player is player numbers (0, 1, 2, 3, etc)
// // * playerSkill.Skill is a skill for each player
// // * > E.g., Gaussian(9.517, 3.926), Gaussian(4.955, 3.503)
// foreach (var playerSkill in orderedPlayerSkills)
// {
//     C.WriteLine($"Player {playerSkill.Player} skill: {playerSkill.Skill}");
// }


// // * The team number of the winning team for each Yahoo weekly matchup (e.g., 10 = djb)
// int[] winnerData = new[]
// {
//     10, 5, 8, 2, 1, // Week 1
//     10, 7, 4, 2, 1, // Week 2
//     10, 5, 2, 8, 9, // Week 3
//     2, 9, 1, 3, 10, // Week 4
//     2, 6, 4, 9, 10, // Week 5
//     7, 5, 9, 8, 10, // Week 6
//     7, 1, 2, 9, 10, // Week 7
//     7, 2, 9, 7, 5,  // Week 8
//     6, 2, 10, 4, 6, // Week 9
//     3, 9, 5, 9, 4,  // Week 10
//     8, 1, 6, 7, 4,  // Week 11
//     1, 3, 10, 7, 5, // Week 12
//     3, 1, 7, 6, 4,  // Week 13
//     9, 10, 1, 2, 6, // Week 14
//     4, 8, 3, 5, 6,  // Week 15
//     4, 10, 8, 3, 6, // Week 16
//     10, 7, 8, 6, 1, // Week 17
//     10, 5, 6, 3, 9, // Week 18
//     5, 7, 6, 8, 1,  // Week 19
//     6, 5, 2, 1, 9,  // Week 20
//     2, 7, 9, 1, 3,  // Week 21
// };

// // the team number of the losing team for each Yahoo weekly matchup (e.g., 10 = djb)
// int[] loserData  = new[]
// {
//     7, 4, 6, 3, 9,  // Week 1
//     5, 9, 6, 8, 3,  // Week 2
//     6, 7, 4, 1, 3,  // Week 3
//     10, 5, 4, 8, 1, // Week 4
//     7, 5, 3, 8, 3,  // Week 5
//     1, 2, 6, 4, 8,  // Week 6
//     3, 5, 6, 4, 4,  // Week 7
//     8, 9, 10, 4, 8, // Week 8
//     3, 1, 7, 5, 8,  // Week 9
//     2, 1, 10, 7, 6, // Week 10
//     2, 3, 10, 5, 2, // Week 11
//     8, 9, 2, 6, 9,  // Week 12
//     8, 10, 2, 5, 3, // Week 13
//     8, 3, 7, 5, 9,  // Week 14
//     8, 10, 7, 3, 6, // Week 15
//     9, 4, 7, 5, 1,  // Week 16
//     9, 4, 5, 3, 2,  // Week 17
//     7, 4, 8, 2, 1,  // Week 18
//     10, 9, 4, 2, 3, // Week 19
//     10, 7, 4, 8, 3, // Week 20
//     10, 6, 5, 4, 8, // Week 21
// };
