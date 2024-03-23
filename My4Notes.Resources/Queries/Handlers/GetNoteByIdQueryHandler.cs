using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;

namespace My4Notes.Resources.Queries;

/// <summary>
/// Handles the GetNoteByIdQuery to retrieve a specific note by its identifier from the application.
/// </summary>
public class GetNoteByIdQueryHandler(ApplicationDbContext context) 
    : IRequestHandler<GetNoteByIdQuery, Note?>
{
    /// <summary>
    /// Handles the GetNoteByIdQuery request.
    /// </summary>
    /// <param name="request">The GetNoteByIdQuery request.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the note if found, null otherwise.</returns>
    public async Task<Note?> Handle(GetNoteByIdQuery request, CancellationToken cancellationToken)
    {
        return await context.Notes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
    }
}