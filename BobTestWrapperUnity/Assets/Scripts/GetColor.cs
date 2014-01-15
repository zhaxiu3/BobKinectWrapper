using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;
using System;

[StructLayout(LayoutKind.Sequential)]
public struct Vector4D {
    public float x;
    public float y;
    public float z;
    public float w;
}
public enum NUI_SKELETON_POSITION_TRACKING_STATE
{
    NUI_SKELETON_POSITION_NOT_TRACKED	= 0,
    NUI_SKELETON_POSITION_INFERRED,	
    NUI_SKELETON_POSITION_TRACKED	
}

public enum NUI_SKELETON_TRACKING_STATE
{	NUI_SKELETON_NOT_TRACKED	= 0,
    NUI_SKELETON_POSITION_ONLY	,
    NUI_SKELETON_TRACKED
}

[StructLayout(LayoutKind.Sequential)]
public struct NUI_SKELETON_DATA{   
    public NUI_SKELETON_TRACKING_STATE eTrackingState;
    public ulong dwTrackingID;
    public ulong dwEnrollmentIndex;
    public ulong dwUserIndex;
    public Vector4D Position;
    [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 20)]
    public Vector4[] SkeletonPositions;
    [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 20)]
    public NUI_SKELETON_POSITION_TRACKING_STATE[] eSkeletonPositionTrackingState;
    public ulong dwQualityFlags;
}

[StructLayout(LayoutKind.Explicit, Size = 8)]
public struct LARGE_INTEGER
{
    [FieldOffset(0)]
    public Int64 QuadPart;
    [FieldOffset(0)]
    public UInt32 LowPart;
    [FieldOffset(4)]
    public Int32 HighPart;
}

[StructLayout(LayoutKind.Sequential)]
public struct NUI_SKELETON_FRAME {
    public LARGE_INTEGER liTimeStamp;
    public ulong dwFrameNumber;
    public ulong dwFlags;
    public Vector4D vFloorClipPlane;
    public Vector4D vNormalToGravity;
    [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6)]
    public NUI_SKELETON_DATA[] SkeletonData;
}
public class GetColor : MonoBehaviour {

    public Renderer mcolorrend;
    public Renderer mdepthrend;

    Texture2D colortexture;
    Texture2D depthtexture;
    byte[] colorRawBytes = new byte[1228800];
    byte[] depthRawBytes = new byte[1228800];
    float[] skeletonRawFloats = new float[480];
    NUI_SKELETON_FRAME skeletonframe;

    Color32[] colors = new Color32[640 * 480];
    Color32[] depths = new Color32[640 * 480];
    [DllImport("BobKinectWrapper", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
    public extern static void StartKinect();

    [DllImport("BobKinectWrapper", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
    public extern static void PollColorData(byte[] ppColorBuffer, int BufferSize);

    [DllImport("BobKinectWrapper", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
    public extern static void PollDepthData(byte[] ppColorBuffer, int BufferSize);

    [DllImport("BobKinectWrapper", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
    public extern static void PollSkeletonData(float[] ppSkeletonBuffer, int BufferSize);

    [DllImport("BobKinectWrapper", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
    public extern static void PollSkeletonFrame(ref NUI_SKELETON_FRAME skeletonFrame);

    [DllImport("BobKinectWrapper", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
    public extern static void StopKinect();

	// Use this for initialization
	void Start () {

        colortexture = new Texture2D(640, 480, TextureFormat.RGBA32, false);
        depthtexture = new Texture2D(640, 480, TextureFormat.RGBA32, false);
        StartKinect();
	}

	// Update is called once per frame
	void Update () {
        GetColors();
        GetDepth();
        GetSkeleton();
        colortexture.SetPixels32(colors);
        colortexture.Apply();
        depthtexture.SetPixels32(depths);
        depthtexture.Apply();
        mcolorrend.material.mainTexture = colortexture;
        mdepthrend.material.mainTexture = depthtexture;
	}

    private void GetColors()
    {
        PollColorData(colorRawBytes, 1228800);
        int count = 0;
        for (int i = 479; i >= 0; i--)
        {
            for (int j = 0; j < 640; j++)
            {
                colors[i * 640 + j].b = colorRawBytes[count++];
                colors[i * 640 + j].g = colorRawBytes[count++];
                colors[i * 640 + j].r = colorRawBytes[count++];
                colors[i * 640 + j].a = colorRawBytes[count++];
            }
        }
    }
    private void GetDepth()
    {
        PollDepthData(depthRawBytes, 1228800);
        int count = 0;
        for (int i = 479; i >= 0; i--)
        {
            for (int j = 0; j < 640; j++)
            {
                depths[i * 640 + j].b = depthRawBytes[count++];
                depths[i * 640 + j].g = depthRawBytes[count++];
                depths[i * 640 + j].r = depthRawBytes[count++];
                depths[i * 640 + j].a = depthRawBytes[count++];
            }
        }
    }
    private void GetSkeleton() {
        PollSkeletonFrame(ref skeletonframe);
        PollSkeletonData(skeletonRawFloats, 480);
        string arrayStr = string.Empty;
        for (int i = 0; i < 480; i++) {
            arrayStr += skeletonRawFloats[i] + " ";
        }
        Debug.Log(arrayStr);

        string skeletonStr = string.Empty;
        skeletonStr += skeletonframe.dwFrameNumber+" ";
        for (int i = 0; i < 6; i++) {
            for (int j = 0; j < 20; j++) {
                skeletonStr += skeletonframe.SkeletonData[i].SkeletonPositions[j].x + " "
                    + skeletonframe.SkeletonData[i].SkeletonPositions[j].y + " "
                    + skeletonframe.SkeletonData[i].SkeletonPositions[j].z + " "
                    + skeletonframe.SkeletonData[i].SkeletonPositions[j].w + " ";
            }
        }
        Debug.Log(skeletonStr);
    }

    void OnDestroy() {
        Debug.Log("stopped");
        StopKinect();
    }
}
