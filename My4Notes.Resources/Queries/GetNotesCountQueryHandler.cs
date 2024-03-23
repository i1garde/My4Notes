using MediatR;
using Microsoft.EntityFrameworkCore;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;

namespace My4Notes.Resources.Queries;

public class GetNotesCountQueryHandler : IRequestHandler<GetNotesCountQuery, int>
{
    private readonly ApplicationDbContext _context;
    
    public GetNotesCountQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(GetNotesCountQuery request, CancellationToken cancellationToken) =>
        await _context.Notes.CountAsync();
}