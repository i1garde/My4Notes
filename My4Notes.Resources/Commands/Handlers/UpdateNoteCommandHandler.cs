using MediatR;
using Microsoft.Extensions.Caching.Memory;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;

namespace My4Notes.Resources.Commands;

/// <summary>
/// Handles the UpdateNoteCommand to update a note in the application.
/// </summary>
public class UpdateNoteCommandHandler(ApplicationDbContext dbContext, IMemoryCache memoryCache) : IRequestHandler<UpdateNoteCommand, Note>
{
    /// <summary>
    /// Handles the UpdateNoteCommand request.
    /// </summary>
    /// <param name="request">The UpdateNoteCommand request.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated note, or default if the note was not found.</returns>
    public async Task<Note> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        var product = dbContext.Notes.FirstOrDefault(p => p.Id == request.Id);

        if (product is null)
            return default;

        product.Id = request.Id;
        product.Title = request.Title;
        product.Text = request.Text;

        await dbContext.SaveChangesAsync();
        memoryCache.Remove("notesList");
        
        return product;
    }
}