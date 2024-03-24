using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using My4Notes.DatabaseAccess;
using My4Notes.Resources.Queries;

namespace My4Notes.Tests;

public class SearchNotesQueryHandlerTests
{
    private ApplicationDbContext _context;
    private IMemoryCache _memoryCache;
    private SearchNotesQueryHandler _handler;

    public SearchNotesQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Notes.AddRange(TestData.Notes);
        _context.SaveChanges();

        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _handler = new SearchNotesQueryHandler(_context, _memoryCache);
    }
    
    [Fact]
    public async Task Handle_ReturnsCorrectNotes_WhenSearchTextMatches()
    {
        // Arrange
        var notes = TestData.Notes;

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var handler = new SearchNotesQueryHandler(_context, memoryCache);

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
        var notes = TestData.Notes;

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var handler = new SearchNotesQueryHandler(_context, memoryCache);

        // Act
        var result = await handler.Handle(new SearchNotesQuery { SearchText = "6" }, default);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_UsesCacheCorrectly()
    {
        // Arrange
        var notes = TestData.Notes;

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var handler = new SearchNotesQueryHandler(_context, memoryCache);

        // Act
        var result1 = await handler.Handle(new SearchNotesQuery { SearchText = "1" }, default);
        var result2 = await handler.Handle(new SearchNotesQuery { SearchText = "2" }, default);
        var result3 = await handler.Handle(new SearchNotesQuery { SearchText = "Title" }, default);

        // Assert
        Assert.Single(result1);
        Assert.Single(result2);
        Assert.Equal(5, result3.Count());
    }
}