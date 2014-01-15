#include "BobNuiStream.h"



CBobNuiStream::CBobNuiStream(INuiSensor* psensor)
	:mhStreamReadyEvent(NULL),
	mpsensor(psensor)
{
	if(mpsensor){
		mpsensor->AddRef();
	}
	mhStreamReadyEvent = CreateEvent(NULL, TRUE, FALSE, NULL);
}


CBobNuiStream::~CBobNuiStream(void)
{
	CloseHandle(mhStreamReadyEvent);
	SafeRelease(mpsensor);
}

HRESULT CBobNuiStream::OpenStream()
{

}

void CBobNuiStream::PauseStream()
{

}
