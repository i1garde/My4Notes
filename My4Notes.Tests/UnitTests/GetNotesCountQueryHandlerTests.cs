using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Moq.EntityFrameworkCore;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;
using My4Notes.Resources.Queries;

namespace My4Notes.Tests;

public class GetNotesCountQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsCorrectCount()
    {
        // Arrange
        var notes = new List<Note>
        {
            new Note { Id = 1, Title = "Note 1" },
            new Note { Id = 2, Title = "Note 2" }
        };

        var mockContext = new Mock<ApplicationDbContext>();
        mockContext.Setup(x => x.Notes).ReturnsDbSet(notes);

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var handler = new GetNotesCountQueryHandler(mockContext.Object, memoryCache);

        // Act
        var result = await handler.Handle(new GetNotesCountQuery(), default);

        // Assert
        Assert.Equal(notes.Count, result);
    }

    [Fact]
    public async Task Handle_ReturnsZero_WhenNoNotes()
    {
        // Arrange
        var notes = new List<Note>();

        var mockContext = new Mock<ApplicationDbContext>();
        mockContext.Setup(x => x.Notes).ReturnsDbSet(notes);

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var handler = new GetNotesCountQueryHandler(mockContext.Object, memoryCache);

        // Act
        var result = await handler.Handle(new GetNotesCountQuery(), default);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task Handle_UsesCacheCorrectly()
    {
        // Arrange
        var notes = new List<Note>
        {
            new Note { Id = 1, Title = "Note 1" },
            new Note { Id = 2, Title = "Note 2" }
        };

        var mockContext = new Mock<ApplicationDbContext>();
        mockContext.Setup(x => x.Notes).ReturnsDbSet(notes);

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var handler = new GetNotesCountQueryHandler(mockContext.Object, memoryCache);

        // Act
        var result1 = await handler.Handle(new GetNotesCountQuery(), default);
        var result2 = await handler.Handle(new GetNotesCountQuery(), default);

        // Assert
        Assert.Equal(result1, result2);
        mockContext.Verify(x => x.Notes, Times.Once);
    }
}