using Microsoft.EntityFrameworkCore;
using My4Notes.Entities;

namespace My4Notes.DatabaseAccess;

public class ApplicationDbContext : DbContext
{
    private string _connectionString = "Host=localhost;Port=5433;Database=My4NotesDB;Username=postgres;Password=Str0ngP@ssw0rd";
    
    public ApplicationDbContext() {}
        
    public ApplicationDbContext(string connString)
    {
        _connectionString = connString;
        Database.EnsureCreated();
    }
    public virtual DbSet<Note> Notes { get; set; }

    // TODO: expose secrets outside of the app as environmental variables
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseNpgsql(_connectionString);
    }
}