using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using My4Notes.DatabaseAccess;
using My4Notes.Resources.Queries;

namespace My4Notes.Tests;

public class GetAllNotesQueryHandlerTests
{
    private ApplicationDbContext _context;
    private IMemoryCache _memoryCache;
    private GetAllNotesQueryHandler _handler;

    public GetAllNotesQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Notes.AddRange(TestData.Notes);
        _context.SaveChanges();

        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _handler = new GetAllNotesQueryHandler(_context, _memoryCache);
    }

    [Fact]
    public async Task Handle_GivenValidRequest_ReturnsNotes()
    {
        // Arrange
        var notes = TestData.Notes;
        
        var request = new GetAllNotesQuery();

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(notes.Count, result.Count());
    }
    
    [Fact]
    public async Task Handle_GivenValidRequest_ReturnsNotesInCorrectOrder()
    {
        // Arrange
        var notes = TestData.Notes;
        var request = new GetAllNotesQuery();

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(notes
            .OrderByDescending(n => n.CreationDate)
            .Select(x => x.Title)
            , result.Select(x => x.Title));
    }

    [Fact]
    public async Task Handle_GivenSameRequestTwice_UsesCache()
    {
        // Arrange
        var notes = TestData.Notes;
        var request = new GetAllNotesQuery();

        // Act
        var result1 = await _handler.Handle(request, CancellationToken.None);
        var result2 = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1, result2);
    }
}