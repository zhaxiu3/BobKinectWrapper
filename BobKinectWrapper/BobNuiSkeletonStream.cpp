#include "BobNuiSkeletonStream.h"


CBobNuiSkeletonStream::CBobNuiSkeletonStream(INuiSensor* psensor)
	:CBobNuiStream(psensor)
{
}


CBobNuiSkeletonStream::~CBobNuiSkeletonStream(void)
{
}

HRESULT CBobNuiSkeletonStream::OpenStream()
{
	throw std::logic_error("The method or operation is not implemented.");
}

void CBobNuiSkeletonStream::PauseStream()
{
	throw std::logic_error("The method or operation is not implemented.");
}

void CBobNuiSkeletonStream::CloseStream()
{
	throw std::logic_error("The method or operation is not implemented.");
}
