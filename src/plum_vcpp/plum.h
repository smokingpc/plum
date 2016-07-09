#ifndef         _LIB_PLUM_

#define         _LIB_PLUM_
#pragma comment(lib, "dbghelp.lib")


#ifdef LIB_PLUM_EXPORTS
#define LIB_PLUM_API __declspec(dllexport)
#elif defined(LIB_PLUM_EXPORTS_STATIC)
#define LIB_PLUM_API 
#else
#define LIB_PLUM_API __declspec(dllimport)
#endif 

//========== Error Codes Definition ==========
//****EH stands for "Exception Handler" ****//
#define     PLUM_OK       0



//========== Function Prototype for Exported API ==========
LIB_PLUM_API DWORD SetupPlum(bool bFullDump, bool bWithInvalidParamHandler, bool bSwallowException);
LIB_PLUM_API void TeardownPlum();

#endif          _LIB_PLUM_