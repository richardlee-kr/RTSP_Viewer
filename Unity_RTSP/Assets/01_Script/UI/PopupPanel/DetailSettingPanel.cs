using UnityEngine;
using TMPro;

public class DetailSettingPanel : MonoBehaviour
{
    //TODO: 새로 연결하지 말고 원래 있는 Raw Texture 가져오기
    [SerializeField] private RTSP_Player fullscreenPlayer;
    private RTSP_Player targetPlayer;

    private RTSP_Setting setting;

    [Header("UI")]
    [SerializeField] private TMP_InputField titleInput;
    [SerializeField] private TMP_InputField ipInput;
    [SerializeField] private TMP_InputField portInput;
    [SerializeField] private TMP_InputField pathInput;
    [SerializeField] private TMP_InputField fpsInput;

    private void OnEnable()
    {
        SetPanel();
    }

    public void SetTarget(RTSP_Player target)
    {
        targetPlayer = target;
    }

    private void SetPanel()
    {
        GetSettings();
        SetDisplay();
    }

    private void GetSettings()
    {
        setting = targetPlayer.GetSetting();

        titleInput.text = setting.title;
        ipInput.text = setting.ip;
        portInput.text = setting.port;
        pathInput.text = setting.path;
        fpsInput.text = $"{setting.fps}";
    }
    private void SetDisplay()
    {
        fullscreenPlayer.Setup(setting, null);
    }

    public void Apply()
    {
        setting.title = titleInput.text;
        setting.ip = ipInput.text;
        setting.port = portInput.text;
        setting.path = pathInput.text;
        setting.fps = int.Parse(fpsInput.text);

        targetPlayer.Setup(setting);
        fullscreenPlayer.Setup(setting, null);
    }
    public void OK()
    {
        Apply();
        this.gameObject.SetActive(false);
    }
}
