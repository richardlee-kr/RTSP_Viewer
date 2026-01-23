using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RTSP_StateController : MonoBehaviour
{
    private RTSP_State currentState;

    [SerializeField] private Image stateImage;
    [SerializeField] private TMP_Text urlText;
    [SerializeField] private TMP_Text titleText;

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
                break;
            case RTSP_State.disconnected:
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
    disconnected,
}