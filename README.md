============ PLUM ============

Win32 crash dump generator lib for C# / VB / VC++.
Author : Roy Wang, 2016
Licensed by GPLv3

[How to use]
1. VC++
    Build plum_vcpp and link output dll by LoadLibrary() or static link.
    You don't have to write any additional codes.
    When unhandled exception occurred, this dll will generate dump file 
    automatically in your EXE's folder.

2. CSharp
    Allocate a object by CPlum(), in Main() of your Program.cs. Like this:
    
        CPlum handler = new CPlum(true, true);
    
    The dump will be also generated automatically in your EXE's folder.

3. VB
    To use plum library in VB, you have to call PlumVB.WriteDumpFile()
    in catch statement. 


