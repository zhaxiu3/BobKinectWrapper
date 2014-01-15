//Define.h
///////////////////////////////////////////
//////////////////////////////////////////
#ifndef _DEFINE_H_
#define _DEFINE_H_
#define _EXTERN_C_  extern "C"  _declspec(dllexport)
#endif

#include <windows.h>
#include <tchar.h>
#include <strsafe.h>


#include "NuiApi.h"

template<class Interface>
inline void SafeRelease( Interface *& pInterfaceToRelease )
{
    if ( pInterfaceToRelease != NULL )
    {
        pInterfaceToRelease->Release();
        pInterfaceToRelease = NULL;
    }
}