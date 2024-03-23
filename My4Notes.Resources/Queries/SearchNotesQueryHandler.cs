using MediatR;
using Microsoft.EntityFrameworkCore;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;

namespace My4Notes.Resources.Queries;

public class SearchNotesQueryHandler : IRequestHandler<SearchNotesQuery, IEnumerable<Note>>
{
    private readonly ApplicationDbContext _context;
    
    public SearchNotesQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Note>> Handle(SearchNotesQuery request, CancellationToken cancellationToken) {
        var loweredSerach = request.SearchText.ToLower();
        return await _context.Notes.AsNoTracking()
            .Where(n => n.Title.ToLower().Contains(loweredSerach) || n.Text.ToLower().Contains(loweredSerach))
            .ToListAsync();
    }
}