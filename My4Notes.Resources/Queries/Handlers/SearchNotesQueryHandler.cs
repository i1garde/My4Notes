using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;

namespace My4Notes.Resources.Queries;

/// <summary>
/// Handles the SearchNotesQuery to search for notes in the application based on a text string.
/// </summary>
public class SearchNotesQueryHandler(ApplicationDbContext context, IMemoryCache memoryCache)
    : IRequestHandler<SearchNotesQuery, IEnumerable<Note>>
{
    /// <summary>
    /// Handles the SearchNotesQuery request.
    /// </summary>
    /// <param name="request">The SearchNotesQuery request.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of notes that match the search text.</returns>
    public async Task<IEnumerable<Note>> Handle(SearchNotesQuery request, CancellationToken cancellationToken)
    {
        var loweredSearch = request.SearchText.ToLower();
        
        if (memoryCache.TryGetValue("notesList", out List<Note> cachedNotes))
        {
            return cachedNotes
                .Where(n => n.Title.ToLower().Contains(loweredSearch) || n.Text.ToLower().Contains(loweredSearch))
                .ToList();
        } else {
            var notes = await context.Notes.AsNoTracking().ToListAsync();
            memoryCache.Set("notesList", notes, TimeSpan.FromSeconds(60));
            return notes
                .Where(n => n.Title.ToLower().Contains(loweredSearch) || n.Text.ToLower().Contains(loweredSearch))
                .ToList();
        }
    }
}