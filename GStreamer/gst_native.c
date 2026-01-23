#include <stdio.h>
#include <gst/gst.h>
#include <gst/app/gstappsink.h>

typedef struct
{
    GstElement* pipeline;
    GstElement* appsink;
    GstSample* sample;
    GstMapInfo map;
} MyGstContext;

static char error_buffer[1024];

static int gst_initialized = 0;

//char* IP_ADDRESS = "192.168.1.45";
//char* PORT = "8554";
//char* STREAMING_PATH = "vlc";

//static gchar* pipelineStr = "videotestsrc ! videoconvert ! appsink name=mysink";
//static gchar* pipelinStr = "rtspsrc location=rtsp://127.0.0.1:8554/vlc latency=200 " "! decodebin " "! videoconvert ! video/x-raw,format=RGBA " "! appsink name=mysink sync=false max-buffers=1 drop=true";

__declspec(dllexport)
MyGstContext* CreatePipeline(const char* rtspurl, int width, int height)
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

    if(!gst_initialized)
    {
        gst_init(NULL, NULL);
        gst_initialized = 1;
    }

    char pipelineStr[512];

    snprintf(
        pipelineStr,
        sizeof(pipelineStr),
        "rtspsrc location=rtsp://%s latency=200 "
        "! decodebin "
        "! videoconvert "
        "! videoscale "
        "! video/x-raw,width=%d,height=%d,format=BGRA " //크기 설정, BGRA 포맷
        "! appsink name=mysink sync=false max-buffers=1 drop=true", //최신 프레임만 유지해서 지연 누적 방지
        rtspurl,
        width,
        height
    );

    GError* error = NULL;
    MyGstContext* ctx = (MyGstContext*)calloc(1, sizeof(MyGstContext));

    ctx->pipeline = gst_parse_launch(pipelineStr, &error);

    if (!ctx->pipeline)
    {
        free(ctx);
        return NULL;
    }

    ctx->appsink = gst_bin_get_by_name(GST_BIN(ctx->pipeline), "mysink");
    if (!ctx->appsink)
    {
        gst_object_unref(ctx->pipeline);
        free(ctx);
        return NULL;
    }

    gst_element_set_state(ctx->pipeline, GST_STATE_PLAYING);
    return ctx;
}

__declspec(dllexport)
unsigned char* GetFrame(MyGstContext* ctx, int* width, int* height)
{
    if (!ctx || !ctx->appsink)
        return NULL;

    ctx->sample = gst_app_sink_try_pull_sample(
        GST_APP_SINK(ctx->appsink), GST_MSECOND * 5
    );

    if (!ctx->sample)
        return NULL;

    GstBuffer* buffer = gst_sample_get_buffer(ctx->sample);

    GstCaps* caps = gst_sample_get_caps(ctx->sample);
    GstStructure* s = gst_caps_get_structure(caps, 0);

    gst_structure_get_int(s, "width", width);
    gst_structure_get_int(s, "height", height);

    if (!gst_buffer_map(buffer, &ctx->map, GST_MAP_READ))
    {
        gst_sample_unref(ctx->sample);
        ctx->sample = NULL;
        return NULL;
    }
    return ctx->map.data; // RGBA raw pointer
}

__declspec(dllexport)
void ReleaseFrame(MyGstContext* ctx)
{
    if (!ctx || !ctx->sample)
        return;

    GstBuffer* buffer = gst_sample_get_buffer(ctx->sample);
    gst_buffer_unmap(buffer, &ctx->map);

    gst_sample_unref(ctx->sample);
    ctx->sample = NULL;
}

__declspec(dllexport)
void DestroyPipeline(MyGstContext* ctx)
{
    if (!ctx)
        return;

    if (ctx->appsink)
    {
        gst_object_unref(ctx->appsink);
        ctx->appsink = NULL;
    }

    if (ctx->pipeline)
    {
        gst_element_set_state(ctx->pipeline, GST_STATE_NULL);
        gst_object_unref(ctx->pipeline);
        ctx->pipeline = NULL;
    }
    
    free(ctx);
}