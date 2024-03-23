using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;

namespace My4Notes.Resources.Queries;

/// <summary>
/// Handles the GetAllNotesQuery to retrieve all notes from the application.
/// </summary>
public class GetAllNotesQueryHandler(ApplicationDbContext context, IMemoryCache memoryCache)
    : IRequestHandler<GetAllNotesQuery, IEnumerable<Note>?>
{
    /// <summary>
    /// Handles the GetAllNotesQuery request.
    /// </summary>
    /// <param name="request">The GetAllNotesQuery request.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of notes.</returns>
    public async Task<IEnumerable<Note>?> Handle(GetAllNotesQuery request, CancellationToken cancellationToken) =>
        await memoryCache.GetOrCreateAsync("notesList", async entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromSeconds(60);
            return await context.Notes
                .AsNoTracking()
                .OrderByDescending(n => n.CreationDate)
                .ToListAsync(cancellationToken);
        });
}