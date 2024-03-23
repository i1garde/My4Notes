using MediatR;
using Microsoft.Extensions.Caching.Memory;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;

namespace My4Notes.Resources.Commands;

public class DeleteNoteCommandHandler(ApplicationDbContext dbContext, IMemoryCache memoryCache) : IRequestHandler<DeleteNoteCommand, Note>
{
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