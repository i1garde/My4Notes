namespace My4Notes.Web.Components.Services;

public class SearchService
{
    private string searchText;
    
    public event Action<string> OnSearchChange;
    
    public string SearchText
    {
        get => searchText;
        set
        {
            if (searchText != value)
            {
                searchText = value;
                NotifySearchTermChanged();
            }
        }
    }
    
    private void NotifySearchTermChanged()
    {
        OnSearchChange?.Invoke(searchText);
    }
}