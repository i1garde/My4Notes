using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace My4Notes.Tests;

public class E2ETests : IDisposable
{
    private readonly IWebDriver _driver;
    private readonly string currentProjectURL = "http://localhost:5189";

    public E2ETests()
    {
        _driver = new ChromeDriver();
    }

    [Fact]
    public void CreateANewNote()
    {
        _driver.Navigate().GoToUrl(currentProjectURL);

        _driver.FindElement(By.Id("createNoteButton")).Click();
        
        Thread.Sleep(1000);
        
        _driver.FindElement(By.Id("titleInput")).SendKeys("Test Title");
        _driver.FindElement(By.Id("textInput")).SendKeys("Test Text");
        _driver.FindElement(By.Id("submitButton")).Click();
        
        Thread.Sleep(1000);
        
        _driver.FindElement(By.Id("viewNoteButton")).Click();
        
        Thread.Sleep(1000);
        
        var getTitle = _driver.FindElement(By.Id("noteItemTitle")).Text;
        var getText = _driver.FindElement(By.TagName("textarea")).Text;
        
        Assert.Equal("Test Title", getTitle); Assert.Equal("Test Text", getText);
        
        _driver.FindElement(By.Id("hideButton")).Click();
        
        Thread.Sleep(1000);

        DeleteNoteHelper();
    }
    
    [Fact]
    public void ViewNote()
    {
        _driver.Navigate().GoToUrl(currentProjectURL);
        CreateNoteHelper("Test Title", "Test Text");
        
        Thread.Sleep(500);
        
        _driver.FindElement(By.Id("viewNoteButton")).Click();
        
        Thread.Sleep(500);
        
        var getTitle = _driver.FindElement(By.Id("noteItemTitle")).Text;
        var getText = _driver.FindElement(By.TagName("textarea")).Text;
        
        Assert.Equal("Test Title", getTitle); 
        Assert.Equal("Test Text", getText);

        DeleteNoteHelper();
    }
    
    [Fact]
    public void DeleteANewNote()
    {
        _driver.Navigate().GoToUrl(currentProjectURL);
        CreateNoteHelper("Test Title", "Test Text");

        IWebElement noteContainer = _driver.FindElement(By.CssSelector("div[id^='noteContainer']"));
        var createdContainerId = noteContainer.GetAttribute("id");
        
        _driver.FindElement(By.Id("editNoteButton")).Click();
        
        Thread.Sleep(500);
        
        _driver.FindElement(By.Id("deleteButton")).Click();
        
        Thread.Sleep(500);
        
        IWebElement checkNoteContainer = _driver.FindElement(By.CssSelector("div[id^='noteContainer']"));
        var checkDeletedId = checkNoteContainer.GetAttribute("id");
        
        Assert.NotEqual(createdContainerId, checkDeletedId);
    }
    
    [Fact]
    public void TotalNotesCounter()
    {
        _driver.Navigate().GoToUrl(currentProjectURL);
        var getNotesCounter = _driver.FindElement(By.Id("totalNotesCounter")).Text.Substring(13);
        var numCounter = Convert.ToInt32(getNotesCounter);
        
        CreateNoteHelper("Test Title", "Test Text");
        
        var afterAddingCounter = _driver.FindElement(By.Id("totalNotesCounter")).Text.Substring(13);
        var numAfterAddingCounter = Convert.ToInt32(afterAddingCounter);
        
        Assert.Equal(numCounter + 1, numAfterAddingCounter);

        DeleteNoteHelper();
    }
    
    [Fact]
    public void UpdateNote()
    {
        _driver.Navigate().GoToUrl(currentProjectURL);
        CreateNoteHelper("Test Title", "Test Text");
        
        _driver.FindElement(By.Id("editNoteButton")).Click();
        
        Thread.Sleep(500);

        _driver.FindElement(By.Id("editTitleInput")).Clear();
        _driver.FindElement(By.Id("editTitleInput")).SendKeys("Updated Test Title");
        _driver.FindElement(By.Id("editTextInput")).Clear();
        _driver.FindElement(By.Id("editTextInput")).SendKeys("Updated Test Text");
        
        _driver.FindElement(By.Id("saveButton")).Click();
        
        Thread.Sleep(500);
        
        _driver.FindElement(By.Id("viewNoteButton")).Click();
        
        Thread.Sleep(500);
        
        var getTitle = _driver.FindElement(By.Id("noteItemTitle")).Text;
        var getText = _driver.FindElement(By.TagName("textarea")).Text;
        
        Assert.Equal("Updated Test Title", getTitle); 
        Assert.Equal("Updated Test Text", getText);

        DeleteNoteHelper();
    }
    
