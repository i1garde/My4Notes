using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using MediatR;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;
using My4Notes.Resources;
using My4Notes.Resources.Commands;
using My4Notes.Resources.Queries;
using My4Notes.Web.Components;
using My4Notes.Web.Components.Pages;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents();
builder.Services.AddAntiforgery();
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddSingleton<ApplicationState>();
builder.Services.AddMemoryCache();
builder.Services.AddApplication();

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

app.MapGet("/", async (ApplicationState appState, IMediator _mediator, ILogger<IndexPage> logger) =>
{
    var query = new GetNotesCountQuery();
    appState.NotesCount = await _mediator.Send(query);
    logger.LogInformation("Index page load.");
    return new RazorComponentResult<IndexPage>();
});

app.MapGet("/notes", async (IMediator _mediator, ILogger<NotesList> logger) =>
{
    var query = new GetAllNotesQuery();
    var notes = await _mediator.Send(query);
    logger.LogInformation("Fetch notes list.");
    return new RazorComponentResult<NotesList>(new { Notes = notes });
});

app.MapGet("/notes/{id}", async (int id, IMediator _mediator, ILogger<GetNote> logger) =>
{
    var query = new GetNoteByIdQuery() { Id = id };
    var note = await _mediator.Send(query);
    logger.LogInformation($"Get note by ID: {id}");
    return new RazorComponentResult<GetNote>(new { Note = note });
});

app.MapPost("/notes/{id}/delete", async (int id, IMediator _mediator, HttpContext context, ILogger<IndexPage> logger) =>
{
    var command = new DeleteNoteCommand() { Id = id };
    var note = await _mediator.Send(command);
    logger.LogInformation($"Delete note with ID: {id}");
    context.Response.Redirect("/");
});

app.MapPut("/notes/{id}", async (
    [FromForm] int id,
    [FromForm] string title, 
    [FromForm] string text,
    IMediator _mediator,
    ILogger<HideNote> logger) =>
{
    var command = new UpdateNoteCommand()
    {
        Id = id,
        Title = title,
        Text = text,
    };
    var updNote = await _mediator.Send(command);
    logger.LogInformation($"Update note ID: {id}, Title: {title}, Text: {text}");
    return new RazorComponentResult<HideNote>(new { Note = updNote });
});

app.MapGet("/notes/{id}/edit", async (
    HttpContext context, 
    IAntiforgery antiforgery, 
    int id, 
    IMediator _mediator,
    ILogger<EditNote> logger) =>
{
    var token = antiforgery.GetAndStoreTokens(context);
    var query = new GetNoteByIdQuery() { Id = id };
    var note = await _mediator.Send(query);
    logger.LogInformation($"Load edit note page for ID: {id}");
    return new RazorComponentResult<EditNote>(new { Note = note, Token = token });
});

app.MapGet("/notes/{id}/hide", async (int id, IMediator _mediator, ILogger<HideNote> logger) =>
{
    var query = new GetNoteByIdQuery() { Id = id };
    var note = await _mediator.Send(query);
    logger.LogInformation($"Hide note with ID: {id}");
    return new RazorComponentResult<HideNote>(new { Note = note });
});

app.MapGet("/createNote", (HttpContext context, IAntiforgery antiforgery, ILogger<CreateNote> logger) =>
{
    var token = antiforgery.GetAndStoreTokens(context);
    logger.LogInformation("Create note modal popup.");
    return new RazorComponentResult<CreateNote>(new { Token = token });
});

app.MapPost("/createNote",
    async (
        [FromForm] string title,
        [FromForm] string text,
        HttpContext context,
        IMediator _mediator,
        ILogger<CreateNote> logger
        ) =>
    {
        var command = new CreateNoteCommand()
        {
            Title = title,
            Text = text,
            CreationDate = DateTime.UtcNow
        };
        await _mediator.Send(command);
        logger.LogInformation($"Create note Title: {title}, Text: {text}.");
        context.Response.Redirect("/");
    }
);

app.MapGet("/search",
    (HttpContext context, IAntiforgery antiforgery, ILogger<SearchBar> logger) => {
        var token = antiforgery.GetAndStoreTokens(context);
        logger.LogInformation("Search bar widget load.");
        return new RazorComponentResult<SearchBar>(new { Token = token });
    });

app.MapPost("/notes/search",
    async (
        [FromForm] string search,
        IMediator _mediator,
        ILogger<SearchBar> logger
        ) => {
        var query = new SearchNotesQuery() { SearchText = search };
        var searchedList = await _mediator.Send(query);
        logger.LogInformation("Update notes list via search bar.");
        return new RazorComponentResult<NotesList>(new { Notes = searchedList });
    });

app.Run();

public partial class Program { }

public class ApplicationState
{
    public int NotesCount { get; set; }
}