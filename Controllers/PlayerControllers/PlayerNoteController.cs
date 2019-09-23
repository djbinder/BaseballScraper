using System;
using BaseballScraper.Infrastructure;
using BaseballScraper.Models;
using BaseballScraper.Models.ConfigurationModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Collections.Generic;
using BaseballScraper.Models.Player;
using System.Threading.Tasks;
using C = System.Console;


#pragma warning disable CS0219, CS0414, IDE0044, IDE0052, IDE0059, IDE0060, IDE1006
namespace BaseballScraper.Controllers.PlayerControllers
{
    [Route("player/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PlayerNoteController: Controller
    {
        private readonly Helpers                _helpers;
        private readonly BaseballScraperContext _context;
        private readonly AirtableConfiguration  _airtableConfig;

        public PlayerNoteController(Helpers helpers, BaseballScraperContext context, IOptions<AirtableConfiguration> airtableConfig)
        {
            _helpers        = helpers;
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
        // [HttpPost("create_note")]
        // public IActionResult CreateNote([Bind("PlayerName")] PlayerNote note)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateNote([FromForm] PlayerNote note)
        {
            _helpers.StartMethod();
            _helpers.OpenMethod(1);
            // Console.WriteLine("https://127.0.0.1:5001/player/playernote/create_note");

            if(ModelState.IsValid)
            {
                C.WriteLine("YES > ModelState.IsValid");
                PrintPlayerNoteDetails(note);
                C.WriteLine($"Player Name: {note.PlayerName}");
            }

            else
            {
                C.WriteLine("NO > ModelState.IsValid");

            }
            // var playerNote = CreateDummyNote();
            // PrintPlayerNoteDetails(playerNote);
            // _context.Add(playerNote);
            // _context.SaveChanges();

            return Ok();
        }










        [HttpGet("note_form")]
        public IActionResult ViewPlayerNoteForm()
        {
            C.WriteLine("PlayerNoteController > ViewPlayerNoteForm()");
            C.WriteLine("https://127.0.0.1:5001/player/playernote/note_form");
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
                NoteText         = "a note for Pj",
                NoteTone     = "positive",
                SourceSite   = "google",
                NoteWriter   = "eno sarris",
                CalendarYear = 2018,
                Season       = 2018,
            };
            return newPlayerNote;
        }

        public PlayerNote CreateSimpleDummyNote()
        {
            PlayerNote newPlayerNote = new PlayerNote
            {
                PlayerName   = "Dan",
            };
            return newPlayerNote;
        }



        public void PrintPlayerNoteDetails(PlayerNote playerNote)
        {
            C.WriteLine($"\nNAME: {playerNote.PlayerName}\t POS: {playerNote.Position}\t TYPE: {playerNote.PositionType}");
            C.WriteLine($"SOURCE: {playerNote.SourceSite}\t WRITER: {playerNote.NoteWriter}\t TONE: {playerNote.NoteTone}\t NOTE YEAR: {playerNote.CalendarYear}\t SEASON: {playerNote.Season}\n");
            C.WriteLine("NOTE TEXT: ");
            C.WriteLine($"{playerNote.NoteText}\n");
        }
    }
}








// <form asp-controller="PlayerNote" asp-action="CreateNote" method="POST">

// <div class="form-group">
// <label asp-for="PlayerName" class="control-label"></label>
// <input asp-for="PlayerName" class="form-control" id="PlayerName" name="PlayerName" class="form-control" value="Frank Thomas">
// </div>
//     <p>
//         <input type="submit" value="Create"/>
//     </p>

// </form>




    // <div class="form-group">
    //     <label asp-for="Position" class="control-label"></label>
    //     <input asp-for="Position" class="form-control" value="SS">
    // </div>

    // <div class="form-group">
    //     <label asp-for="PositionType" class="control-label"></label>
    //     <input asp-for="PositionType" class="form-control" value="H">
    // </div>

    // <div class="form-group">
    //     <label asp-for="Note" class="control-label"></label>
    //     <input asp-for="Note" class="form-control" value="this player is great">
    // </div>







// <form asp-controller="PlayerNote" asp-action="CreateNote" method="POST">

// <div class="form-group">
// <label asp-for="PlayerName" class="control-label"></label>
// <input asp-for="PlayerName" class="form-control">
// </div>

// <div class="form-group">
// <label asp-for="Position" class="control-label"></label>
// <input asp-for="Position" class="form-control">
// </div>

// <div class="form-group">
// <label asp-for="PositionType" class="control-label"></label>
// <input asp-for="PositionType" class="form-control">
// </div>

// <div class="form-group">
// <label asp-for="Note" class="control-label"></label>
// <input asp-for="Note" class="form-control">
// </div>

// <div class="form-group">
// <label asp-for="NoteTone" class="control-label"></label>
// <input asp-for="NoteTone" class="form-control">
// </div>

// <div class="form-group">
// <label asp-for="SourceSite" class="control-label"></label>
// <input asp-for="SourceSite" class="form-control">
// </div>

// <div class="form-group">
// <label asp-for="NoteWriter" class="control-label"></label>
// <input asp-for="NoteWriter" class="form-control">
// </div>

// <div class="form-group">
// <label asp-for="CalendarYear" class="control-label"></label>
// <input asp-for="CalendarYear" class="form-control">
// </div>

// <div class="form-group">
// <label asp-for="Season" class="control-label"></label>
// <input asp-for="Season" class="form-control">
// </div>

// <div class="form-group">
// <input type="submit" value="Create" class="btn btn-default" />
// </div>

// </form>