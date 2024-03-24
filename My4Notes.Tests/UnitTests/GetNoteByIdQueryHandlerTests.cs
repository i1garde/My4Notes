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
    private ApplicationDbContext _context;
    private GetNoteByIdQueryHandler _handler;

    public GetNoteByIdQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Notes.AddRange(TestData.Notes);
        _context.SaveChanges();

        _handler = new GetNoteByIdQueryHandler(_context);
    }
    [Fact]
    public async Task Handle_ReturnsCorrectNote_WhenNoteExists()
    {
        // Arrange
        var notes = TestData.Notes;

        var handler = new GetNoteByIdQueryHandler(_context);

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
        var notes = TestData.Notes;

        var handler = new GetNoteByIdQueryHandler(_context);

        // Act
        var result = await handler.Handle(new GetNoteByIdQuery { Id = 6 }, default);

        // Assert
        Assert.Null(result);
    }
}