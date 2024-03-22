using System.ComponentModel;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;
using My4Notes.Web.Components;
using My4Notes.Web.Components.Pages;
using My4Notes.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents();
builder.Services.AddAntiforgery();
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddSingleton<ApplicationState>();
builder.Services.AddTransient<NotesService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Configures the application to respond to a GET request to the root URL ("/").
// Parameters:
//   appState: The application state.
//   notesService: The service for managing notes.
// Returns:
//   A task that represents the asynchronous operation. The task result is a RazorComponentResult that renders the IndexPage component.
app.MapGet("/", async (ApplicationState appState, NotesService notesService) =>
{
    appState.NotesCount = await notesService.GetNotesCountAsync();
    return new RazorComponentResult<IndexPage>();
});

// Configures the application to respond to a GET request to the "/notes" URL.
// Parameters:
//   notesService: The service for managing notes.
// Returns:
//   A task that represents the asynchronous operation. The task result is a RazorComponentResult that renders the NotesList component with the list of notes.
app.MapGet("/notes", async (NotesService notesService) =>
{
    var notes = await notesService.GetNotesAsync();
    return new RazorComponentResult<NotesList>(new { Notes = notes });
});

// Configures the application to respond to a GET request to the "/notes/{id}" URL.
// Parameters:
//   notesService: The service for managing notes.
//   id: The ID of the note to retrieve.
// Returns:
//   A task that represents the asynchronous operation. The task result is a RazorComponentResult that renders the GetNote component with the specified note.
app.MapGet("/notes/{id}", async (NotesService notesService, int id) =>
{
    var note = await notesService.GetNoteByIdAsync(id);
    return new RazorComponentResult<GetNote>(new { Note = note });
});

// Configures the application to respond to a PUT request to the "/notes/{id}" URL.
// Parameters:
//   id: The ID of the note to update.
//   title: The new title of the note.
//   text: The new text of the note.
//   notesService: The service for managing notes.
// Returns:
//   A task that represents the asynchronous operation. The task result is a RazorComponentResult that renders the HideNote component with the updated note.
app.MapPut("/notes/{id}", async (
    [FromForm] int id,
    [FromForm] string title, 
    [FromForm] string text,
    NotesService notesService) =>
{
    var noteById = await notesService.GetNoteByIdAsync(id);
    noteById!.Title = title;
    noteById.Text = text;
    await notesService.UpdateNoteAsync(noteById);
    return new RazorComponentResult<HideNote>(new { Note = noteById });
});

// Configures the application to respond to a GET request to the "/notes/{id}/edit" URL.
// Parameters:
//   context: The HttpContext for the current request.
//   antiforgery: The anti-forgery service.
//   notesService: The service for managing notes.
//   id: The ID of the note to edit.
// Returns:
//   A task that represents the asynchronous operation. The task result is a RazorComponentResult that renders the EditNote component with the specified note and an antiforgery token.
app.MapGet("/notes/{id}/edit", async (HttpContext context, IAntiforgery antiforgery, NotesService notesService, int id) =>
{
    var token = antiforgery.GetAndStoreTokens(context);
    var note = await notesService.GetNoteByIdAsync(id);
    return new RazorComponentResult<EditNote>(new { Note = note, Token = token });
});

// Configures the application to respond to a GET request to the "/notes/{id}/hide" URL.
// Parameters:
//   notesService: The service for managing notes.
//   id: The ID of the note to hide.
// Returns:
//   A task that represents the asynchronous operation. The task result is a RazorComponentResult that renders the HideNote component with the specified note.
app.MapGet("/notes/{id}/hide", async (NotesService notesService, int id) =>
{
    var note = await notesService.GetNoteByIdAsync(id);
    return new RazorComponentResult<HideNote>(new { Note = note });
});

// Configures the application to respond to a GET request to the "/createNote" URL.
// Parameters:
//   context: The HttpContext for the current request.
//   antiforgery: The anti-forgery service.
// Returns:
//   A RazorComponentResult that renders the CreateNote component with an antiforgery token.
app.MapGet("/createNote", (HttpContext context, IAntiforgery antiforgery) =>
{
    var token = antiforgery.GetAndStoreTokens(context);
    return new RazorComponentResult<CreateNote>(new { Token = token });
});

// Configures the application to respond to a POST request to the "/createNote" URL.
// Parameters:
//   title: The title of the new note.
//   text: The text of the new note.
//   context: The HttpContext for the current request.
//   notesService: The service for managing notes.
// Returns:
//   A task that represents the asynchronous operation. After the note is added, the response is redirected to the root URL ("/").
app.MapPost("/createNote",
    async (
        [FromForm] string title,
        [FromForm] string text,
        HttpContext context,
        NotesService notesService) => {
            await notesService.AddNoteAsync(new Note { Title = title, Text = text });
            context.Response.Redirect("/");
});

// Configures the application to respond to a GET request to the "/search" URL.
// Parameters:
//   context: The HttpContext for the current request.
//   antiforgery: The anti-forgery service.
// Returns:
//   A RazorComponentResult that renders the SearchBar component with an antiforgery token.
app.MapGet("/search",
    (HttpContext context, IAntiforgery antiforgery) => {
        var token = antiforgery.GetAndStoreTokens(context);
        return new RazorComponentResult<SearchBar>(new { Token = token });
    });

// Configures the application to respond to a POST request to the "/notes/search" URL.
// Parameters:
//   search: The search string to match against note titles.
//   notesService: The service for managing notes.
// Returns:
//   A task that represents the asynchronous operation. The task result is a RazorComponentResult that renders the NotesList component with the list of notes that match the search string.
app.MapPost("/notes/search",
    async (
        [FromForm] string search,
        NotesService notesService) => {
        var searchedList = await notesService.GetNotesByTitleAsync(search);
        return new RazorComponentResult<NotesList>(new { Notes = searchedList });
    });

app.Run();

public class ApplicationState
{
    public int NotesCount { get; set; }
}