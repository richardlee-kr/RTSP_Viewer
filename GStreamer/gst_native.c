#include <stdio.h>
#include <gst/gst.h>
#include <gst/app/gstappsink.h>

static GstElement* pipeline = NULL;
static GstElement* appsink = NULL;
static GstSample* current_sample = NULL;
static GstMapInfo current_map;

static char error_buffer[1024];

//static gchar* pipelineStr = "videotestsrc ! videoconvert ! appsink name=mysink";
//static gchar* pipelinStr = "rtspsrc location=rtsp://127.0.0.1:8554/vlc latency=200 " "! decodebin " "! videoconvert ! video/x-raw,format=RGBA " "! appsink name=mysink sync=false max-buffers=1 drop=true";

__declspec(dllexport)
const char* InitPipelineWithSize(int width, int height)
{
    /*
    g_setenv(
        "GST_PLUGIN_PATH",
        "C:/Unity_git/RTSP/Unity_RTSP/Assets/Plugins/x86_64/gstreamer-1.0/lib/gstreamer-1.0",
        TRUE
    );
    g_setenv(
        "PATH",
        "C:/Unity_git/RTSP/Unity_RTSP/Assets/Plugins/x86_64;"
        "C:/Unity_git/RTSP/Unity_RTSP/Assets/Plugins/x86_64/gstreamer-1.0/bin",
        TRUE
    );
    */

    gst_init(NULL, NULL);

    char pipelineStr[512];

    snprintf(
        pipelineStr,
        sizeof(pipelineStr),
        "videotestsrc "
        "! videoconvert "
        "! videoscale "
        "! videoflip method=vertical-flip "
        "! video/x-raw,width=%d,height=%d,format=RGBA "
        "! appsink name=mysink sync=false max-buffers=1 drop=true",
        width,
        height
    );

    GError* error = NULL;

    pipeline = gst_parse_launch(pipelineStr, &error);

    if (!pipeline)
    {
        if(error)
        {
            snprintf(error_buffer, sizeof(error_buffer), "pipeline error: %s", error->message);
            g_error_free(error);
            return error_buffer;
        }
        else
        {
            return "unknown pipeline error";
        }
        return "pipeline is NULL";
    }

    appsink = gst_bin_get_by_name(GST_BIN(pipeline), "mysink");
    if (!appsink)
    {
        return "appsink is NULL";
    }

    gst_element_set_state(pipeline, GST_STATE_PLAYING);
    return "pipeline created";
}

__declspec(dllexport)
unsigned char* GetFrame(int* width, int* height)
{
    if (!appsink)
        return NULL;

    current_sample = gst_app_sink_try_pull_sample(
        GST_APP_SINK(appsink), 0
    );

    if (!current_sample)
        return NULL;

    GstCaps* caps = gst_sample_get_caps(current_sample);
    GstStructure* s = gst_caps_get_structure(caps, 0);

    gst_structure_get_int(s, "width", width);
    gst_structure_get_int(s, "height", height);

    GstBuffer* buffer = gst_sample_get_buffer(current_sample);

    if (!gst_buffer_map(buffer, &current_map, GST_MAP_READ))
        return NULL;

    return current_map.data; // RGBA raw pointer
}

__declspec(dllexport)
void ReleaseFrame(void)
{
    if (!current_sample)
        return;

    GstBuffer* buffer = gst_sample_get_buffer(current_sample);
    gst_buffer_unmap(buffer, &current_map);

    gst_sample_unref(current_sample);
    current_sample = NULL;
}

__declspec(dllexport)
void StopPipeline(void)
{
    if (!pipeline)
        return;

    gst_element_set_state(pipeline, GST_STATE_NULL);
    gst_object_unref(pipeline);
    pipeline = NULL;
}