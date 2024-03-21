using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using My4Notes.DatabaseAccess;
using My4Notes.Entities;

namespace My4Notes.Services
{
    public class NotesService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public NotesService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<int> GetNotesCountAsync()
        {
            return await _applicationDbContext.Notes.CountAsync();
        }

        public async Task<Note> GetNoteByIdAsync(int id)
        {
            var note = await _applicationDbContext.Notes.FindAsync(id);
            return note!;
        }

        public async Task AddNoteAsync(Note note)
        {
            _applicationDbContext.Notes.Add(note);
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task UpdateNoteAsync(Note note)
        {
            _applicationDbContext.Notes.Update(note);
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task<List<Note>> GetNotesAsync()
        {
            return await _applicationDbContext.Notes.ToListAsync();
        }

        public async Task<List<Note>> GetNotesByTitleAsync(string search)
        {
            var loweredSerach = search.ToLower();
            return await _applicationDbContext.Notes
                .Where(n => n.Title.ToLower().Contains(loweredSerach) || n.Text.ToLower().Contains(loweredSerach))
                .ToListAsync();
        }
    }
}
