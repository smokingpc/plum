
#include "stdafx.h"
#include <tchar.h>
#include <dbghelp.h>
#include <stdlib.h>
#include <stdio.h>
#include "plum.h"

//supress warning C4996. Don't use the secure-crt API.
//If use secure-crt API and string functions has invalid_param exception,
//it could cause recursive call to PlumInvalidParamHandler().
#pragma warning(disable : 4996)


//This exception code should be defined to STATUS_INVALID_PARAMETER. 
#define     INVALID_PARAM_CODE      0xC000000D      

//==============================================================
static LPTOP_LEVEL_EXCEPTION_FILTER     g_pfnOldHandler = NULL;
static MINIDUMP_TYPE                    g_DumpType = MiniDumpWithHandleData;
static TCHAR                            *g_szOutPath = NULL;
static bool                             g_bSwallowException = false;
static _invalid_parameter_handler       g_pfnOldInvalidParamHandler = NULL;

//==============================================================
LONG WINAPI PlumHandler(struct _EXCEPTION_POINTERS* pException);
void PlumInvalidParamHandler(const wchar_t* szExpression, const wchar_t* szFunction, const wchar_t* szSrcFile, unsigned int nLineNo, uintptr_t pReserved);
TCHAR *GetCurrentExePath();
TCHAR *GenerateDumpFullpath(TCHAR *szOutDir);

DWORD SetupPlum(bool bFullDump, bool bWithInvalidParamHandler, bool bSwallowException)
{
    g_szOutPath = GetCurrentExePath();
    g_bSwallowException = bSwallowException;
    if(bFullDump)
        g_DumpType = MiniDumpWithFullMemory;

    g_pfnOldHandler = SetUnhandledExceptionFilter( PlumHandler );

    if(bWithInvalidParamHandler)
        g_pfnOldInvalidParamHandler = _set_invalid_parameter_handler( PlumInvalidParamHandler );

    return PLUM_OK;
}

void TeardownPlum()
{
    if(NULL != g_pfnOldHandler)
        SetUnhandledExceptionFilter( g_pfnOldHandler );
    if(NULL != g_pfnOldInvalidParamHandler)
        _set_invalid_parameter_handler( g_pfnOldInvalidParamHandler );

    if(NULL != g_szOutPath)
    {
        delete[] g_szOutPath;
        g_szOutPath = NULL;
    }
}

LONG WINAPI PlumHandler(struct _EXCEPTION_POINTERS* pException)
{
    MINIDUMP_EXCEPTION_INFORMATION DumpInfo = {0};
    HANDLE hDumpFile = INVALID_HANDLE_VALUE;
    TCHAR *szOutFile = GenerateDumpFullpath(g_szOutPath);

    if(NULL != szOutFile)
    {
        ZeroMemory(&DumpInfo, sizeof(MINIDUMP_EXCEPTION_INFORMATION));
        DumpInfo.ThreadId = GetCurrentThreadId();
        DumpInfo.ExceptionPointers = pException;
        DumpInfo.ClientPointers = FALSE;

        hDumpFile = CreateFile(szOutFile, GENERIC_WRITE, 0, NULL, CREATE_ALWAYS, 0, NULL);
        if(INVALID_HANDLE_VALUE != hDumpFile)
        {
            MiniDumpWriteDump(  GetCurrentProcess(), GetCurrentProcessId(), hDumpFile, g_DumpType, &DumpInfo, NULL, NULL);
            CloseHandle(hDumpFile);
        }

        delete[] szOutFile;
        szOutFile = NULL;
    }

    //use EXCEPTION_CONTINUE_SEARCH can pass this exception to next handler. 
    //It make PostMortem debuggers can get this exception.
    LONG nRet = EXCEPTION_CONTINUE_SEARCH;

    // If g_bSwallowException is set to TRUE in SetupPlum(), consume this exception.
    // It make process still alive and running.
    if(g_bSwallowException)
        nRet = EXCEPTION_EXECUTE_HANDLER;
    return nRet;
}

void PlumInvalidParamHandler(const wchar_t* szExpression, const wchar_t* szFunction, const wchar_t* szSrcFile, unsigned int nLineNo, uintptr_t pReserved)
{
    //Not passing the arguments into raised exception.
    //Because in windbg, invalid param occuring frame will be shown in callstack.
    RaiseException(INVALID_PARAM_CODE, EXCEPTION_NONCONTINUABLE, 0, NULL);
}


//==============================================================
TCHAR *GenerateDumpFullpath(TCHAR *szOutDir)
{
    if(NULL == szOutDir)
        return NULL;

    //todo: check if the file exist?

    TCHAR *szOutFile = new TCHAR[MAX_PATH];
    ZeroMemory(szOutFile, MAX_PATH);
    SYSTEMTIME systime = {0};
    GetLocalTime(&systime);

    //FileName = YYYYMMDD-HHMMSS.dmp
    _stprintf(  szOutFile, 
                _T("%s\\%04d%02d%02d-%02d%02d%02d.dmp"), 
                szOutDir, 
                systime.wYear, 
                systime.wMonth, 
                systime.wDay, 
                systime.wHour, 
                systime.wMinute, 
                systime.wSecond);

    return szOutFile;
}

TCHAR *GetCurrentExePath()
{
    DWORD nSizeInChar = MAX_PATH*2;
    TCHAR *szPath = new TCHAR[nSizeInChar];
    ZeroMemory(szPath, nSizeInChar);
    if(0 != GetModuleFileName(NULL, szPath, nSizeInChar))
    {
        TCHAR *token = _tcsrchr(szPath, '\\');
        if(NULL != token)
            token[0] = '\0';
    }
    else
    {
        delete[] szPath;
        szPath = NULL;
    }

    return szPath;
}

