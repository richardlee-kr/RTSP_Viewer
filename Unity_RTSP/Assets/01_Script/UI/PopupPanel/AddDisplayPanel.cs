using UnityEngine;
using TMPro;

public class AddDisplayPanel : MonoBehaviour
{
    [SerializeField] private PageManager pageManager;

    [SerializeField] private TMP_InputField titleInput;
    [SerializeField] private TMP_InputField ipInput;
    [SerializeField] private TMP_InputField portInput;
    [SerializeField] private TMP_InputField pathInput;
    [SerializeField] private TMP_InputField fpsInput;
    

    private RTSP_Setting CreateSetting()
    {
        RTSP_Setting _setting = new RTSP_Setting();
        _setting.title = titleInput.text;
        _setting.ip = ipInput.text;
        _setting.port = portInput.text;
        _setting.path = pathInput.text;
        _setting.fps = int.Parse(fpsInput.text);

        return _setting;
    }

    public void RequestAddingPlayer()
    {
        RTSP_Setting _setting = CreateSetting();
        if(_setting == null)
        {
            Debug.LogError("Wrong input for create player.");
            return;
        }
        else
        {
            pageManager.GetCurrentPage().AddDisplay(_setting);
            gameObject.SetActive(false);
        }
    }
}
