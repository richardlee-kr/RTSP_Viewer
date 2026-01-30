using UnityEngine;
using System.Collections.Generic;

public class DisplayPage : MonoBehaviour
{
    [SerializeField] private GameObject displayPrefab;
    [SerializeField] private GameObject addDisplay;
    public GameObject holder;

    private List<GameObject> displaysInPage = new List<GameObject>();
    private PageManager pageManager;

    private void Update()
    {
        displaysInPage.RemoveAll(item => item == null);
    }

    public void SetManager(PageManager manager)
    {
        this.pageManager = manager;
    }

    public void AddDisplay(RTSP_Setting setting)
    {
        GameObject newDisplay = Instantiate(displayPrefab, holder.transform);
        newDisplay.name = $"{setting.title}";
        newDisplay.transform.GetChild(0).GetComponent<RTSP_Player>().Setup(setting, this);
        displaysInPage.Add(newDisplay);
        pageManager.AddDisplay(newDisplay);

        if(displaysInPage.Count == 6)
        {
            pageManager.AddNewPage();
        }
        CheckAddDisplayVisible();
    }
    public void AddDisplay(GameObject newDisplay)
    {
        displaysInPage.Add(newDisplay);
    }
    
    public void RemoveDisplay(GameObject player)
    {
        displaysInPage.Remove(player.transform.parent.gameObject);
        pageManager.RemoveDisplay(player.transform.parent.gameObject);
        Destroy(player.transform.parent.gameObject);

        pageManager.ReorderDisplays();
    }

    public void CheckAddDisplayVisible()
    {
        if(displaysInPage.Count < 6)
        {
            addDisplay.SetActive(true);
            addDisplay.transform.SetAsLastSibling();
        }
        else
        {
            addDisplay.SetActive(false);
        }
    }
    public void RequestOpeningAddDisplayPopup()
    {
        pageManager.OpenAddDisplayPopup();
    }


    public void ClearDisplayList()
    {
        displaysInPage.Clear();
    }
    public void ClearDisplayParent()
    {
        foreach(GameObject display in displaysInPage)
        {
            display.transform.SetParent(null);
        }
    }
    public int GetDisplayCount() => displaysInPage.Count;
    public List<GameObject> GetAllDisplays() => displaysInPage;
}
