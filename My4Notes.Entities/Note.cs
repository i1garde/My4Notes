using System.Runtime.CompilerServices;

namespace My4Notes.Entities;

public class Note
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public DateTimeOffset CreationDate { get; set; }

    public static string formatCreationDate(Note note) =>
        $"Created {DateTimeOffset.Now.Day - note.CreationDate.Day} days ago";
}