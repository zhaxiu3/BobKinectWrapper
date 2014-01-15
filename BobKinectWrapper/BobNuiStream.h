#pragma once

#include "Define.h"

class CBobNuiStream
{
protected:
	HANDLE mhStreamReadyEvent;
	INuiSensor* mpsensor;
public:
	CBobNuiStream(INuiSensor* sensor);
	~CBobNuiStream(void);
	
	virtual HRESULT OpenStream();
	virtual void PauseStream();
	virtual void CloseStream();
};

