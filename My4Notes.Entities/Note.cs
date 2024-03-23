using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace My4Notes.Entities;

/// <summary>
/// Represents a note in the application.
/// </summary>
public class Note
{
    public int Id { get; set; }
    [StringLength(255)]
    public string? Title { get; set; }
    public string? Text { get; set; }
    public DateTime CreationDate { get; set; }
    
    /// <summary>
    /// Formats the creation date of a note into a relative time string.
    /// </summary>
    /// <param name="note">The note whose creation date is to be formatted.</param>
    /// <returns>A string representing the relative time since the note was created.</returns>
    public static string formatCreationDate(Note note)
    {
        var creationTimeSpanUTC = DateTime.UtcNow - note.CreationDate;
        var minutes = (int)creationTimeSpanUTC.TotalMinutes;
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