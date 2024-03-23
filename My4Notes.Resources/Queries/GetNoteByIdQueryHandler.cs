using MediatR;
using Microsoft.EntityFrameworkCore;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;

namespace My4Notes.Resources.Queries;

public class GetNoteByIdQueryHandler : IRequestHandler<GetNoteByIdQuery, Note>
{
    private readonly ApplicationDbContext _context;
    
    public GetNoteByIdQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Note> Handle(GetNoteByIdQuery request, CancellationToken cancellationToken) =>
        await _context.Notes.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
}