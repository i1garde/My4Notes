using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;
using My4Notes.Resources.Commands;

namespace My4Notes.Tests;

public class UpdateNoteCommandHandlerTests
{
    private ApplicationDbContext _context;

    public UpdateNoteCommandHandlerTests()
    {
        // Setup In-Memory Database for testing purposes
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
    }
    
    [Fact]
    public async Task Handle_UpdatesNoteCorrectly()
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

        string updatedTitle = "Updated Title";
        string updatedText = "Updated Text";

        var updateHandler = new UpdateNoteCommandHandler(_context, memoryCache);

        var updateCommand = new UpdateNoteCommand { Id = notes[0].Id, Title = updatedTitle, Text = updatedText };

        // Act
        var createAct = await createHandler.Handle(createCommand, default);
        var updateAct = await updateHandler.Handle(updateCommand, default);

        // Assert
        Assert.Equal(createAct.Id, updateAct.Id);
        Assert.Equal(updatedTitle, updateAct.Title);
        Assert.Equal(updatedText, updateAct.Text);
    }

    [Fact]
    public async Task Handle_ReturnsDefaultWhenNoteDoesNotExist()
    {
        // Arrange
        var notes = TestData.Notes;

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        string updatedTitle = "Updated Title";
        string updatedText = "Updated Text";

        var updateHandler = new UpdateNoteCommandHandler(_context, memoryCache);

        var updateCommand = new UpdateNoteCommand { Id = notes[0].Id, Title = updatedTitle, Text = updatedText };

        // Act
        var updateAct = await updateHandler.Handle(updateCommand, default);

        // Assert
        Assert.Null(updateAct);
    }

    [Fact]
    public async Task Handle_RemovesCorrectCacheEntry()
    {
        // Arrange
        var notes = TestData.Notes;

        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        memoryCache.Set("notesList", notes);

        var createHandler = new CreateNoteCommandHandler(_context, memoryCache);

        var createCommand = new CreateNoteCommand
        {
            Title = notes[0].Title,
            Text = notes[0].Text,
            CreationDate = notes[0].CreationDate
        };

        string updatedTitle = "Updated Title";
        string updatedText = "Updated Text";

        var updateHandler = new UpdateNoteCommandHandler(_context, memoryCache);

        var updateCommand = new UpdateNoteCommand { Id = notes[0].Id, Title = updatedTitle, Text = updatedText };

        // Act
        var createAct = await createHandler.Handle(createCommand, default);
        var updateAct = await updateHandler.Handle(updateCommand, default);

        // Assert
        Assert.False(memoryCache.TryGetValue("notesList", out List<Note> _));
    }
}