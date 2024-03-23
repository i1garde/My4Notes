using MediatR;
using Microsoft.Extensions.Caching.Memory;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;

namespace My4Notes.Resources.Commands;

public class CreateNoteCommandHandler(ApplicationDbContext dbContext, IMemoryCache memoryCache) : IRequestHandler<CreateNoteCommand, Note>
{
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