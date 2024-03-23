using MediatR;
using My4Notes.Entities;

namespace My4Notes.Resources.Queries;

/// <summary>
/// Represents a query to retrieve a specific note by its identifier from the application.
/// </summary>
public class GetNoteByIdQuery : IRequest<Note?>
{
    public int Id { get; set; }
}