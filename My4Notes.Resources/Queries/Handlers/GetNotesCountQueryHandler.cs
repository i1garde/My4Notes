using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;

namespace My4Notes.Resources.Queries;

/// <summary>
/// Handles the GetNotesCountQuery to retrieve the total count of notes in the application.
/// </summary>
public class GetNotesCountQueryHandler(ApplicationDbContext context, IMemoryCache memoryCache) 
    : IRequestHandler<GetNotesCountQuery, int>
{
    /// <summary>
    /// Handles the GetNotesCountQuery request.
    /// </summary>
    /// <param name="request">The GetNotesCountQuery request.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the total count of notes.</returns>
    public async Task<int> Handle(GetNotesCountQuery request, CancellationToken cancellationToken) =>
        await memoryCache.GetOrCreateAsync("notesCount", async entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromSeconds(60);
            return await context.Notes.AsNoTracking().CountAsync();
        });
}