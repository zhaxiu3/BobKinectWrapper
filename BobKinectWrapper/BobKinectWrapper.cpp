#include "BobKinectWrapper.h"

BYTE* BobKinectWrapper::ColorBuffer;
BYTE* BobKinectWrapper::DepthBuffer;

BobKinectWrapper::BobKinectWrapper(void) : m_pNuiSensor(NULL)
{
	CreateFirstConnected();
	isRunning = true;
}



BobKinectWrapper::~BobKinectWrapper(void)
{
	if (m_pNuiSensor)
	{
		m_pNuiSensor->NuiShutdown();
    }

	if (m_hNextColorFrameEvent != INVALID_HANDLE_VALUE)
    {
        CloseHandle(m_hNextColorFrameEvent);
    }
	
    SafeRelease(m_pNuiSensor);
	
	delete[] ColorBuffer;
	ColorBuffer = NULL;
	delete[] DepthBuffer;
	DepthBuffer = NULL;
	//MessageBox(NULL, _T("exit"),_T("exit"),0);

}

void BobKinectWrapper::Update(){
	if(NULL == m_pNuiSensor){
		return;
	}
	
	if(WAIT_OBJECT_0 == WaitForSingleObject(m_hNextColorFrameEvent, 0)){
		ProcessColor();
        ProcessDepth();
	}
}


int BobKinectWrapper::Run(HINSTANCE hInstance, int nCmdShow){
	const int eventCount = 1;
	HANDLE hEvents[eventCount];
	while(isRunning){
		
	//MessageBox(NULL, _T("update"),_T("update"),0);
		hEvents[0] = m_hNextColorFrameEvent;
		DWORD dwEvent = MsgWaitForMultipleObjects(eventCount, hEvents, FALSE, INFINITE, QS_ALLINPUT);
		if (WAIT_OBJECT_0 == dwEvent)
        {
            Update();
        }
		
	}
	return 0;
}

HRESULT BobKinectWrapper::CreateFirstConnected(){
	INuiSensor *pNuiSensor;
	HRESULT hr;

	int iSensorCount = 0;
	hr = NuiGetSensorCount(&iSensorCount);
	if(FAILED(hr)){
		return hr;
	}

	for(int i = 0; i < iSensorCount; ++i){
		hr = NuiCreateSensorByIndex(i, &pNuiSensor);
		if(FAILED(hr)){
			continue;
		}

		hr = pNuiSensor->NuiStatus();
		if(S_OK == hr){
			m_pNuiSensor = pNuiSensor;
			break;
		}

		pNuiSensor->Release();
	}

	if(NULL != m_pNuiSensor){
		hr = m_pNuiSensor->NuiInitialize(NUI_INITIALIZE_FLAG_USES_COLOR|NUI_INITIALIZE_FLAG_USES_DEPTH_AND_PLAYER_INDEX);
		if(SUCCEEDED(hr)){
			m_hNextColorFrameEvent = CreateEvent(NULL, TRUE, FALSE, NULL);

			hr = m_pNuiSensor->NuiImageStreamOpen(
				NUI_IMAGE_TYPE_COLOR,
				NUI_IMAGE_RESOLUTION_640x480,
				0,
				2,
				m_hNextColorFrameEvent,
				&m_pColorStreamHandle);
		}
		if(SUCCEEDED(hr)){
			m_hNextDepthFrameEvent = CreateEvent(NULL, TRUE, FALSE, NULL);

			hr = m_pNuiSensor->NuiImageStreamOpen(
				NUI_IMAGE_TYPE_DEPTH_AND_PLAYER_INDEX,
				NUI_IMAGE_RESOLUTION_640x480,
				0,
				2,
				m_hNextDepthFrameEvent,
				&m_pDepthStreamHandle);
		}
	}

	if(NULL == m_pNuiSensor || FAILED(hr)){
		MessageBox(NULL, _T("no ready Kinect Found"),_T("no ready Kinect Found"),0);
		return E_FAIL;
	}

	return hr;
}


void BobKinectWrapper::ProcessColor(){
	HRESULT hr;
	//MessageBox(NULL, _T("processingColor"),_T("processingColor"),0);
	NUI_IMAGE_FRAME imageFrame;
	hr = m_pNuiSensor->NuiImageStreamGetNextFrame(m_pColorStreamHandle, 0, &imageFrame);
    if (FAILED(hr))
    {
        return;
    }

	INuiFrameTexture * pTexture = imageFrame.pFrameTexture;	
    NUI_LOCKED_RECT LockedRect;
	pTexture->LockRect(0, &LockedRect, NULL, 0);
	if(NULL == ColorBuffer){
		ColorBuffer = new BYTE[LockedRect.size];
	}
	if(LockedRect.Pitch != 0){
		memcpy_s(ColorBuffer, LockedRect.size, LockedRect.pBits,LockedRect.size);
	}

	TCHAR buffer[1024];
	//wsprintf(buffer,_T("%d"), LockedRect.size);
	//MessageBox(NULL, _T("processingColor"),buffer,0);
	pTexture->UnlockRect(0);
	m_pNuiSensor->NuiImageStreamReleaseFrame(m_pColorStreamHandle, &imageFrame);

}

void BobKinectWrapper::ProcessDepth(){	
    HRESULT hr;
    NUI_IMAGE_FRAME imageFrame;

	hr = m_pNuiSensor->NuiImageStreamGetNextFrame(m_pDepthStreamHandle,0, &imageFrame);
	if(FAILED(hr)){
		return;
	}

	BOOL nearMode;
	INuiFrameTexture * pTexture;
	hr = m_pNuiSensor->NuiImageFrameGetDepthImagePixelFrameTexture(
		m_pDepthStreamHandle, &imageFrame,&nearMode, &pTexture);
	if(FAILED(hr)){		
		m_pNuiSensor->NuiImageStreamReleaseFrame(m_pDepthStreamHandle, &imageFrame);
		return;
	}

	NUI_LOCKED_RECT LockedRect;

	pTexture->LockRect(0, &LockedRect, NULL,0);

	if(NULL == DepthBuffer){
		DepthBuffer = new BYTE[LockedRect.size];
	}

	if(LockedRect.Pitch != 0){
		memcpy_s(DepthBuffer, LockedRect.size, LockedRect.pBits, LockedRect.size);
	}

	pTexture->UnlockRect(0);
	pTexture->Release();

	m_pNuiSensor->NuiImageStreamReleaseFrame(m_pDepthStreamHandle, &imageFrame);

}

void runProc(){
	sgkinect->Run(NULL , 0);

}
void StartKinect(){
	sgkinect = new BobKinectWrapper();	
	sgupdateProc = new std::thread(runProc);
}


void PollColorData(BYTE* ppColorBuffer, int BufferSize){
	if(NULL ==sgkinect->ColorBuffer)
		return;
	memcpy_s(ppColorBuffer, BufferSize, sgkinect->ColorBuffer,BufferSize);
}

void PollDepthData(BYTE* ppDepthBuffer, int BufferSize){
	if(NULL ==sgkinect->DepthBuffer)
		return;
	memcpy_s(ppDepthBuffer, BufferSize, sgkinect->DepthBuffer,BufferSize);
}

void StopKinect(){
	delete sgkinect;
	isRunning = false;
}