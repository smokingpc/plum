Public Enum DUMP_TYPE
    MiniDumpNormal = 0
    MiniDumpWithDataSegs = 1
    MiniDumpWithFullMemory = 2
    MiniDumpWithHandleData = 4
    MiniDumpFilterMemory = 8
    MiniDumpScanMemory = 10
    MiniDumpWithUnloadedModules = 20
    MiniDumpWithIndirectlyReferencedMemory = 40
    MiniDumpFilterModulePaths = 80
    MiniDumpWithProcessThreadData = 100
    MiniDumpWithPrivateReadWriteMemory = 200
    MiniDumpWithoutOptionalData = 400
    MiniDumpWithFullMemoryInfo = 800
    MiniDumpWithThreadInfo = 1000
    MiniDumpWithCodeSegs = 2000
End Enum


Public Class PlumVB
    <Runtime.InteropServices.DllImport("dbghelp.dll")> _
    Private Shared Function MiniDumpWriteDump( _
            ByVal hProcess As IntPtr, _
            ByVal ProcessId As Int32, _
            ByVal hFile As IntPtr, _
            ByVal DumpType As DUMP_TYPE, _
            ByVal ExceptionParam As IntPtr, _
            ByVal UserStreamParam As IntPtr, _
            ByVal CallbackParam As IntPtr) As Boolean
    End Function

    Public Shared Sub WriteDumpFile(ByVal filename As String)
        Dim fs As IO.FileStream = Nothing
        If (IO.File.Exists(filename)) Then
            fs = IO.File.Open(filename, IO.FileMode.Append)
        Else
            fs = IO.File.Create(filename)
        End If
        Dim proc As Process = Process.GetCurrentProcess()
        MiniDumpWriteDump(proc.Handle, proc.Id, _
                          fs.SafeFileHandle.DangerousGetHandle(), _
                          DUMP_TYPE.MiniDumpWithFullMemory, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero)
        fs.Close()
    End Sub
End Class
