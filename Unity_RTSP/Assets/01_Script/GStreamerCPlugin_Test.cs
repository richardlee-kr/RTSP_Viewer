using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class GStreamerCPlugin_Test : MonoBehaviour
{
    [DllImport("gst_native")]
    private static extern IntPtr InitPipeline();

    void Start()
    {
        var result = InitPipeline();
        Debug.Log(Marshal.PtrToStringAnsi(result));
    }
}
