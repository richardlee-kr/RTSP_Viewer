using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RTSP_StateController : MonoBehaviour
{
    private RTSP_State currentState;

    [SerializeField] private Image stateImage;
    [SerializeField] private TMP_Text urlText;
    [SerializeField] private TMP_Text titleText;

    [SerializeField] private GameObject loadingScreenImage;
    [SerializeField] private GameObject whiteScreenImage;

    public void SetTitleText(string newTitle)
    {
        titleText.text = newTitle;
    }
    public void SetUrlText(string url)
    {
        urlText.text = $"rtsp://{url}";
    }
    public void UpdateState(RTSP_State state)
    {
        currentState = state;
        UpdateUI();
    }

    private void UpdateUI()
    {
        UpdateStateImage();
    }

    private void UpdateStateImage()
    {
        switch(currentState)
        {
            case RTSP_State.connected:
                stateImage.color = new Color32(0,255,0,255);
                loadingScreenImage.SetActive(false);
                whiteScreenImage.SetActive(false);
                break;
            case RTSP_State.reconnecting:
                stateImage.color = new Color32(255,255,0,127);
                loadingScreenImage.SetActive(true);
                whiteScreenImage.SetActive(false);
                break;
            case RTSP_State.disconnected:
                loadingScreenImage.SetActive(false);
                whiteScreenImage.SetActive(true);
                stateImage.color = new Color32(127,127,127,255);
                break;
            default:
                break;
        }
    }

}

public enum RTSP_State
{
    connected,
    reconnecting,
    disconnected,
}