#pragma once

#include "Define.h"
#include "NuiApi.h"
#include <thread>

class BobKinectWrapper
{

	static const int cDepthWidth = 640;
	static const int cDepthHeight = 480;
	static const int cStatusMessageMaxLen = MAX_PATH*2;	
	
public:
	
	static BYTE* ColorBuffer;
	static BYTE* DepthBuffer;

	BobKinectWrapper(void);
	~BobKinectWrapper(void);

	int Run(HINSTANCE hInstance, int nCmdShow);

private:
	INuiSensor * m_pNuiSensor;
	HANDLE                  m_pColorStreamHandle;
	HANDLE                  m_hNextColorFrameEvent;
	
    HANDLE                  m_pDepthStreamHandle;
    HANDLE                  m_hNextDepthFrameEvent;

	void                    Update();
	HRESULT                 CreateFirstConnected();
	void                    ProcessColor();
	void                    ProcessDepth();
};

static BobKinectWrapper* sgkinect;
static bool isRunning = true;
static std::thread* sgupdateProc;
_EXTERN_C_ void StartKinect();
_EXTERN_C_ void PollColorData(BYTE* ppColorBuffer, int BufferSize);
_EXTERN_C_ void PollDepthData(BYTE* ppDepthBuffer, int BufferSize);
_EXTERN_C_ void StopKinect();

