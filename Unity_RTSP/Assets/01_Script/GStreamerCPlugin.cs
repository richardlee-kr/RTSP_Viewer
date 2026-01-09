using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class GStreamerCPlugin : MonoBehaviour
{
     // ===== Native DLL =====
    [DllImport("gst_native", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr InitPipelineWithSize(int width, int height);

    [DllImport("gst_native", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr GetFrame(ref int width, ref int height);

    [DllImport("gst_native", CallingConvention = CallingConvention.Cdecl)]
    private static extern void ReleaseFrame();

    [DllImport("gst_native", CallingConvention = CallingConvention.Cdecl)]
    private static extern void StopPipeline();

    // ===== Unity =====
    public RawImage targetRawImage;

    private Texture2D videoTexture;
    private int texWidth = 0;
    private int texHeight = 0;

    void Start()
    {
        // RawImage 실제 픽셀 크기 계산
        RectTransform rt = targetRawImage.rectTransform;
        Vector2 size = rt.rect.size;
        float scale = rt.lossyScale.x;

        int width = Mathf.RoundToInt(size.x * scale);
        int height = Mathf.RoundToInt(size.y * scale);

        IntPtr msgPtr = InitPipelineWithSize(width, height);
        string msg = Marshal.PtrToStringAnsi(msgPtr);

        Debug.Log($"[GStreamer] {msg} ({width}x{height})");
    }

    void Update()
    {
        int w = 0, h = 0;
        IntPtr dataPtr = GetFrame(ref w, ref h);

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

    void OnDestroy()
    {
        StopPipeline();
    }
}
