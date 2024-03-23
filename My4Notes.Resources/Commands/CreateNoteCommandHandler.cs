using MediatR;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;

namespace My4Notes.Resources.Commands;

public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, Note>
{
    private readonly ApplicationDbContext _dbContext;

    public CreateNoteCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Note> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
    {
        var product = new Note
        {
            Title = request.Title,
            Text = request.Text,
            CreationDate = request.CreationDate,
        };

        _dbContext.Notes.Add(product);
        await _dbContext.SaveChangesAsync();
        return product;
    }
}