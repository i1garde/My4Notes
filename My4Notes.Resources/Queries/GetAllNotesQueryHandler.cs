using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;

namespace My4Notes.Resources.Queries;

public class GetAllNotesQueryHandler(ApplicationDbContext context, IMemoryCache memoryCache)
    : IRequestHandler<GetAllNotesQuery, IEnumerable<Note>?>
{
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