using UnityEngine;
using System.Collections.Generic;

public class PanelManager : MonoBehaviour
{
    [SerializeField] List<PageManager> pages = new List<PageManager>();

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
            DeleteLastPage();
        }
    }

    private void AddFirstPage()
    {
        PageManager newPage = Instantiate(displayPagePrefab, transform).GetComponent<PageManager>();
        newPage.SetManager(this);
        pages.Add(newPage);

        buttonManager.AddButton();
    }
    public void AddNewPage()
    {
        if(pages.Count < 12)
        {
            PageManager newPage = Instantiate(displayPagePrefab, transform).GetComponent<PageManager>();
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
    public void DeleteLastPage()
    {
        buttonManager.RemoveButton();
        Destroy(pages[pages.Count-1].gameObject);
        pages.RemoveAt(pages.Count-1);
    }

    public void ChangePage(int index)
    {
        for(int i = 0; i < pages.Count; i++)
        {
            pages[i].gameObject.SetActive(false);
        }
        pages[index].gameObject.SetActive(true);
    }

    public void OpenAddDisplayPopup() => addDisplayPopup.SetActive(true);

    public PageManager GetCurrentPlayerHolder() => pages[currentPageNum];
}
