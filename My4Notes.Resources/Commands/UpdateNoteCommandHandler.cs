using MediatR;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;

namespace My4Notes.Resources.Commands;

public class UpdateNoteCommandHandler : IRequestHandler<UpdateNoteCommand, Note>
{
    private readonly ApplicationDbContext _dbContext;

    public UpdateNoteCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Note> Handle(UpdateNoteCommand request, CancellationToken cancellationToken)
    {
        var product = _dbContext.Notes.FirstOrDefault(p => p.Id == request.Id);

        if (product is null)
            return default;

        product.Id = request.Id;
        product.Title = request.Title;
        product.Text = request.Text;

        await _dbContext.SaveChangesAsync();
        return product;
    }
}