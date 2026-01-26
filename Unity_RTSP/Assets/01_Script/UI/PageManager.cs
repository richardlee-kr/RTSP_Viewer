using UnityEngine;
using System.Collections.Generic;

public class PageManager : MonoBehaviour
{
    [SerializeField] List<DisplayPage> pages = new List<DisplayPage>();
    [SerializeField] List<GameObject> allDisplays = new List<GameObject>();

    [SerializeField] private GameObject displayPagePrefab;

    [SerializeField] private PageButtonManager buttonManager;
    [SerializeField] private GameObject addDisplayPopup;

    private int currentPageNum = 0;

    void Start()
    {
        AddFirstPage();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            AddNewPage();
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            DeletePage(pages.Count-1);
        }
    }

    private void AddFirstPage()
    {
        DisplayPage newPage = Instantiate(displayPagePrefab, transform).GetComponent<DisplayPage>();
        newPage.SetManager(this);
        pages.Add(newPage);

        buttonManager.AddButton();
    }
    public void AddNewPage()
    {
        if(pages.Count < 12)
        {
            DisplayPage newPage = Instantiate(displayPagePrefab, transform).GetComponent<DisplayPage>();
            newPage.gameObject.SetActive(false);
            newPage.SetManager(this);
            pages.Add(newPage);

            buttonManager.AddButton();
        }
        else
        {
            Debug.LogError("Cannot add page more than 12");
        }
    }
    public void DeletePage(int index)
    {
        buttonManager.RemoveButton();
        Destroy(pages[index].gameObject);
        pages.RemoveAt(index);
    }

    public void ChangePage(int index)
    {
        for(int i = 0; i < pages.Count; i++)
        {
            pages[i].gameObject.SetActive(false);
        }
        pages[index].gameObject.SetActive(true);
        currentPageNum = index;
    }

    public void AddDisplay(GameObject newDisplay)
    {
        allDisplays.Add(newDisplay);
    }
    public void RemoveDisplay(GameObject display)
    {
        allDisplays.Remove(display);
    }

    public void ReorderDisplays()
    {
        int _pageIndex = 0;

        foreach (DisplayPage page in pages)
        {
            page.ClearDisplayParent();
            page.ClearDisplayList();
        }

        for (int i = 0; i < allDisplays.Count; i++)
        {
            GameObject _display = allDisplays[i];
            _pageIndex = i / 6;

            if(_pageIndex >= pages.Count)
            {
                AddNewPage();
            }

            DisplayPage _page = pages[_pageIndex];

            _page.AddDisplay(_display);
            _display.transform.SetParent(_page.transform, false);
        }

        for (int i = pages.Count - 1; i >= 0; i--)
        {
            if (pages[i].GetDisplayCount() == 0 && pages.Count > 1)
            {
                DeletePage(i);
            }
        }

        if(pages[pages.Count-1].GetDisplayCount() == 6)
        {
            AddNewPage();
            ChangePage(pages.Count-1);
        }
        if (pages.Count == 0)
        {
            AddNewPage();
        }

        foreach(DisplayPage page in pages)
        {
            page.CheckAddDisplayVisible();
        }
    }

    public void OpenAddDisplayPopup() => addDisplayPopup.SetActive(true);

    public DisplayPage GetCurrentPage() => pages[currentPageNum];
}
