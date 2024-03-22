using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
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

        public NotesService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        /// <summary>
        /// Retrieves the number of notes stored in the database asynchronously.
        /// </summary>
        /// <returns>The number of notes stored in the database.</returns>
        public async Task<int> GetNotesCountAsync()
        {
            return await _applicationDbContext.Notes.CountAsync();
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
        }

        /// <summary>
        /// Asynchronously retrieves all notes from the database.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of notes.</returns>
        public async Task<List<Note>> GetNotesAsync()
        {
            return await _applicationDbContext.Notes.ToListAsync();
        }

        /// <summary>
        /// Asynchronously retrieves notes from the database that contain the specified search string in their title or text.
        /// </summary>
        /// <param name="search">The string to search for in the note title and text.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of notes that match the search criteria.</returns>
        public async Task<List<Note>> GetNotesByTitleAsync(string search)
        {
            var loweredSerach = search.ToLower();
            return await _applicationDbContext.Notes
                .Where(n => n.Title.ToLower().Contains(loweredSerach) || n.Text.ToLower().Contains(loweredSerach))
                .ToListAsync();
        }
    }
}
