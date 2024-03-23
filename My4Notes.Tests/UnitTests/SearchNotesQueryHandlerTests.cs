using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Moq.EntityFrameworkCore;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;
using My4Notes.Resources.Queries;

namespace My4Notes.Tests;

public class SearchNotesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsCorrectNotes_WhenSearchTextMatches()
    {
        // Arrange
        var notes = new List<Note>
        {
            new Note { Id = 1, Title = "Note 1", Text = "Text 1" },
            new Note { Id = 2, Title = "Note 2", Text = "Text 2" }
        };

        var mockContext = new Mock<ApplicationDbContext>();
        mockContext.Setup(x => x.Notes).ReturnsDbSet(notes);

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var handler = new SearchNotesQueryHandler(mockContext.Object, memoryCache);

        // Act
        var result = await handler.Handle(new SearchNotesQuery { SearchText = "1" }, default);

        // Assert
        Assert.Single(result);
        Assert.Equal(1, result.First().Id);
    }

    [Fact]
    public async Task Handle_ReturnsEmpty_WhenSearchTextDoesNotMatch()
    {
        // Arrange
        var notes = new List<Note>
        {
            new Note { Id = 1, Title = "Note 1", Text = "Text 1" },
            new Note { Id = 2, Title = "Note 2", Text = "Text 2" }
        };

        var mockContext = new Mock<ApplicationDbContext>();
        mockContext.Setup(x => x.Notes).ReturnsDbSet(notes);

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var handler = new SearchNotesQueryHandler(mockContext.Object, memoryCache);

        // Act
        var result = await handler.Handle(new SearchNotesQuery { SearchText = "3" }, default);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_UsesCacheCorrectly()
    {
        // Arrange
        var notes = new List<Note>
        {
            new Note { Id = 1, Title = "Note 1", Text = "Text 1" },
            new Note { Id = 2, Title = "Note 2", Text = "Text 2" }
        };

        var mockContext = new Mock<ApplicationDbContext>();
        mockContext.Setup(x => x.Notes).ReturnsDbSet(notes);

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var handler = new SearchNotesQueryHandler(mockContext.Object, memoryCache);

        // Act
        var result1 = await handler.Handle(new SearchNotesQuery { SearchText = "1" }, default);
        var result2 = await handler.Handle(new SearchNotesQuery { SearchText = "2" }, default);

        // Assert
        Assert.Single(result1);
        Assert.Single(result2);
        mockContext.Verify(x => x.Notes, Times.Once);
    }
}