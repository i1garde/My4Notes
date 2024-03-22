using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;
using My4Notes.Services;

namespace My4Notes.Tests;

public class ServiceUnitTest
{
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
}