using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;
using My4Notes.Resources.Queries;

namespace My4Notes.Tests;

public class GetNotesCountQueryHandlerTests
{
    private ApplicationDbContext _context;
    private ApplicationDbContext _contextEmpty;
    private IMemoryCache _memoryCache;
    private GetNotesCountQueryHandler _handler;

    public GetNotesCountQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        var optionsEmpty = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _contextEmpty = new ApplicationDbContext(optionsEmpty);
        _context.Notes.AddRange(TestData.Notes);
        _context.SaveChanges();

        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _handler = new GetNotesCountQueryHandler(_context, _memoryCache);
    }
    [Fact]
    public async Task Handle_ReturnsCorrectCount()
    {
        // Arrange
        var notes = TestData.Notes;

        var handler = new GetNotesCountQueryHandler(_context, _memoryCache);

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

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var emptyHandler = new GetNotesCountQueryHandler(_contextEmpty, _memoryCache);

        // Act
        var result = await emptyHandler.Handle(new GetNotesCountQuery(), default);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task Handle_UsesCacheCorrectly()
    {
        // Arrange
        var notes = TestData.Notes;

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var handler = new GetNotesCountQueryHandler(_context, memoryCache);

        // Act
        var result1 = await handler.Handle(new GetNotesCountQuery(), default);
        var result2 = memoryCache.Get<int>("notesCount");

        // Assert
        Assert.Equal(result1, result2);
    }
}