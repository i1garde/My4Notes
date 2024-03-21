using System.ComponentModel;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;
using My4Notes.Web.Components;
using My4Notes.Web.Components.Pages;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents();
//    .AddInteractiveServerComponents();

builder.Services.AddAntiforgery();

builder.Services.AddDbContext<ApplicationDbContext>();

builder.Services.AddSingleton<ApplicationState>();

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

//app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.MapGet("/", async (ApplicationState appState, ApplicationDbContext _applicationDbContext) =>
{
    appState.NotesCount = await _applicationDbContext.Notes.CountAsync();
    return new RazorComponentResult<IndexPage>();
});

app.MapGet("/notes", async (ApplicationDbContext _applicationDbContext) =>
{
    var notes = await _applicationDbContext.Notes.ToListAsync();
    return new RazorComponentResult<NotesList>(new { Notes = notes });
});

app.MapGet("/notes/{id}", async (ApplicationDbContext _applicationDbContext, int id) =>
{
    var note = await _applicationDbContext.Notes.FindAsync(id);
    return new RazorComponentResult<GetNote>(new { Note = note });
});

app.MapPut("/notes/{id}", async (
    [FromForm] int id,
    [FromForm] string title, 
    [FromForm] string text,
    ApplicationDbContext _applicationDbContext) =>
{
    var noteById = await _applicationDbContext.Notes.FindAsync(id);
    noteById.Title = title;
    noteById.Text = text;
    _applicationDbContext.Notes.Update(noteById);
    await _applicationDbContext.SaveChangesAsync();
    return new RazorComponentResult<HideNote>(new { Note = noteById });
});

app.MapGet("/notes/{id}/edit", async (HttpContext context, IAntiforgery antiforgery, ApplicationDbContext _applicationDbContext, int id) =>
{
    var token = antiforgery.GetAndStoreTokens(context);
    var note = await _applicationDbContext.Notes.FindAsync(id);
    return new RazorComponentResult<EditNote>(new { Note = note, Token = token });
});

app.MapGet("/notes/{id}/hide", async (ApplicationDbContext _applicationDbContext, int id) =>
{
    var note = await _applicationDbContext.Notes.FindAsync(id);
    return new RazorComponentResult<HideNote>(new { Note = note });
});

app.MapGet("/createNote", async (HttpContext context, IAntiforgery antiforgery) =>
{
    var token = antiforgery.GetAndStoreTokens(context);
    return new RazorComponentResult<CreateNote>(new { Token = token });
});

app.MapPost("/createNote",
    async (
        [FromForm] string title,
        [FromForm] string text,
        HttpContext context, 
        ApplicationDbContext _applicationDbContext) => {
    await _applicationDbContext.Notes.AddAsync(new Note { Title = title, Text = text, CreationDate = DateTimeOffset.Now.ToUniversalTime() });
    await _applicationDbContext.SaveChangesAsync();
    context.Response.Redirect("/");
});

app.MapGet("/search",
    async (HttpContext context, IAntiforgery antiforgery) => {
        var token = antiforgery.GetAndStoreTokens(context);
        return new RazorComponentResult<SearchBar>(new { Token = token });
    });

app.MapPost("/notes/search",
    async (
        [FromForm] string search,
        ApplicationDbContext _applicationDbContext) => {
        search = search.ToLower();

        // Retrieve notes where either the title or text contains the search string
        var notes = await _applicationDbContext.Notes
            .Where(note => note.Title.ToLower().Contains(search) || 
                           note.Text.ToLower().Contains(search))
            .ToListAsync();
        return new RazorComponentResult<NotesList>(new { Notes = notes });
    });

app.Run();

public class ApplicationState
{
    public int NotesCount { get; set; }
}