using MediatR;
using My4Notes.Entities;
using My4Notes.Resources.Commands;
using My4Notes.Resources.Queries;

namespace My4Notes.Web.Components.Services;

public class NotesService
{
    private IMediator _mediator;
    
    public event Func<Task> OnNotesListChange;

    public NotesService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<List<Note>> GetNotes()
    {
        var query = new GetAllNotesQuery();
        var notes = await _mediator.Send(query);
        return notes.ToList();
    }

    public async Task CreateNote(string title, string text)
    {
        var command = new CreateNoteCommand() { Title = title, Text = text, CreationDate = DateTime.UtcNow };
        var notes = await _mediator.Send(command);
        NotifyNotesListChanged();
    }
    
    public async Task EditNote(int id, string title, string text)
    {
        var command = new UpdateNoteCommand() { Id = id, Title = title, Text = text };
        var notes = await _mediator.Send(command);
        NotifyNotesListChanged();
    }
    
    public async Task DeleteNote(int id)
    {
        var command = new DeleteNoteCommand() { Id = id };
        var notes = await _mediator.Send(command);
        NotifyNotesListChanged();
    }
    
    public async Task<int> GetNotesCount()
    {
        var query = new GetNotesCountQuery();
        var notesCount = await _mediator.Send(query);
        return notesCount;
    }
    
    private void NotifyNotesListChanged()
    {
        OnNotesListChange?.Invoke();
    }
}