using MediatR;
using Microsoft.Extensions.Caching.Memory;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;

namespace My4Notes.Resources.Commands;

/// <summary>
/// Handles the CreateNoteCommand to create a new note in the application.
/// </summary>
public class CreateNoteCommandHandler(ApplicationDbContext dbContext, IMemoryCache memoryCache) : IRequestHandler<CreateNoteCommand, Note>
{
    /// <summary>
    /// Handles the CreateNoteCommand request.
    /// </summary>
    /// <param name="request">The CreateNoteCommand request.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created note.</returns>
    public async Task<Note> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
    {
        var product = new Note
        {
            Title = request.Title,
            Text = request.Text,
            CreationDate = request.CreationDate,
        };

        dbContext.Notes.Add(product);
        await dbContext.SaveChangesAsync();
        memoryCache.Remove("notesList");
        memoryCache.Remove("notesCount");
        return product;
    }
}