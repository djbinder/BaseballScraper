using System;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models;
using BaseballScraper.Models.Configuration;
// using BaseballScraper.Models.Player;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Collections.Generic;

using System.Data;
using System.Text.Encodings.Web;
using Dapper;


namespace BaseballScraper.Controllers
{
    // #pragma warning disable CS0414, CS0219
    [Route("playernote")]
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
        [Route("")]
        public IActionResult PlayerNoteMain()
        {
            string content = "player note main content";
            return Content(content);
        }


        [HttpGet("Create")]
        public IActionResult Create()
        {
            _h.StartMethod();
            _h.CompleteMethod();
            return View("playernote");
        }


        [HttpGet]
        [Route("list")]
        public void GetPlayerList()
        {
            List<PlayerNote> notesList = _context.PlayerNotes.ToList();
            var  notesCount            = notesList.Count();
            Console.WriteLine($"Notes Count: {notesCount}");
        }

        [HttpPost]
        [Route("CreateNote")]
        public IActionResult CreateNote(PlayerNote note)
        {
            _h.StartMethod();

            Console.WriteLine(note);
            Console.WriteLine($"Name: {note.PlayerName}");
            Console.WriteLine($"Position: {note.Position}");
            Console.WriteLine($"Position Type: {note.PositionType}");
            Console.WriteLine($"Note: {note.Note}");

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
                Season    = 2018,
            };

            _context.Add(newPlayerNote);
            _context.SaveChanges();

            _h.CompleteMethod();

            return RedirectToAction("PlayerNoteMain");
        }
    }
}