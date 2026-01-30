using System;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage), typeof(RTSP_StateController))]
public class RTSP_Player : MonoBehaviour
{
     // ===== Native DLL =====
    [DllImport("gst_native", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr CreatePipeline(string url, int width, int height);

    [DllImport("gst_native", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr GetFrame(IntPtr ctx, ref int width, ref int height);

    [DllImport("gst_native", CallingConvention = CallingConvention.Cdecl)]
    private static extern void ReleaseFrame(IntPtr ctx);

    [DllImport("gst_native", CallingConvention = CallingConvention.Cdecl)]
    private static extern void DestroyPipeline(IntPtr ctx);


    // ===== Unity =====
    private RawImage targetRawImage;
    IntPtr ctx;

    private DisplayPage page;

    [SerializeField] private RTSP_Setting setting;
    private string rtsp_url;

    //FPS timer
    private float frameInterval;
    private float renderTimer;

    //재연결 timer
    private bool isReconnecting = false;
    private float reconnectTimer = 0f;
    private float timeout = 1f;

    private int reconnectCounter = 0;

    //Texture
    private Texture2D videoTexture;
    private int texWidth = 0;
    private int texHeight = 0;

    //Component
    [HideInInspector] public RTSP_StateController controller;

    void Start()
    {
        //Initialize();
        //StartPipeline();
    }

    void Update()
    {
        UpdateTimer();
        CheckTimer();
    }

    public Texture2D GetTexture()
    {
        return videoTexture;
    }

    public void StartPipeline()
    {
        StartPipeline(rtsp_url);
    }
    public void StartPipeline(string url)
    {
        //RawImage 실제 픽셀 계산
        RectTransform rt = targetRawImage.rectTransform;

        Vector2 size = rt.rect.size;
        float scale = rt.lossyScale.x;

        int width = Mathf.RoundToInt(size.x * scale);
        int height = Mathf.RoundToInt(size.y * scale);

        //Debug.Log($"{gameObject.name} tries to start RTSP pipeline.\n Target size = {width} x {height}");

        //GStreamer pipeline 초기화
        ctx = CreatePipeline(url, width, height);
    }

    public void ManualReconnect() //by button
    {
        ReconnectRTSP();
        reconnectCounter = 0;
    }
    private void ReconnectRTSP()
    {
        if(isReconnecting)
        {
            return;
        }
        
        controller.UpdateState(RTSP_State.reconnecting);
        isReconnecting = true;
        StartCoroutine(ReconnectCoroutine());
    }

    public void Setup(RTSP_Setting newSetting, DisplayPage page = null)
    {
        this.setting = newSetting;
        if(page != null)
            this.page = page;
        Initialize();
        StartPipeline();
    }

    private void UpdateTexture()
    {
        int w = 0, h = 0;
        IntPtr dataPtr = GetFrame(ctx, ref w, ref h);

        if(dataPtr == IntPtr.Zero || w <= 0 || h <= 0)
        {
            return;
        }

        reconnectTimer = 0f;
        reconnectCounter = 0;

        controller.UpdateState(RTSP_State.connected);

        //videoTexture 생성 (최초 1회)
        if (videoTexture == null)
        {
            texWidth = w;
            texHeight = h;

            videoTexture = new Texture2D(
                texWidth,
                texHeight,
                TextureFormat.BGRA32,
                false
            );

            videoTexture.wrapMode = TextureWrapMode.Clamp;
            videoTexture.filterMode = FilterMode.Bilinear;

            targetRawImage.texture = videoTexture;
        }

        videoTexture.LoadRawTextureData(dataPtr, texWidth * texHeight * 4);
        videoTexture.Apply(false, false);

        ReleaseFrame(ctx);
    }

    private void SafeDestroyPipeline()
    {
        if(ctx != IntPtr.Zero)
        {
            DestroyPipeline(ctx);
            ctx = IntPtr.Zero;
        }
    }

    //URL 조합
    private string CombineUrl()
    {
        return $"{setting.ip}:{setting.port}/{setting.path.TrimStart('/')}";
    }
    
    private IEnumerator ReconnectCoroutine()
    {
        SafeDestroyPipeline();
        yield return null;
        StartPipeline(rtsp_url);

        reconnectTimer = 0f;
        renderTimer = 0f;

        reconnectCounter++;

        isReconnecting = false;
    }

    private void Initialize()
    {
        SetRequireComponent();
        SetUI();
        SetImage();
        SetFPS();
    }

    private void SetRequireComponent()
    {
        controller = GetComponent<RTSP_StateController>();
    }
    private void SetUI()
    {
        rtsp_url = CombineUrl();
        controller.SetUrlText(rtsp_url);
        controller.SetTitleText(setting.title);
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
    private void SetFPS()
    {
        frameInterval = 1f / setting.fps;
    }

    private void UpdateTimer()
    {
        renderTimer += Time.deltaTime;
        reconnectTimer += Time.deltaTime;
    }
    private void CheckTimer()
    {
        if(renderTimer >= frameInterval)
        {
            renderTimer = 0f;
            UpdateTexture();
        }

        if(reconnectTimer >= timeout && reconnectCounter < 20)
        {
            ReconnectRTSP();
        }

        if(reconnectCounter >= 20)
        {
            controller.UpdateState(RTSP_State.disconnected);
        }
    }

    public void RemoveSelf()
    {
        page.RemoveDisplay(this.gameObject);
    }

    void OnDestroy()
    {
        SafeDestroyPipeline();
    }
    public RTSP_Setting GetSetting() => setting;
}

[Serializable]
public class RTSP_Setting
{
    public string title;
    public string ip;
    public string port;
    public string path;
    public int fps;
}