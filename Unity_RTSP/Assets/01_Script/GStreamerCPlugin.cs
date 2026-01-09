using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class GStreamerCPlugin : MonoBehaviour
{
    [DllImport("gst_native")]
    static extern IntPtr InitPipeline();

    [DllImport("gst_native")]
    static extern IntPtr GetFrame(ref int w, ref int h);

    [DllImport("gst_native")]
    static extern void ReleaseFrame();

    Texture2D tex;

    void Start()
    {
        var result = InitPipeline();
        Debug.Log(Marshal.PtrToStringAnsi(result));
    }

    void Update()
    {
        int w = 0, h = 0;
        IntPtr ptr = GetFrame(ref w, ref h);

        if (ptr != IntPtr.Zero)
        {
            if (tex == null)
                tex = new Texture2D(w, h, TextureFormat.RGBA32, false);

            tex.LoadRawTextureData(ptr, w * h * 4);
            tex.Apply();

            GetComponent<Renderer>().material.mainTexture = tex;
            ReleaseFrame();
        }
    }
}
