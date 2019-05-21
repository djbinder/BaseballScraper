using System.Collections.Generic;
using System.Threading.Tasks;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models.Lahman;
using Microsoft.AspNetCore.Mvc;

namespace BaseballScraper.Controllers
{

    #region OVERVIEW ------------------------------------------------------------

        /// <summary> Methods to read and get records from the Lahman database / csv files </summary>
        /// <remarks> Files will need to be updated after each Mlb season </remarks>
        /// <remarks> There are many more Lahman csv files; those are not included at this point; </remarks>
        /// <list> RESOURCES
        ///     <item> Lahman Csv File Descriptions
        ///         <description> http://www.seanlahman.com/files/database/readme58.txt </description>
        ///     </item>
        /// </list>
        /// <list> LAHMAN CSV FILES
        ///     <item> Appearances </item>
        ///     <item> Batting </item>
        ///     <item> Parks </item>
        ///     <item> People </item>
        ///     <item> Pitching </item>
        ///     <item> Teams </item>
        /// </list>

    #endregion OVERVIEW ------------------------------------------------------------



    #pragma warning disable CS0414, CS0219, IDE0051, IDE0059, CS1591, IDE0044
    [Route("lahman")]
    public class LahmanController: Controller
    {
        private readonly Helpers _h     = new Helpers();
        private readonly CsvHandler _cH = new CsvHandler();

        private readonly string LahmanAppearancesFilePath = "BaseballData/Lahman/Appearances.csv";
        private readonly string LahmanBattingFilePath     = "BaseballData/Lahman/Batting.csv";
        private readonly string LahmanParksFilePath       = "BaseballData/Lahman/Parks.csv";
        private readonly string LahmanPeopleFilePath      = "BaseballData/Lahman/People.csv";
        private readonly string LahmanPitchingFilePath    = "BaseballData/Lahman/Pitching.csv";
        private readonly string LahmanTeamsFilePath       = "BaseballData/Lahman/Teams.csv";


        #region LAHMAN NON-ASYNC ------------------------------------------------------------

            // STATUS: this works
            /// <summary> Call non-async function to read a lahman csv file </summary>
            /// <remarks> You would only be calling one of the included six functions at a time; all six included below for ease / consolidation </remarks>
            [HttpGet]
            [Route("records")]
            public void RunLahmanFunction()
            {
                _h.StartMethod();
                // GET ALL LAHMAN CSV FILE RECORDS (non-async)
                    // GetAllLahmanCsvFileRecords(LahmanAppearancesFilePath);
                    // GetAllLahmanCsvFileRecords(LahmanBattingFilePath);
                    // GetAllLahmanCsvFileRecords(LahmanParksFilePath);
                    // GetAllLahmanCsvFileRecords(LahmanPeopleFilePath);
                    // GetAllLahmanCsvFileRecords(LahmanPitchingFilePath);
                    // GetAllLahmanCsvFileRecords(LahmanTeamsFilePath);
            }


            // STATUS: this works
            /// <summary> Read a lahman csv file non-async </summary>
            /// <param name="filePath"> One of the six Lahman csv file paths </param>
            public void GetAllLahmanCsvFileRecords(string filePath)
            {
                // _cH.ReadCsvRecords(filePath, typeof(LahmanAppearances), typeof(LahmanAppearancesClassMap));
                // _cH.ReadCsvRecords(filePath, typeof(LahmanBatting), typeof(LahmanBattingClassMap));
                // _cH.ReadCsvRecords(filePath, typeof(LahmanParks), typeof(LahmanParksClassMap));
                // _cH.ReadCsvRecords(filePath, typeof(LahmanPeople), typeof(LahmanPeopleClassMap));
                // _cH.ReadCsvRecords(filePath, typeof(LahmanPitching), typeof(LahmanPitchingClassMap));
                // _cH.ReadCsvRecords(filePath, typeof(LahmanTeams), typeof(LahmanTeamsClassMap));
            }

        #endregion LAHMAN NON-ASYNC ------------------------------------------------------------



        #region LAHMAN ASYNC ------------------------------------------------------------

            // STATUS: this works
            /// <summary> Call async function to read a lahman csv file </summary>
            /// <remarks> You would only be calling one of the included six functions at a time; all six included below for ease / consolidation </remarks>
            [HttpGet]
            [Route("async")]
            public async Task RunLahmanFunctionAsync()
            {
                // GET ALL LAHMAN CSV FILE RECORDS ASYNC
                    await GetAllLahmanCsvFileRecordsAsync(LahmanAppearancesFilePath);
                    // await GetAllLahmanCsvFileRecordsAsync(LahmanBattingFilePath);
                    // await GetAllLahmanCsvFileRecordsAsync(LahmanParksFilePath);
                    // await GetAllLahmanCsvFileRecordsAsync(LahmanPeopleFilePath);
                    // await GetAllLahmanCsvFileRecordsAsync(LahmanPitchingFilePath);
                    // await GetAllLahmanCsvFileRecordsAsync(LahmanTeamsFilePath);
            }


            // STATUS: this works
            /// <summary> Read a lahman csv file async </summary>
            /// <param name="filePath"> One of the six Lahman csv file paths </param>
            public async Task GetAllLahmanCsvFileRecordsAsync(string filePath)
            {
                await _cH.ReadCsvRecordsAsync(filePath, typeof(LahmanAppearances), typeof(LahmanAppearancesClassMap));
                // await _cH.ReadCsvRecordsAsync(filePath, typeof(LahmanBatting), typeof(LahmanBattingClassMap));
                // await _cH.ReadCsvRecordsAsync(filePath, typeof(LahmanParks), typeof(LahmanParksClassMap));
                // await _cH.ReadCsvRecordsAsync(filePath, typeof(LahmanPeople), typeof(LahmanPeopleClassMap));
                // await _cH.ReadCsvRecordsAsync(filePath, typeof(LahmanPitching), typeof(LahmanPitchingClassMap));
                // await _cH.ReadCsvRecordsAsync(filePath, typeof(LahmanTeams), typeof(LahmanTeamsClassMap));
            }


            // TODO: This works but is it necessary? If so, create for the other five Lahman classes/ csv file types
            [Route("async/appearances")]
            // public async Task<IEnumerable<dynamic>> GetAllLahmanAppearancesRecords()
            public async Task GetAllLahmanAppearancesRecords()
            {
                var records = await _cH.ReadCsvRecordsAsync(LahmanAppearancesFilePath, typeof(LahmanAppearances), typeof(LahmanAppearancesClassMap));
                // return records;
            }


            // TODO: This doesn't work; You get an error that the file textReader closes before the CsvHandler function completes; Need to determine if this method is necessary;
            public async Task<IEnumerable<dynamic>> GetAllLahmanAppearancesRecordsEnumerable()
            {
                var records = await _cH.ReadCsvRecordsAsync(LahmanAppearancesFilePath, typeof(LahmanAppearances), typeof(LahmanAppearancesClassMap));
                return records;
            }

        #endregion LAHMAN ASYNC ------------------------------------------------------------



    }
}
