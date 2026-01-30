using UnityEngine;
using System.Collections.Generic;

public class PageManager : MonoBehaviour
{
    [SerializeField] List<DisplayPage> pages = new List<DisplayPage>();
    [SerializeField] List<GameObject> allDisplays = new List<GameObject>();

    [SerializeField] private GameObject displayPagePrefab;

    [SerializeField] private GameObject addDisplayPopup;
    [SerializeField] private PageButtonManager buttonManager;

    private int currentPageIndex = 0;

    void Start()
    {
        AddFirstPage();
    }

    void Update()
    {

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
        currentPageIndex = index;
    }

    public void NextPage()
    {
        if(currentPageIndex+1 < pages.Count)
            ChangePage(currentPageIndex+1);
        buttonManager.SetCurrentPageButtonColor(currentPageIndex);
    }
    public void PrevPage()
    {
        if(currentPageIndex > 0)
            ChangePage(currentPageIndex-1);
        buttonManager.SetCurrentPageButtonColor(currentPageIndex);
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
            _display.transform.SetParent(_page.holder.transform, false);
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
            ChangePage(currentPageIndex);
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


    public void SwapDisplays(GameObject displayA, GameObject displayB)
    {
        int indexA = allDisplays.IndexOf(displayA);
        int indexB = allDisplays.IndexOf(displayB);

        if (indexA < 0 || indexB < 0) return;

        allDisplays[indexA] = displayB;
        allDisplays[indexB] = displayA;
    }

    public void OpenAddDisplayPopup() => addDisplayPopup.SetActive(true);

    public DisplayPage GetCurrentPage() => pages[currentPageIndex];
}
