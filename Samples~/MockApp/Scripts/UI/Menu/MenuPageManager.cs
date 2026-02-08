using System.Collections.Generic;
using UnityEngine;

public abstract class MenuPageManager<T> : MonoBehaviour where T : MenuPage
{
    [Header("Body Objects")]
    [SerializeField] 
    protected T[] _menuPages;
    
    private int _currentMenuPage = -1;
    private List<int> _pageHistory;
    private int _maxPageHistoryLength;
    
    protected virtual void Start()
    {
        _maxPageHistoryLength = _menuPages.Length;
        _pageHistory = new List<int>(_maxPageHistoryLength + 1);
            
        OpenPage(0);
    }

    public void OpenPage(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex >= _menuPages.Length)
        {
            Debug.LogWarning($"Invalid page index detected: {pageIndex}. Must be between 0 and {_menuPages.Length - 1}.");
            return;
        }

        if (pageIndex == _currentMenuPage)
        {
            Debug.Log($"Transitioning to the same page: {_menuPages[pageIndex].name}. No action taken.");
            return;
        }
        
        _pageHistory.RemoveAll(page => page >= pageIndex);
        _pageHistory.Add(_currentMenuPage);
        
        if (_pageHistory.Count > _maxPageHistoryLength)
            _pageHistory.RemoveAt(0);
                
        if (_currentMenuPage != -1)
            _menuPages[_currentMenuPage].Close();
        
        _currentMenuPage = pageIndex;
        _menuPages[_currentMenuPage].Open();
    }

    public void OpenPageByName(string pageName)
    {
        for (int i = 0; i < _menuPages.Length; i++)
        {
            if (_menuPages[i].name == pageName)
            {
                OpenPage(i);
                return;
            }
        }
        
        Debug.LogWarning($"No page found with the name: {pageName}");
    }

    public void PreviousPage()
    {
        if (_pageHistory.Count == 0)
        {
            Debug.Log("No previous page in history to navigate to.");
            return;
        }

        int previousPageIndex = _pageHistory[_pageHistory.Count - 1];
        _pageHistory.RemoveAt(_pageHistory.Count - 1);

        if (previousPageIndex == -1)
        {
            Debug.LogWarning("Previous page index is invalid (-1). Cannot navigate back.");
            return;
        }
        
        _menuPages[_currentMenuPage].Close();
        _currentMenuPage = previousPageIndex;
        _menuPages[_currentMenuPage].Open();
    }
}
