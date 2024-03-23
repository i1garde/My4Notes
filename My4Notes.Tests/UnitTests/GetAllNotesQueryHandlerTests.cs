using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Moq.EntityFrameworkCore;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;
using My4Notes.Resources.Queries;

namespace My4Notes.Tests;

public class GetAllNotesQueryHandlerTests
{
    private string testConnString = "Host=localhost;Port=5433;Database=My4NotesTestDB;Username=postgres;Password=Str0ngP@ssw0rd";
    
    [Fact]
    public async Task Handle_ReturnsCorrectNotes()
    {
        // Arrange
        var notes = new List<Note>
        {
            new Note { Id = 1, Title = "Note 1", Text = "Text 1", CreationDate = DateTime.UtcNow },
            new Note { Id = 2, Title = "Note 2", Text = "Text 2", CreationDate = DateTime.UtcNow }
        };
        var mockContext = new Mock<ApplicationDbContext>();
        mockContext.Setup(x => x.Notes).ReturnsDbSet(notes);
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var handler = new GetAllNotesQueryHandler(mockContext.Object, memoryCache);

        // Act
        var result = await handler.Handle(new GetAllNotesQuery(), default);

        // Assert
        Assert.Equal(notes.Count, result.Count());
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenNoNotes()
    {
        // Arrange
        var notes = new List<Note>();

        var mockContext = new Mock<ApplicationDbContext>();
        mockContext.Setup(x => x.Notes).ReturnsDbSet(notes);

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var handler = new GetAllNotesQueryHandler(mockContext.Object, memoryCache);

        // Act
        var result = await handler.Handle(new GetAllNotesQuery(), default);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_ReturnsNotesInCorrectOrder()
    {
        // Arrange
        var notes = new List<Note>
        {
            new Note { Id = 1, Title = "Note 1", CreationDate = DateTime.Now.AddDays(-1) },
            new Note { Id = 2, Title = "Note 2", CreationDate = DateTime.Now }
        };

        var mockContext = new Mock<ApplicationDbContext>();
        mockContext.Setup(x => x.Notes).ReturnsDbSet(notes);

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var handler = new GetAllNotesQueryHandler(mockContext.Object, memoryCache);

        // Act
        var result = await handler.Handle(new GetAllNotesQuery(), default);

        // Assert
        Assert.Equal(notes.OrderByDescending(n => n.CreationDate), result);
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

        var handler = new GetAllNotesQueryHandler(mockContext.Object, memoryCache);

        // Act
        var result1 = await handler.Handle(new GetAllNotesQuery(), default);
        var result2 = await handler.Handle(new GetAllNotesQuery(), default);

        // Assert
        Assert.Equal(result1, result2);
        mockContext.Verify(x => x.Notes, Times.Once);
    }
}