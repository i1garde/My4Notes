using MediatR;
using Microsoft.Extensions.Caching.Memory;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;

namespace My4Notes.Resources.Commands;

/// <summary>
/// Handles the DeleteNoteCommand to delete a note in the application.
/// </summary>
public class DeleteNoteCommandHandler(ApplicationDbContext dbContext, IMemoryCache memoryCache) : IRequestHandler<DeleteNoteCommand, Note>
{
    /// <summary>
    /// Handles the DeleteNoteCommand request.
    /// </summary>
    /// <param name="request">The DeleteNoteCommand request.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deleted note, or default if the note was not found.</returns>
    public async Task<Note> Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
    {
        var product = dbContext.Notes.FirstOrDefault(p => p.Id == request.Id);

        if (product is null)
            return default;

        dbContext.Remove(product);
        await dbContext.SaveChangesAsync();
        memoryCache.Remove("notesList");
        memoryCache.Remove("notesCount");
        return product;
    }
}