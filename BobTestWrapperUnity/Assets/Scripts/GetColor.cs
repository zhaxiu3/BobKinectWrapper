using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;

public class GetColor : MonoBehaviour {

    public Renderer mcolorrend;
    public Renderer mdepthrend;

    Texture2D colortexture;
    Texture2D depthtexture;
    byte[] colorRawBytes = new byte[1228800];
    byte[] depthRawBytes = new byte[1228800];
    Color32[] colors = new Color32[640 * 480];
    Color32[] depths = new Color32[640 * 480];
    [DllImport("BobKinectWrapper", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
    public extern static void StartKinect();

    [DllImport("BobKinectWrapper", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
    public extern static void PollColorData(byte[] ppColorBuffer, int BufferSize);

    [DllImport("BobKinectWrapper", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
    public extern static void PollDepthData(byte[] ppColorBuffer, int BufferSize);

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

    void OnDestroy() {
        Debug.Log("stopped");
        StopKinect();
    }
}
