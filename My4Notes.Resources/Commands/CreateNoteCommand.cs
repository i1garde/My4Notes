using MediatR;
using My4Notes.Entities;

namespace My4Notes.Resources.Commands;

/// <summary>
/// Represents a command to create a new note in the application.
/// </summary>
public class CreateNoteCommand : IRequest<Note>
{
    public string? Title { get; set; }
    public string? Text { get; set; }
    public DateTime CreationDate { get; set; }
}