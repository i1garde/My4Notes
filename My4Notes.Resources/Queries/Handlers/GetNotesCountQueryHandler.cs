using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;

namespace My4Notes.Resources.Queries;

public class GetNotesCountQueryHandler(ApplicationDbContext context, IMemoryCache memoryCache) 
    : IRequestHandler<GetNotesCountQuery, int>
{
    public async Task<int> Handle(GetNotesCountQuery request, CancellationToken cancellationToken) =>
        await memoryCache.GetOrCreateAsync("notesCount", async entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromSeconds(60);
            return await context.Notes.AsNoTracking().CountAsync();
        });
}