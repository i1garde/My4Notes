using MediatR;
using My4Notes.Entities;

namespace My4Notes.Resources.Queries;

public class SearchNotesQuery : IRequest<IEnumerable<Note>>
{
    public string? SearchText { get; set; }
}