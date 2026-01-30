using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage), typeof(RTSP_StateController))]
public class DuplicatedPlayer : MonoBehaviour
{
    private RawImage targetRawImage;
    private RTSP_Player originalPlayer;

    //FPS timer
    private float frameInterval;
    private float renderTimer;

    //Component
    private RTSP_StateController controller;

    public void Setup(RTSP_Player target)
    {
        this.originalPlayer = target;
        Initialize();
    }

    private void Update()
    {
        UpdateTimer();
        CheckTimer();
    }

    private void UpdateTimer()
    {
        renderTimer += Time.deltaTime;
    }

    private void CheckTimer()
    {
        if(renderTimer >= frameInterval)
        {
            renderTimer = 0f;
            UpdateTexture();
        }
    }

    private void UpdateTexture()
    {
        targetRawImage.texture = originalPlayer.GetTexture();
        controller.UpdateState(originalPlayer.controller.GetCurrentState());
    }

    private void Initialize()
    {
        SetRequireComponent();
        SetImage();
    }

    private void SetRequireComponent()
    {
        controller = GetComponent<RTSP_StateController>();
    }
    private void SetImage()
    {
        if(targetRawImage == null)
        {
            if(TryGetComponent<RawImage>(out RawImage image))
            {
                targetRawImage = image;
            }
            else
            {
                Debug.LogError($"No RawImage in {gameObject.name}");
                return;
            }
        }
        //flip image
        RawImage img = targetRawImage;
        img.uvRect = new Rect(0, 1, 1, -1);
    }

    public void ManualReconnect()
    {
        originalPlayer.ManualReconnect();
    }
}
