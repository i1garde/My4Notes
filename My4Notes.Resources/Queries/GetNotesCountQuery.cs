using MediatR;
using My4Notes.Entities;

namespace My4Notes.Resources.Queries;

/// <summary>
/// Represents a query to retrieve the total count of notes in the application.
/// </summary>
public class GetNotesCountQuery : IRequest<int>
{
    
}