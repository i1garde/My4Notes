using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;
using My4Notes.Services;

namespace My4Notes.Tests;

public class ServiceUnitTest
{
    private string testConnString = "Host=localhost;Port=5433;Database=My4NotesTestDB;Username=postgres;Password=Str0ngP@ssw0rd";
    
    [Fact]
    public async void Service_GetNotesCountAsync_Test()
    {
        var appDb = new Mock<ApplicationDbContext>();
        
        var note1 = new Note() { Id = 1, Title = "First title", Text = "First text" };
        var note2 = new Note() {
            Id = 2, 
            Title = "Second note", 
            Text = "Second text", 
            CreationDate = DateTimeOffset.Now 
        };
        IList<Note> notes = new List<Note>() { note1, note2 };
        
        appDb.Setup(m => m.Notes)
            .ReturnsDbSet(notes);
        
        var notesService = new NotesService(appDb.Object);

        var notesCount = await notesService.GetNotesCountAsync();

        Assert.Equal(2, notesCount);
    }
    
    [Fact]
    public async void Service_GetNotesAsync_Test()
    {
        var appDb = new Mock<ApplicationDbContext>();
        
        var note1 = new Note() { Id = 1, Title = "First title", Text = "First text" };
        var note2 = new Note() {
            Id = 2, 
            Title = "Second note", 
            Text = "Second text", 
            CreationDate = DateTimeOffset.Now 
        };
        IList<Note> notes = new List<Note>() { note1, note2 };
        
        appDb.Setup(m => m.Notes)
            .ReturnsDbSet(notes);
        
        var notesService = new NotesService(appDb.Object);

        var notesTest = await notesService.GetNotesAsync();

        Assert.Equal(note1.Text, notesTest[0].Text);
        Assert.Equal(note2.Text, notesTest[1].Text);
    }
    
    [Fact]
    public async void Service_GetNotesByTitleAsync_Test()
    {
        var appDb = new Mock<ApplicationDbContext>();
        
        var note1 = new Note() { Id = 1, Title = "First title", Text = "First text" };
        var note2 = new Note() {
            Id = 2, 
            Title = "Second note", 
            Text = "Second text", 
            CreationDate = DateTimeOffset.Now 
        };
        IList<Note> notes = new List<Note>() { note1, note2 };
        
        appDb.Setup(m => m.Notes)
            .ReturnsDbSet(notes);
        
        var notesService = new NotesService(appDb.Object);

        var notesTest = await notesService.GetNotesByTitleAsync("Second");
        Assert.Equal(2, notesTest[0].Id);
    }
    
    [Fact]
    public async Task GetNoteByIdAsync_ReturnsNote()
    {
        int noteId = 1;
        var expectedNote = new Note() { Id = 1, Title = "First title", Text = "First text" };
        var mockDbContext = new Mock<ApplicationDbContext>();
        mockDbContext.Setup(db => db.Notes.FindAsync(noteId)).ReturnsAsync(expectedNote);
        var noteService = new NotesService(mockDbContext.Object);

        var result = await noteService.GetNoteByIdAsync(noteId);

        Assert.Equal(expectedNote, result);
    }
    
    [Fact]
    public async Task GetNoteByIdAsync_ReturnsNote_WhenNoteExists()
    {
        var _context = new ApplicationDbContext(testConnString);
        var noteService = new NotesService(_context);
        var note = new Note { Title = "Test Title", Text = "Test Text" };
        _context.Notes.Add(note);
        await _context.SaveChangesAsync();

        var result = await noteService.GetNoteByIdAsync(note.Id);

        Assert.NotNull(result);
        Assert.Equal(note.Id, result.Id);
        Assert.Equal(note.Title, result.Title);
        Assert.Equal(note.Text, result.Text);
    }

    [Fact]
    public async Task GetNoteByIdAsync_ReturnsNull_WhenNoteDoesNotExist()
    {
        var _context = new ApplicationDbContext(testConnString);
        var noteService = new NotesService(_context);
        var nonExistentId = 999;

        var result = await noteService.GetNoteByIdAsync(nonExistentId);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task AddNoteAsync_AddsNoteToDatabase()
    {
        var _context = new ApplicationDbContext(testConnString);
        var noteService = new NotesService(_context);
        var note = new Note { Title = "Test Title", Text = "Test Text" };

        await noteService.AddNoteAsync(note);

        var result = await _context.Notes.FindAsync(note.Id);
        Assert.NotNull(result);
        Assert.Equal(note.Title, result.Title);
        Assert.Equal(note.Text, result.Text);
    }
    
    [Fact]
    public async Task UpdateNoteAsync_UpdatesNoteInDatabase()
    {
        var _context = new ApplicationDbContext(testConnString);
        var noteService = new NotesService(_context);
        var note = new Note { Title = "Test Title", Text = "Test Text" };
        _context.Notes.Add(note);
        await _context.SaveChangesAsync();

        note.Title = "Updated Title";
        note.Text = "Updated Text";

        await noteService.UpdateNoteAsync(note);

        var result = await _context.Notes.FindAsync(note.Id);
        Assert.NotNull(result);
        Assert.Equal(note.Title, result.Title);
        Assert.Equal(note.Text, result.Text);
    }
}