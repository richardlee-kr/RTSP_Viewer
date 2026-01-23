using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PageButtonManager : MonoBehaviour
{
    [SerializeField] List<Button> pageButtons = new List<Button>();
    [SerializeField] private PanelManager panelManager;

    private int pageCount = 0;
    public void AddButton()
    {
        int _count = pageCount;
        pageButtons[pageCount].onClick.AddListener(() => RequestChangePage(_count));
        pageButtons[pageCount].gameObject.SetActive(true);
        pageCount++;
    }
    public void RemoveButton()
    {
        pageButtons[--pageCount].gameObject.SetActive(false);
    }

    private void RequestChangePage(int index)
    {
        panelManager.ChangePage(index);
    }
}
