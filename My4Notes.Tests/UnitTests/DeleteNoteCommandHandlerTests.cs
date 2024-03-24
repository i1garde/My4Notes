using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;
using My4Notes.Resources.Commands;

namespace My4Notes.Tests;

public class DeleteNoteCommandHandlerTests
{
    private ApplicationDbContext _context;

    public DeleteNoteCommandHandlerTests()
    {
        // Setup In-Memory Database for testing purposes
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
    }
    
    [Fact]
    public async Task Handle_DeletesNoteCorrectly()
    {
        // Arrange
        var notes = TestData.Notes;

        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        
        var createHandler = new CreateNoteCommandHandler(_context, memoryCache);

        var createCommand = new CreateNoteCommand
        {
            Title = notes[0].Title,
            Text = notes[0].Text,
            CreationDate = notes[0].CreationDate
        };

        var deleteHandler = new DeleteNoteCommandHandler(_context, memoryCache);

        var deleteCommand = new DeleteNoteCommand { Id = 1 };

        // Act
        var createAct = await createHandler.Handle(createCommand, default);
        var deleteAct = await deleteHandler.Handle(deleteCommand, default);

        // Assert
        Assert.Equal(createAct.Id, deleteAct.Id);
    }

    [Fact]
    public async Task Handle_ReturnsDefaultWhenNoteDoesNotExist()
    {
        // Arrange
        var notes = TestData.Notes;

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var handler = new DeleteNoteCommandHandler(_context, memoryCache);

        var command = new DeleteNoteCommand { Id = 1 };

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_RemovesCorrectCacheEntries()
    {
        // Arrange
        var notes = TestData.Notes;

        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        memoryCache.Set("notesList", notes);
        memoryCache.Set("notesCount", notes.Count);

        var createHandler = new CreateNoteCommandHandler(_context, memoryCache);

        var createCommand = new CreateNoteCommand
        {
            Title = notes[0].Title,
            Text = notes[0].Text,
            CreationDate = notes[0].CreationDate
        };
        
        var deleteHandler = new DeleteNoteCommandHandler(_context, memoryCache);

        var deleteCommand = new DeleteNoteCommand { Id = 1 };

        // Act
        var createAct = await createHandler.Handle(createCommand, default);
        var deleteAct = await deleteHandler.Handle(deleteCommand, default);

        // Assert
        Assert.False(memoryCache.TryGetValue("notesList", out List<Note> _));
        Assert.False(memoryCache.TryGetValue("notesCount", out int _));
    }
}