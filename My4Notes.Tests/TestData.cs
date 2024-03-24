using My4Notes.Entities;

namespace My4Notes.Tests;

public static class TestData
{
    public static List<Note> Notes = new List<Note>
    {
        new Note { Id = 1, Title = "Title 1", Text = "Text 1", CreationDate = DateTime.UtcNow },
        new Note { Id = 2, Title = "Title 2", Text = "Text 2", CreationDate = DateTime.UtcNow.AddMinutes(-7) },
        new Note { Id = 3, Title = "Title 3", Text = "Text 3", CreationDate = DateTime.UtcNow.AddMinutes(-30) },
        new Note { Id = 4, Title = "Title 4", Text = "Text 4", CreationDate = DateTime.UtcNow.AddHours(-3) },
        new Note { Id = 5, Title = "Title 5", Text = "Text 5", CreationDate = DateTime.UtcNow.AddDays(-2) },
    };

    // Add more test data if necessary
}