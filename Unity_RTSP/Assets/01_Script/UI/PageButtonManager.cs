using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class PageButtonManager : MonoBehaviour
{
    [SerializeField] List<Button> pageButtons = new List<Button>();
    [SerializeField] private PageManager pageManager;

    [SerializeField] private Color activeColor = new Color(1f,0.5f,0f);
    [SerializeField] private Color inactiveColor = Color.black;

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
        pageManager.ChangePage(index);
        SetCurrentPageButtonColor(index);
    }

    public void SetCurrentPageButtonColor(int index)
    {
        for (int i = 0; i < pageButtons.Count; i++)
        {
            TMP_Text buttonText = pageButtons[i].GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.color = (i == index) ? activeColor : inactiveColor;
            }
        }
    }
}
