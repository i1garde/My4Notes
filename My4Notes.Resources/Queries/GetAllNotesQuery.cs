using MediatR;
using My4Notes.Entities;

namespace My4Notes.Resources.Queries;

/// <summary>
/// Represents a query to retrieve all notes from the application.
/// </summary>
public class GetAllNotesQuery : IRequest<IEnumerable<Note>?>
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Text { get; set; }
    public DateTime CreationDate { get; set; }
}