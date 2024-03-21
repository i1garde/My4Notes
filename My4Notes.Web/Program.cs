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

app.MapGet("/", async (ApplicationState appState, NotesService notesService) =>
{
    appState.NotesCount = await notesService.GetNotesCountAsync();
    return new RazorComponentResult<IndexPage>();
});

app.MapGet("/notes", async (NotesService notesService) =>
{
    var notes = await notesService.GetNotesAsync();
    return new RazorComponentResult<NotesList>(new { Notes = notes });
});

app.MapGet("/notes/{id}", async (NotesService notesService, int id) =>
{
    var note = await notesService.GetNoteByIdAsync(id);
    return new RazorComponentResult<GetNote>(new { Note = note });
});

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

app.MapGet("/notes/{id}/edit", async (HttpContext context, IAntiforgery antiforgery, NotesService notesService, int id) =>
{
    var token = antiforgery.GetAndStoreTokens(context);
    var note = await notesService.GetNoteByIdAsync(id);
    return new RazorComponentResult<EditNote>(new { Note = note, Token = token });
});

app.MapGet("/notes/{id}/hide", async (NotesService notesService, int id) =>
{
    var note = await notesService.GetNoteByIdAsync(id);
    return new RazorComponentResult<HideNote>(new { Note = note });
});

app.MapGet("/createNote", (HttpContext context, IAntiforgery antiforgery) =>
{
    var token = antiforgery.GetAndStoreTokens(context);
    return new RazorComponentResult<CreateNote>(new { Token = token });
});

app.MapPost("/createNote",
    async (
        [FromForm] string title,
        [FromForm] string text,
        HttpContext context,
        NotesService notesService) => {
            await notesService.AddNoteAsync(new Note { Title = title, Text = text });
            context.Response.Redirect("/");
});

app.MapGet("/search",
    (HttpContext context, IAntiforgery antiforgery) => {
        var token = antiforgery.GetAndStoreTokens(context);
        return new RazorComponentResult<SearchBar>(new { Token = token });
    });

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