using MediatR;
using Microsoft.EntityFrameworkCore;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;

namespace My4Notes.Resources.Queries;

public class GetAllNotesQueryHandler : IRequestHandler<GetAllNotesQuery, IEnumerable<Note>>
{
    private readonly ApplicationDbContext _context;
    
    public GetAllNotesQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Note>> Handle(GetAllNotesQuery request, CancellationToken cancellationToken) =>
        await _context.Notes.ToListAsync();
}