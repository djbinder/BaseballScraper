using System;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models;
using BaseballScraper.Models.ConfigurationModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Collections.Generic;
using BaseballScraper.Models.Player;


#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.PlayerControllers
{
    [Route("api/player/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PlayerNoteController: Controller
    {
        private readonly Helpers _helpers;
        private readonly BaseballScraperContext _context;
        private readonly AirtableConfiguration _airtableConfig;

        public PlayerNoteController(Helpers helpers, BaseballScraperContext context, IOptions<AirtableConfiguration> airtableConfig)
        {
            _helpers = helpers;
            _airtableConfig = airtableConfig.Value;
            _context        = context;
        }


        /*
            https://127.0.0.1:5001/playernote/test
        */
        [HttpGet("test")]
        public void TestController()
        {
            _helpers.StartMethod();
        }


        // STATUS [ July 23, 2019 ]: this works but still lots to do
        [HttpPost("create_note")]
        public IActionResult CreateNote(PlayerNote playerNote)
        {
            PrintPlayerNoteDetails(playerNote);

            _context.Add(playerNote);
            _context.SaveChanges();

            return Ok(playerNote);
        }


        [HttpGet("player_note_form")]
        public IActionResult ViewPlayerNoteForm()
        {
            Console.WriteLine("PlayerNoteForm");
            return View("PlayerNoteForm");
        }


        // [HttpGet]
        // [Route("list")]
        // public void GetPlayerList()
        // {
        //     _helpers.StartMethod();
        //     List<PlayerNote> notesList = _context.PlayerNotes.ToList();
        //     var  notesCount            = notesList.Count();
        //     Console.WriteLine($"Notes Count: {notesCount}");
        // }


        public PlayerNote CreateDummyNote()
        {
            PlayerNote newPlayerNote = new PlayerNote
            {
                PlayerName   = "Pj",
                Position     = "SS",
                PositionType = "H",
                Note         = "a note for Pj",
                NoteTone     = "positive",
                SourceSite   = "google",
                NoteWriter   = "eno sarris",
                CalendarYear = 2018,
                Season       = 2018,
            };
            return newPlayerNote;
        }



        public void PrintPlayerNoteDetails(PlayerNote playerNote)
        {
            Console.WriteLine($"\nNAME: {playerNote.PlayerName}\t POS: {playerNote.Position}\t TYPE: {playerNote.PositionType}");
            Console.WriteLine($"SOURCE: {playerNote.SourceSite}\t WRITER: {playerNote.NoteWriter}\t TONE: {playerNote.NoteTone}\t NOTE YEAR: {playerNote.CalendarYear}\t SEASON: {playerNote.Season}\n");
            Console.WriteLine("NOTE TEXT: ");
            Console.WriteLine($"{playerNote.Note}\n");
        }
    }
}
