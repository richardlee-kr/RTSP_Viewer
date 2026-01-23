using UnityEngine;
using System.Collections.Generic;

public class PanelManager : MonoBehaviour
{
    [SerializeField] List<PageManager> pages = new List<PageManager>();

    private int currentPageNum = 0;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public PageManager GetCurrentPlayerHolder() => pages[currentPageNum];
}
