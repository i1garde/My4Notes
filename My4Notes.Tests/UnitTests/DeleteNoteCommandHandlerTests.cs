using Microsoft.Extensions.Caching.Memory;
using Moq;
using Moq.EntityFrameworkCore;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;
using My4Notes.Resources.Commands;

namespace My4Notes.Tests;

public class DeleteNoteCommandHandlerTests
{
    [Fact]
    public async Task Handle_DeletesNoteCorrectly()
    {
        // Arrange
        var notes = new List<Note>
        {
            new Note { Id = 1, Title = "Note 1", Text = "Text 1", CreationDate = DateTime.Now }
        };

        var mockContext = new Mock<ApplicationDbContext>();
        mockContext.Setup(x => x.Notes).ReturnsDbSet(notes);
        mockContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var handler = new DeleteNoteCommandHandler(mockContext.Object, memoryCache);

        var command = new DeleteNoteCommand { Id = 1 };

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        Assert.Equal(command.Id, result.Id);
        mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsDefaultWhenNoteDoesNotExist()
    {
        // Arrange
        var notes = new List<Note>();

        var mockContext = new Mock<ApplicationDbContext>();
        mockContext.Setup(x => x.Notes).ReturnsDbSet(notes);

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var handler = new DeleteNoteCommandHandler(mockContext.Object, memoryCache);

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
        var notes = new List<Note>
        {
            new Note { Id = 1, Title = "Note 1", Text = "Text 1", CreationDate = DateTime.Now }
        };

        var mockContext = new Mock<ApplicationDbContext>();
        mockContext.Setup(x => x.Notes).ReturnsDbSet(notes);
        mockContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        memoryCache.Set("notesList", notes);
        memoryCache.Set("notesCount", notes.Count);

        var handler = new DeleteNoteCommandHandler(mockContext.Object, memoryCache);

        var command = new DeleteNoteCommand { Id = 1 };

        // Act
        await handler.Handle(command, default);

        // Assert
        Assert.False(memoryCache.TryGetValue("notesList", out List<Note> _));
        Assert.False(memoryCache.TryGetValue("notesCount", out int _));
    }
}