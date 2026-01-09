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
    private static extern IntPtr InitPipelineWithSize(string url, int width, int height);

    [DllImport("gst_native", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr GetFrame(ref int width, ref int height);

    [DllImport("gst_native", CallingConvention = CallingConvention.Cdecl)]
    private static extern void ReleaseFrame();

    [DllImport("gst_native", CallingConvention = CallingConvention.Cdecl)]
    private static extern void StopPipeline();

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

        // ===== 4. Native pipeline 초기화 =====
        IntPtr msgPtr = InitPipelineWithSize(url, width, height);
        string msg = Marshal.PtrToStringAnsi(msgPtr);

        Debug.Log("[GStreamer] " + msg);
    }
    private void ReconnectRTSP()
    {
        ChangeRTSPAddress(rtsp_url);
    }
    public void ChangeRTSPAddress(string url)
    {
        StopPipeline();
        StartPipeline(url);
    }

    private void UpdateTexture()
    {
        int w = 0, h = 0;
        IntPtr dataPtr = GetFrame(ref w, ref h);

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

            ReleaseFrame();
        }
        else
        {
            if (Time.time - lastFrameTime > timeout)
            {
                Debug.Log($"[GStreamer] No frames for {timeout}s, restarting pipeline...");
                ChangeRTSPAddress(rtsp_url);
                lastFrameTime = Time.time;
            }
        }

    }

    private string CombineUrl()
    {
        return $"{rtsp_address}:{rtsp_port}/{rtsp_path.TrimStart('/')}";
    }
    
    public IEnumerator ReconnectCoroutine()
    {
        isReconnecting = true;

        StopPipeline();
        yield return null;
        ReconnectRTSP();

        isReconnecting = false;
    }

    void OnDestroy()
    {
        StopPipeline();
    }
}