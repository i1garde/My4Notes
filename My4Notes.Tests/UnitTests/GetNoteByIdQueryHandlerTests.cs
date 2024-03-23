using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Moq.EntityFrameworkCore;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;
using My4Notes.Resources.Queries;

namespace My4Notes.Tests;

public class GetNoteByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsCorrectNote_WhenNoteExists()
    {
        // Arrange
        var notes = new List<Note>
        {
            new Note { Id = 1, Title = "Note 1" },
            new Note { Id = 2, Title = "Note 2" }
        };

        var mockContext = new Mock<ApplicationDbContext>();
        mockContext.Setup(x => x.Notes).ReturnsDbSet(notes);

        var handler = new GetNoteByIdQueryHandler(mockContext.Object);

        // Act
        var result = await handler.Handle(new GetNoteByIdQuery { Id = 1 }, default);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenNoteDoesNotExist()
    {
        // Arrange
        var notes = new List<Note>
        {
            new Note { Id = 1, Title = "Note 1" },
            new Note { Id = 2, Title = "Note 2" }
        };

        var mockContext = new Mock<ApplicationDbContext>();
        mockContext.Setup(x => x.Notes).ReturnsDbSet(notes);

        var handler = new GetNoteByIdQueryHandler(mockContext.Object);

        // Act
        var result = await handler.Handle(new GetNoteByIdQuery { Id = 3 }, default);

        // Assert
        Assert.Null(result);
    }
}