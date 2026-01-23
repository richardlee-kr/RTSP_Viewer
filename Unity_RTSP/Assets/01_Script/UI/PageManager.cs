using UnityEngine;

public class PageManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject addDisplay;

    private int playerCount;

    public void AddPlayer(RTSP_Setting setting)
    {
        GameObject newPlayer = Instantiate(playerPrefab, transform);
        newPlayer.transform.GetChild(0).GetComponent<RTSP_Player>().Setup(setting, this);
        
        playerCount++;
        if(playerCount == 6)
        {
            addDisplay.SetActive(false);
        }
        else
        {
            addDisplay.transform.SetAsLastSibling();
        }
    }
    public void RemovePlayer()
    {
        playerCount--;
        if(playerCount < 6)
        {
            addDisplay.transform.SetAsLastSibling();
            addDisplay.SetActive(true);
        }
    }
}
