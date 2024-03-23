using MediatR;
using My4Notes.Entities;

namespace My4Notes.Resources.Queries;

public class GetNoteByIdQuery : IRequest<Note?>
{
    public int Id { get; set; }
}