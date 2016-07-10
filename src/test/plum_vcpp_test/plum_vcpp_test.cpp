// plum_vcpp_test.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
//#include "plum.h"

int _tmain(int argc, _TCHAR* argv[])
{
    unsigned int *err_data = 0;

    LoadLibrary(_T("plum.dll"));

    err_data[1] = 13;

	return 0;
}