    [Fact]
    public void SearchNoteInSearchBar()
    {
        _driver.Navigate().GoToUrl(currentProjectURL);
        CreateNoteHelper("Test Title", "Test Text");
        CreateNoteHelper("Interesting Test Title", "Specific Test Text");
        CreateNoteHelper("Unlikely to match", "That's it");
        
        _driver.FindElement(By.Id("searchBarWidget")).SendKeys("match");
        
        Thread.Sleep(1000);
        
        _driver.FindElement(By.Id("viewNoteButton")).Click();
        Thread.Sleep(500);
        
        var getTitle = _driver.FindElement(By.Id("noteItemTitle")).Text;
        var getText = _driver.FindElement(By.TagName("textarea")).Text;
        
        Assert.Equal("Unlikely to match", getTitle); 
        Assert.Equal("That's it", getText);

        DeleteNoteHelper();
        DeleteNoteHelper();
        DeleteNoteHelper();
    }
    
    [Fact]
    public void PaginationCorrectNumberOfPages()
    {
        _driver.Navigate().GoToUrl(currentProjectURL);
        for (int i = 0; i < 24; i++)
        {
            CreateNoteHelper($"Test Title {i}", $"Test Text {i}");
        }
        
        var page1 = _driver.FindElement(By.Id("pagination-li-1"));
        var page2 = _driver.FindElement(By.Id("pagination-li-2"));
        var page3 = _driver.FindElement(By.Id("pagination-li-3"));
        
        Assert.NotNull(page1);
        Assert.NotNull(page2);
        Assert.NotNull(page3);
        
        for (int i = 0; i < 24; i++)
        {
            DeleteNoteHelper();
        }
    }
    
    [Fact]
    public void PaginationAppearesWhenNotesNumberMoreThanEight()
    {
        _driver.Navigate().GoToUrl(currentProjectURL);
        for (int i = 0; i < 9; i++)
        {
            CreateNoteHelper($"Test Title {i}", $"Test Text {i}");
        }
        
        var pagination = _driver.FindElement(By.Id("pagination-nav"));
        
        Assert.NotNull(pagination);
        
        for (int i = 0; i < 9; i++)
        {
            DeleteNoteHelper();
        }
    }
    
    [Fact]
    public void PaginationNumberPageSwitchesWorksCorrectly()
    {
        _driver.Navigate().GoToUrl(currentProjectURL);
        for (int i = 0; i < 10; i++)
        {
            CreateNoteHelper($"Test Title {i}", $"Test Text {i}");
        }
        
        IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
        js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
        Thread.Sleep(500);
        
        _driver.FindElement(By.Id("pagination-button-2")).Click();
        Thread.Sleep(500);
        
        var secondPageNote = _driver.FindElement(By.Id("noteItemTitle")).Text;
        
        js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
        Thread.Sleep(500);
        
        _driver.FindElement(By.Id("pagination-button-1")).Click();
        Thread.Sleep(500);
        
        var firstPageNote = _driver.FindElement(By.Id("noteItemTitle")).Text;
        
        Assert.Equal($"Test Title 1", secondPageNote);
        Assert.Equal($"Test Title 9", firstPageNote);
        
        for (int i = 0; i < 10; i++)
        {
            DeleteNoteHelper();
        }
    }
    
    [Fact]
    public void PaginationPreviousNextPageSwitchesWorksCorrectly()
    {
        _driver.Navigate().GoToUrl(currentProjectURL);
        for (int i = 0; i < 10; i++)
        {
            CreateNoteHelper($"Test Title {i}", $"Test Text {i}");
        }
        
        IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
        js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
        Thread.Sleep(500);
        
        _driver.FindElement(By.Id("pagination-button-next")).Click();
        Thread.Sleep(500);
        
        var secondPageNote = _driver.FindElement(By.Id("noteItemTitle")).Text;
        
        js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
        Thread.Sleep(500);
        
        _driver.FindElement(By.Id("pagination-button-prev")).Click();
        Thread.Sleep(500);
        
        var firstPageNote = _driver.FindElement(By.Id("noteItemTitle")).Text;
        
        Assert.Equal($"Test Title 1", secondPageNote);
        Assert.Equal($"Test Title 9", firstPageNote);
        
        for (int i = 0; i < 10; i++)
        {
            DeleteNoteHelper();
        }
    }

    public void Dispose()
    {
        _driver.Quit();
        _driver.Dispose();
    }

    private void CreateNoteHelper(string title, string text)
    {
        _driver.FindElement(By.Id("createNoteButton")).Click();
        
        Thread.Sleep(1000);
        
        _driver.FindElement(By.Id("titleInput")).SendKeys(title);
        _driver.FindElement(By.Id("textInput")).SendKeys(text);
        _driver.FindElement(By.Id("submitButton")).Click();
        
        Thread.Sleep(500);
    }
    
    private void DeleteNoteHelper()
    {
        _driver.FindElement(By.Id("editNoteButton")).Click();
        
        Thread.Sleep(500);
        
        _driver.FindElement(By.Id("deleteButton")).Click();
        
        Thread.Sleep(500);
    }
}