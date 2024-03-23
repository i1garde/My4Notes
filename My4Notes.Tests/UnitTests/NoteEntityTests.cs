using My4Notes.Entities;

namespace My4Notes.Tests;

public class NoteEntityTests
{
    [Fact]
    public void FormatCreationDate_ReturnsJustNow()
    {
        // Arrange
        var note = new Note { CreationDate = DateTime.UtcNow };

        // Act
        var result = Note.formatCreationDate(note);

        // Assert
        Assert.Equal("Just now", result);
    }

    [Fact]
    public void FormatCreationDate_Returns30MinutesAgo()
    {
        // Arrange
        var note = new Note { CreationDate = DateTime.UtcNow.AddMinutes(-30) };

        // Act
        var result = Note.formatCreationDate(note);

        // Assert
        Assert.Equal("30 minutes ago", result);
    }

    [Fact]
    public void FormatCreationDate_Returns2HoursAgo()
    {
        // Arrange
        var note = new Note { CreationDate = DateTime.UtcNow.AddMinutes(-120) };

        // Act
        var result = Note.formatCreationDate(note);

        // Assert
        Assert.Equal("2 hours ago", result);
    }

    [Fact]
    public void FormatCreationDate_Returns3DaysAgo()
    {
        // Arrange
        var note = new Note { CreationDate = DateTime.UtcNow.AddDays(-3) };

        // Act
        var result = Note.formatCreationDate(note);

        // Assert
        Assert.Equal("3 days ago", result);
    }
}