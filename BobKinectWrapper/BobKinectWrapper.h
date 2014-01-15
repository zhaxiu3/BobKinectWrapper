#pragma once

#include "Define.h"
#include <thread>

typedef struct _BobStruct{
	float a[100];
	int b[100];
}BobStruct;

class BobKinectWrapper
{

	static const int cDepthWidth = 640;
	static const int cDepthHeight = 480;
	static const int cStatusMessageMaxLen = MAX_PATH*2;	
	
public:
	
	static BYTE* ColorBuffer;
	static BYTE* DepthBuffer;
	static FLOAT SkeletonBuffer[NUI_SKELETON_COUNT * NUI_SKELETON_POSITION_COUNT*4];
	static NUI_SKELETON_FRAME SkeletonFrameBuffer;

	BobKinectWrapper(void);
	~BobKinectWrapper(void);

	int Run(HINSTANCE hInstance, int nCmdShow);

private:
	INuiSensor * m_pNuiSensor;
	HANDLE                  m_pColorStreamHandle;
	HANDLE                  m_hNextColorFrameEvent;
	
    HANDLE                  m_pDepthStreamHandle;
    HANDLE                  m_hNextDepthFrameEvent;

	HANDLE                  m_pSkeletonStreamHandle;
	HANDLE                  m_hNextSkeletonEvent;

	void                    Update();
	HRESULT                 CreateFirstConnected();
	void                    ProcessColor();
	void                    ProcessDepth();
	void                    ProcessSkeleton();
};

static BobKinectWrapper* sgkinect;
static bool isRunning = true;
static std::thread* sgupdateProc;
_EXTERN_C_ void StartKinect();
_EXTERN_C_ void PollColorData(BYTE* ppColorBuffer, int BufferSize);
_EXTERN_C_ void PollDepthData(BYTE* ppDepthBuffer, int BufferSize);
_EXTERN_C_ void PollSkeletonData(float SkeletonBuffer[], int BufferSize);
_EXTERN_C_ void PollSkeletonFrame(BobStruct* skeletonFrame);
_EXTERN_C_ void StopKinect();

