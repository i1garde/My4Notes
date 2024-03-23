using System.ComponentModel;
using System.Reflection;
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

app.MapGet("/", async (ApplicationState appState, IMediator _mediator) =>
{
    var query = new GetNotesCountQuery();
    appState.NotesCount = await _mediator.Send(query);
    return new RazorComponentResult<IndexPage>();
});

app.MapGet("/notes", async (IMediator _mediator) =>
{
    var query = new GetAllNotesQuery();
    var notes = await _mediator.Send(query);
    return new RazorComponentResult<NotesList>(new { Notes = notes });
});

app.MapGet("/notes/{id}", async (int id, IMediator _mediator) =>
{
    var query = new GetNoteByIdQuery() { Id = id };
    var note = await _mediator.Send(query);
    
    return new RazorComponentResult<GetNote>(new { Note = note });
});

app.MapPut("/notes/{id}", async (
    [FromForm] int id,
    [FromForm] string title, 
    [FromForm] string text,
    IMediator _mediator) =>
{
    var command = new UpdateNoteCommand()
    {
        Id = id,
        Title = title,
        Text = text,
    };
    var updNote = await _mediator.Send(command);
    
    return new RazorComponentResult<HideNote>(new { Note = updNote });
});

app.MapGet("/notes/{id}/edit", async (HttpContext context, IAntiforgery antiforgery, int id, IMediator _mediator) =>
{
    var token = antiforgery.GetAndStoreTokens(context);
    var query = new GetNoteByIdQuery() { Id = id };
    var note = await _mediator.Send(query);
    return new RazorComponentResult<EditNote>(new { Note = note, Token = token });
});

app.MapGet("/notes/{id}/hide", async (int id, IMediator _mediator) =>
{
    var query = new GetNoteByIdQuery() { Id = id };
    var note = await _mediator.Send(query);
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
        IMediator _mediator) =>
    {
        var command = new CreateNoteCommand()
        {
            Title = title,
            Text = text,
            CreationDate = DateTime.UtcNow
        };
        await _mediator.Send(command);
        context.Response.Redirect("/");
    }
);

app.MapGet("/search",
    (HttpContext context, IAntiforgery antiforgery) => {
        var token = antiforgery.GetAndStoreTokens(context);
        return new RazorComponentResult<SearchBar>(new { Token = token });
    });

app.MapPost("/notes/search",
    async (
        [FromForm] string search,
        IMediator _mediator) => {
        var query = new SearchNotesQuery() { SearchText = search };
        var searchedList = await _mediator.Send(query);
        return new RazorComponentResult<NotesList>(new { Notes = searchedList });
    });

app.Run();

public class ApplicationState
{
    public int NotesCount { get; set; }
}