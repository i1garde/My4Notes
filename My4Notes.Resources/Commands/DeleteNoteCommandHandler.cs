using MediatR;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;

namespace My4Notes.Resources.Commands;

public class DeleteNoteCommandHandler : IRequestHandler<DeleteNoteCommand, Note>
{
    private readonly ApplicationDbContext _dbContext;

    public DeleteNoteCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Note> Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
    {
        var product = _dbContext.Notes.FirstOrDefault(p => p.Id == request.Id);

        if (product is null)
            return default;

        _dbContext.Remove(product);
        await _dbContext.SaveChangesAsync();
        return product;
    }
}