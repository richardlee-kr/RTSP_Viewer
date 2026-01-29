using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class GStreamerCPlugin : MonoBehaviour
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
    public RawImage targetRawImage;

    [Header("RTSP streaming address")]
    public string rtsp_url;

    private Texture2D videoTexture;
    private int texWidth = 0;
    private int texHeight = 0;

    static bool envInitialized = false;

    IntPtr ctx;

    void Awake()
    {
        if(envInitialized)
            return;

        // ===== 1. GStreamer plugin 경로 설정 (상대경로) =====
        /*
        #if UNITY_EDITOR
            string pluginPath = Path.Combine(Application.dataPath, "Plugins/x86_64/gstreamer-1.0");
        #else
            string pluginPath = Path.Combine(Application.dataPath, "Plugins/x86_64");
        #endif

        string pluginPath = Path.Combine(Application.dataPath, "Plugins/x86_64/gstreamer-1.0");
        Environment.SetEnvironmentVariable(
            "GST_PLUGIN_PATH",
            pluginPath
        );
        */

        /*
        // ===== 2. GStreamer DLL (bin) 경로 PATH에 추가 =====
        string binPath = System.IO.Path.Combine(
            Application.dataPath,
            "Plugins/x86_64"
        );

        string currentPath = Environment.GetEnvironmentVariable("PATH");
        if (!currentPath.Contains(binPath))
        {
            Environment.SetEnvironmentVariable(
                "PATH",
                binPath + ";" + currentPath
            );
        }
        */

        envInitialized = true;

        //Debug.Log("[GStreamer] GST_PLUGIN_PATH = " + pluginPath);
        //Debug.Log("[GStreamer] PATH += " + binPath);
    }

    void Start()
    {
        // ===== 3. RawImage 실제 픽셀 크기 계산 =====
        RectTransform rt = targetRawImage.rectTransform;

        Vector2 size = rt.rect.size;
        float scale = rt.lossyScale.x;

        int width = Mathf.RoundToInt(size.x * scale);
        int height = Mathf.RoundToInt(size.y * scale);

        Debug.Log($"[GStreamer] Target size = {width} x {height}");

        // ===== 4. Native pipeline 초기화 =====
        ctx = CreatePipeline(rtsp_url, width, height);
        RawImage img = targetRawImage;
        img.uvRect = new Rect(0, 1, 1, -1);
    }

    void Update()
    {
        int w = 0, h = 0;
        IntPtr dataPtr = GetFrame(ctx, ref w, ref h);

        if (dataPtr == IntPtr.Zero || w == 0 || h == 0)
            return;

        // Texture 생성 (최초 1회)
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

        // Native → Unity 복사
        videoTexture.LoadRawTextureData(dataPtr, texWidth * texHeight * 4);
        videoTexture.Apply(false);

        ReleaseFrame(ctx);
    }

    void OnDestroy()
    {
        DestroyPipeline(ctx);
    }
}
