using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;

namespace My4Notes.Resources.Queries;

public class SearchNotesQueryHandler(ApplicationDbContext context, IMemoryCache memoryCache)
    : IRequestHandler<SearchNotesQuery, IEnumerable<Note>>
{
    public async Task<IEnumerable<Note>> Handle(SearchNotesQuery request, CancellationToken cancellationToken)
    {
        var loweredSearch = request.SearchText.ToLower();
        
        if (memoryCache.TryGetValue("notesList", out List<Note> cachedNotes))
        {
            // If the list is in the cache, perform the search operation on it
            return cachedNotes
                .Where(n => n.Title.ToLower().Contains(loweredSearch) || n.Text.ToLower().Contains(loweredSearch))
                .ToList();
        }
        else
        {
            // If the list is not in the cache, retrieve it from the database and perform the search operation
            var notes = await context.Notes.AsNoTracking().ToListAsync();
            memoryCache.Set("notesList", notes, TimeSpan.FromSeconds(60));
            return notes
                .Where(n => n.Title.ToLower().Contains(loweredSearch) || n.Text.ToLower().Contains(loweredSearch))
                .ToList();
        }
    }
}