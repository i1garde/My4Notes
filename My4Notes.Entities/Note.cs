using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace My4Notes.Entities;

public class Note
{
    public int Id { get; set; }
    [StringLength(255)]
    public string? Title { get; set; }
    public string? Text { get; set; }
    public DateTime CreationDate { get; set; }

    /// <summary>
    /// Formats the creation date of the note to display the number of days since the note was created.
    /// </summary>
    /// <param name="note">The note whose creation date to format.</param>
    /// <returns>A string that represents the number of days since the note was created.</returns>
    public static string formatCreationDate(Note note)
    {
        var creationTimeSpanUTC = DateTime.UtcNow - note.CreationDate;
        var minutes = creationTimeSpanUTC.Minutes;
        string result = minutes switch
        {
            var m when m < 1 => "Just now",
            var m when m < 60 => $"{m} minutes ago",
            var m when m < 1440 => $"{m / 60} hours ago",
            _ => $"{minutes / 1440} days ago"
        };
        return result;
    }
}