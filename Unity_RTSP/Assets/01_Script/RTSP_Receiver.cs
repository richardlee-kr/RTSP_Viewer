using System;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class RTSP_Receiver : MonoBehaviour
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

    IntPtr ctx;

    // ===== Unity =====
    public RawImage targetRawImage;

    [Header("RTSP streaming url")]
    public string rtsp_address;
    public string rtsp_port;
    public string rtsp_path;
    private string rtsp_url;

    private bool isReconnecting = false;
    private float lastFrameTime = 0f;
    private float timeout = 1f;

    private Texture2D videoTexture;
    private int texWidth = 0;
    private int texHeight = 0;

    void Start()
    {
        StartPipeline();
    }

    void Update()
    {
        UpdateTexture();
    }

    public void StartPipeline()
    {
        rtsp_url = CombineUrl();
        StartPipeline(rtsp_url);
    }
    public void StartPipeline(string url)
    {
        // ===== RawImage 실제 픽셀 크기 계산 =====
        RectTransform rt = targetRawImage.rectTransform;

        Vector2 size = rt.rect.size;
        float scale = rt.lossyScale.x;

        int width = Mathf.RoundToInt(size.x * scale);
        int height = Mathf.RoundToInt(size.y * scale);

        Debug.Log($"[GStreamer] Target size = {width} x {height}");

        // ===== pipeline 초기화 =====
        ctx = CreatePipeline(url, width, height);
    }
    public void ReconnectRTSP()
    {
        StartCoroutine(ReconnectCoroutine());
    }
    public void ChangeRTSPAddress(string url)
    {
        DestroyPipeline(ctx);
        StartPipeline(url);
    }

    private void UpdateTexture()
    {
        int w = 0, h = 0;
        IntPtr dataPtr = GetFrame(ctx, ref w, ref h);

        if(dataPtr != IntPtr.Zero || w > 0 || h > 0)
        {
            lastFrameTime = Time.time;

            // Texture 생성 (최초 1회)
            if (videoTexture == null)
            {
                texWidth = w;
                texHeight = h;

                videoTexture = new Texture2D(
                    texWidth,
                    texHeight,
                    TextureFormat.RGBA32,
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
        else
        {
            if (Time.time - lastFrameTime > timeout)
            {
                Debug.Log($"[GStreamer] No frames for {timeout}s, restarting pipeline...");
                ReconnectRTSP();
                lastFrameTime = Time.time;
            }
        }

    }

    private string CombineUrl()
    {
        return $"{rtsp_address}:{rtsp_port}/{rtsp_path.TrimStart('/')}";
    }
    
    private IEnumerator ReconnectCoroutine()
    {
        isReconnecting = true;

        DestroyPipeline(ctx);
        yield return null;
        StartPipeline(rtsp_url);

        isReconnecting = false;
    }

    void OnDestroy()
    {
        DestroyPipeline(ctx);
    }
}