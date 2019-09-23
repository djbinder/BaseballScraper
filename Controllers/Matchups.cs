using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML.Probabilistic.Distributions;
using Microsoft.ML.Probabilistic.Models;

#pragma warning disable MA0048
namespace BaseballScraper
{
    public class MatchupsController : Controller
    {
        // STATUS [ July 24, 2019 ]: this works
        // See: https://docs.microsoft.com/en-us/dotnet/machine-learning/how-to-guides/matchup-app-infer-net
        [HttpGet("machine_learning")]
        public void GameMatchups()
        {
            // the team number of the winning team for each Yahoo weekly matchup (e.g., 10 = djb)
            int[] winnerData = new[] { 10, 5, 8, 2, 1, 10, 7, 4, 2, 1, 10, 5, 2, 8, 9, 2, 9, 1, 3, 10, 2, 6, 4, 9, 10, 7, 5, 9, 8, 10, 7, 1, 2, 9, 10, 7, 2, 9, 7, 5, 6, 2, 10, 4, 6, 3, 9, 5, 9, 4, 8, 1, 6, 7, 4, 1, 3, 10, 7, 5, 3, 1, 7, 6, 4, 9, 10, 1, 2, 6, 4, 8, 3, 5, 6, 4, 10, 8, 3, 6, 10, 7, 8, 6, 1  };
            // the team number of the losing team for each Yahoo weekly matchup (e.g., 10 = djb)
            int[] loserData  = new[] { 7, 4, 6, 3, 9, 5, 9, 6, 8, 3, 6, 7, 4, 1, 3, 10, 5, 4, 8, 1, 7, 5, 3, 8, 3, 1, 2, 6, 4, 8, 3, 5, 6, 4, 4, 8, 9, 10, 4, 8,3, 1, 7, 5, 8, 2, 1, 10, 7, 6, 2, 3, 10, 5, 2, 8, 9, 2, 6, 9, 8, 10, 2, 5, 3, 8, 3, 7, 5, 9, 8, 10, 7, 3, 6, 9, 4, 7, 5, 1, 9, 4, 5, 3, 2 };

            Console.WriteLine($"winnerData.Count: {winnerData.Length}");
            Console.WriteLine($"loserData.Count: {loserData.Length}");

            Range game   = new Range(winnerData.Length);
            Range player = new Range(winnerData.Concat(loserData).Max() + 1);

            Console.WriteLine($"game.SizeAsInt: {game.SizeAsInt}");
            Console.WriteLine($"player.SizeAsInt: {player.SizeAsInt}");

            VariableArray<double> playerSkills = Variable.Array<double>(player);
            playerSkills[player]               = Variable.GaussianFromMeanAndVariance(6, 9).ForEach(player);

            VariableArray<int> winners = Variable.Array<int>(game);
            VariableArray<int> losers  = Variable.Array<int>(game);


            using (Variable.ForEach(game))
            {
                Variable<double> winnerPerformance = Variable.GaussianFromMeanAndVariance(playerSkills[winners[game]], 1.0);
                Variable<double> loserPerformance  = Variable.GaussianFromMeanAndVariance(playerSkills[losers[game]], 1.0);

                Variable.ConstrainTrue(winnerPerformance > loserPerformance);
            }

            winners.ObservedValue = winnerData;
            losers.ObservedValue  = loserData;

            InferenceEngine inferenceEngine = new InferenceEngine();
            Gaussian[]      inferredSkills  = inferenceEngine.Infer<Gaussian[]>(playerSkills);

            var orderedPlayerSkills = inferredSkills
                .Select((s, i) => new { Player = i, Skill = s })
                .OrderByDescending(ps => ps.Skill.GetMean());


            foreach (var playerSkill in orderedPlayerSkills)
            {
                Console.WriteLine($"Player {playerSkill.Player} skill: {playerSkill.Skill}");
            }
        }
    }
}
