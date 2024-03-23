using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;

namespace My4Notes.Resources.Queries;

public class GetNoteByIdQueryHandler(ApplicationDbContext context) 
    : IRequestHandler<GetNoteByIdQuery, Note?>
{
    public async Task<Note?> Handle(GetNoteByIdQuery request, CancellationToken cancellationToken)
    {
        return await context.Notes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
    }
}