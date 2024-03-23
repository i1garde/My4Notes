using MediatR;
using My4Notes.Entities;

namespace My4Notes.Resources.Queries;

/// <summary>
/// Represents a query to search for notes in the application based on a text string.
/// </summary>
public class SearchNotesQuery : IRequest<IEnumerable<Note>>
{
    public string? SearchText { get; set; }
}