using MediatR;
using My4Notes.Entities;

namespace My4Notes.Resources.Commands;

/// <summary>
/// Represents a command to delete a note in the application.
/// </summary>
public class DeleteNoteCommand : IRequest<Note>
{
    public int Id { get; set; }
}