using MediatR;
using Microsoft.Extensions.Caching.Memory;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;

namespace My4Notes.Resources.Commands;

public class UpdateNoteCommandHandler(ApplicationDbContext dbContext, IMemoryCache memoryCache) : IRequestHandler<UpdateNoteCommand, Note>
{
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