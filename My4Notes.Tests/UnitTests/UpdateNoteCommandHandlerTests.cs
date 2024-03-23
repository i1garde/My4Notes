using Microsoft.Extensions.Caching.Memory;
using Moq;
using Moq.EntityFrameworkCore;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;
using My4Notes.Resources.Commands;

namespace My4Notes.Tests;

public class UpdateNoteCommandHandlerTests
{
    [Fact]
    public async Task Handle_UpdatesNoteCorrectly()
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

        var handler = new UpdateNoteCommandHandler(mockContext.Object, memoryCache);

        var command = new UpdateNoteCommand { Id = 1, Title = "Updated Note", Text = "Updated Text" };

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(command.Title, result.Title);
        Assert.Equal(command.Text, result.Text);
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

        var handler = new UpdateNoteCommandHandler(mockContext.Object, memoryCache);

        var command = new UpdateNoteCommand { Id = 1, Title = "Updated Note", Text = "Updated Text" };

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_RemovesCorrectCacheEntry()
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

        var handler = new UpdateNoteCommandHandler(mockContext.Object, memoryCache);

        var command = new UpdateNoteCommand { Id = 1, Title = "Updated Note", Text = "Updated Text" };

        // Act
        await handler.Handle(command, default);

        // Assert
        Assert.False(memoryCache.TryGetValue("notesList", out List<Note> _));
    }
}