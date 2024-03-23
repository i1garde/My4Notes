using MediatR;
using My4Notes.Entities;

namespace My4Notes.Resources.Commands;

public class DeleteNoteCommand : IRequest<Note>
{
    public int Id { get; set; }
}