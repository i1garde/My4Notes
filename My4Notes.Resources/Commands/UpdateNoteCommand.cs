using MediatR;
using My4Notes.Entities;

namespace My4Notes.Resources.Commands;

/// <summary>
/// Represents a command to update a note in the application.
/// </summary>
public class UpdateNoteCommand : IRequest<Note>
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Text { get; set; }
}