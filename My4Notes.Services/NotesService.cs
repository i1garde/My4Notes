using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;

namespace My4Notes.Services
{
    /// <summary>
    /// Provides methods to interact with notes via the application's database.
    /// </summary>
    public class NotesService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMemoryCache _cache;

        public NotesService(ApplicationDbContext applicationDbContext, IMemoryCache cache)
        {
            _applicationDbContext = applicationDbContext;
            _cache = cache;
        }

        /// <summary>
        /// Retrieves the number of notes stored in the database asynchronously.
        /// </summary>
        /// <returns>The number of notes stored in the database.</returns>
        public async Task<int> GetNotesCountAsync()
        {
            var notesCount = await _cache.GetOrCreateAsync("notesCount", async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(1);
                return await _applicationDbContext.Notes.AsNoTracking().CountAsync();
            });

            Console.WriteLine($"IN-MEMORY CACHE: {notesCount}");

            return notesCount;
            
            //return await _applicationDbContext.Notes.AsNoTracking().CountAsync();
        }

        /// <summary>
        /// Retrieves a note from the database asynchronously by its ID.
        /// </summary>
        /// <param name="id">The ID of the note to retrieve.</param>
        /// <returns>The note with the specified ID, or null if not found.</returns>
        public async Task<Note> GetNoteByIdAsync(int id)
        {
            var note = await _applicationDbContext.Notes.FindAsync(id);
            return note!;
        }

        /// <summary>
        /// Asynchronously adds a note to the database.
        /// </summary>
        /// <param name="note">The note to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task AddNoteAsync(Note note)
        {
            _applicationDbContext.Notes.Add(note);
            await _applicationDbContext.SaveChangesAsync();
            _cache.Remove("notes");
            _cache.Remove("notesCount");
        }

        /// <summary>
        /// Asynchronously updates a note in the database.
        /// </summary>
        /// <param name="note">The note to update.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task UpdateNoteAsync(Note note)
        {
            _applicationDbContext.Notes.Update(note);
            await _applicationDbContext.SaveChangesAsync();
            _cache.Remove("notes");
            _cache.Remove("notesCount");
        }

        /// <summary>
        /// Asynchronously retrieves all notes from the database.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of notes or null.</returns>
        public async Task<List<Note>?> GetNotesAsync()
        {
            return await _cache.GetOrCreateAsync("notes", async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(5);
                return await _applicationDbContext.Notes.AsNoTracking().OrderByDescending(x => x.CreationDate).ToListAsync();
            });
            // return await _applicationDbContext.Notes.AsNoTracking().OrderByDescending(x => x.CreationDate).ToListAsync();
        }

        /// <summary>
        /// Asynchronously retrieves notes from the database that contain the specified search string in their title or text.
        /// </summary>
        /// <param name="search">The string to search for in the note title and text.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of notes that match the search criteria.</returns>
        public async Task<List<Note>> GetNotesByTitleAsync(string search)
        {
            var loweredSerach = search.ToLower();
            return await _applicationDbContext.Notes.AsNoTracking()
                .Where(n => n.Title.ToLower().Contains(loweredSerach) || n.Text.ToLower().Contains(loweredSerach))
                .ToListAsync();
        }
    }
}