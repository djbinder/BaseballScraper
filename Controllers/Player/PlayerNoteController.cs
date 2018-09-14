using System;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models;
using BaseballScraper.Models.Configuration;
using BaseballScraper.Models.Player;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Collections.Generic;

namespace BaseballScraper.Controllers.Player
{
    #pragma warning disable CS0414, CS0219
    // [Route("player")]
    public class PlayerNoteController: Controller
    {
        private Helpers _h = new Helpers();
        private BaseballScraperContext _context;
        private readonly AirtableConfiguration _airtableConfig;



        public PlayerNoteController(BaseballScraperContext context, IOptions<AirtableConfiguration> airtableConfig)
        {
            _airtableConfig = airtableConfig.Value;
            _context        = context;
        }


        [HttpGet]
        [Route("player/viewform")]
        public IActionResult ViewForm()
        {
            _h.StartMethod();
            _h.CompleteMethod();
            return View("playernote");
        }


        [HttpGet]
        [Route("player/list")]
        public void GetPlayerList()
        {
            _h.StartMethod();
            List<PlayerNote> notesList = _context.PlayerNotes.ToList();
            var  notesCount            = notesList.Count();
            Console.WriteLine($"Notes Count: {notesCount}");
            _h.CompleteMethod();
        }

        [HttpGet]
        [Route("player/CreatePlayerNote")]
        public void CreatePlayerNote(PlayerNote note)
        {
            _h.StartMethod();

            // Console.WriteLine($"Name: {note.PlayerName}");
            // Console.WriteLine($"Position: {note.Position}");
            // Console.WriteLine($"Position Type: {note.PositionType}");

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
                MlbSeason    = 2018,
            };

            _context.Add(newPlayerNote);
            _context.SaveChanges();

            // DbConnector.Execute($"INSERT INTO PlayerNotes (PlayerName, Position, PositionType, Note, NoteTone, SourceSite, NoteWriter, CalendarYear, MlbSeason) VALUES ('{note.PlayerName}', '{note.Position}', '{note.PositionType}', '{note.Note}', '{note.NoteTone}', '{note.SourceSite}', '{note.NoteWriter}', '{note.CalendarYear}', '{note.MlbSeason}')");

            _h.CompleteMethod();
        }




    }
}