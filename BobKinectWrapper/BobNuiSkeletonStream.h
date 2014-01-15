#pragma once
#include "bobnuistream.h"
class CBobNuiSkeletonStream :
	public CBobNuiStream
{
public:
	CBobNuiSkeletonStream(INuiSensor* psensor);
	~CBobNuiSkeletonStream(void);

	virtual HRESULT OpenStream();

	virtual void PauseStream();

	virtual void CloseStream();

};

