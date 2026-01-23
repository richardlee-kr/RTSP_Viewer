using UnityEngine;
using System.Collections.Generic;

public class PanelManager : MonoBehaviour
{
    [SerializeField] List<RTSP_PlayerHolder> playerHolder = new List<RTSP_PlayerHolder>();

    private int currentPageNum = 0;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public RTSP_PlayerHolder GetCurrentPlayerHolder() => playerHolder[currentPageNum];
}
