using Microsoft.EntityFrameworkCore;
using My4Notes.Entities;

namespace My4Notes.DatabaseAccess;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
        Database.EnsureCreated();
    }
    public DbSet<Note> Notes { get; set; }

    // TODO: expose secrets outside of the app as environmental variables
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=My4NotesDB;Username=postgres;Password=Str0ngP@ssw0rd");
    }
}