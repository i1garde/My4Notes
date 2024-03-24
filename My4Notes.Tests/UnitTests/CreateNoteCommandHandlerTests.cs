using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;
using My4Notes.Resources.Commands;

namespace My4Notes.Tests;

public class CreateNoteCommandHandlerTests
{
    private ApplicationDbContext _context;

    public CreateNoteCommandHandlerTests()
    {
        // Setup In-Memory Database for testing purposes
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
    }
    
    [Fact]
    public async Task Handle_CreatesNoteCorrectly()
    {
        // Arrange
        var notes = TestData.Notes;

        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        
        var handler = new CreateNoteCommandHandler(_context, memoryCache);

        var command = new CreateNoteCommand
        {
            Title = notes[0].Title,
            Text = notes[0].Text,
            CreationDate = notes[0].CreationDate
        };

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        Assert.Equal(command.Title, result.Title);
        Assert.Equal(command.Text, result.Text);
        Assert.Equal(command.CreationDate, result.CreationDate);
    }

    [Fact]
    public async Task Handle_RemovesCorrectCacheEntries()
    {
        // Arrange
        var notes = TestData.Notes;

        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        memoryCache.Set("notesList", notes);
        memoryCache.Set("notesCount", notes.Count);

        var newNote = new Note()
        {
            Title = "New note",
            Text = "Just created note",
            CreationDate = DateTime.UtcNow
        };

        var handler = new CreateNoteCommandHandler(_context, memoryCache);

        var command = new CreateNoteCommand
        {
            Title = newNote.Title,
            Text = newNote.Text,
            CreationDate = newNote.CreationDate
        };

        // Act
        await handler.Handle(command, default);

        // Assert
        Assert.False(memoryCache.TryGetValue("notesList", out List<Note> _));
        Assert.False(memoryCache.TryGetValue("notesCount", out int _));
    }
}