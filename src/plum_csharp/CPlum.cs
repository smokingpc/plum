using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;

namespace Plum
{
    public class CPlum
    {
        //copy from dbghelp.h
        internal enum MINIDUMP_TYPE
        {
            MiniDumpNormal = 0x00000000,
            MiniDumpWithDataSegs = 0x00000001,
            MiniDumpWithFullMemory = 0x00000002,
            MiniDumpWithHandleData = 0x00000004,
            MiniDumpFilterMemory = 0x00000008,
            MiniDumpScanMemory = 0x00000010,
            MiniDumpWithUnloadedModules = 0x00000020,
            MiniDumpWithIndirectlyReferencedMemory = 0x00000040,
            MiniDumpFilterModulePaths = 0x00000080,
            MiniDumpWithProcessThreadData = 0x00000100,
            MiniDumpWithPrivateReadWriteMemory = 0x00000200,
            MiniDumpWithoutOptionalData = 0x00000400,
            MiniDumpWithFullMemoryInfo = 0x00000800,
            MiniDumpWithThreadInfo = 0x00001000,
            MiniDumpWithCodeSegs = 0x00002000,
            MiniDumpWithoutAuxiliaryState = 0x00004000,
            MiniDumpWithFullAuxiliaryState = 0x00008000,
            MiniDumpWithPrivateWriteCopyMemory = 0x00010000,
            MiniDumpIgnoreInaccessibleMemory = 0x00020000,
            MiniDumpWithTokenInformation = 0x00040000,
            MiniDumpWithModuleHeaders = 0x00080000,
            MiniDumpFilterTriage = 0x00100000,
            MiniDumpValidTypeFlags = 0x001fffff,
        }

        readonly string BinPath = AppDomain.CurrentDomain.BaseDirectory;
        bool DumpFullMemory = false;
        bool TerminateProcess = false;

        //Note: If Application.ThreadException is set in WinForm app ,
        //      CPlum will NOT get the UnhandledException event.
        //      refer to https://msdn.microsoft.com/zh-tw/library/system.windows.forms.application.threadexception(v=vs.110).aspx
        public CPlum(bool terminate, bool fulldump)
        { 
            // if true==terminate, this class will terminate current process 
            //after dump complete.
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
            TerminateProcess = terminate;
            DumpFullMemory = fulldump;
        }
        private string GenerateDumpFilename()
        {
            string filename = "";

            filename = string.Format("{0}.dmp", 
                            DateTime.Now.ToString("yyyyMMdd_HHmmss"));

            return filename;
        }

        private void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            string file = GenerateDumpFilename();
            WriteMiniDumpToFile(file);

            if (TerminateProcess)
                Process.GetCurrentProcess().Kill();
        }

        [DllImport("dbghelp.dll")]
        static extern bool MiniDumpWriteDump(
            IntPtr hProcess,
            Int32 ProcessId,
            IntPtr hFile,
            MINIDUMP_TYPE DumpType,
            IntPtr ExceptionParam,
            IntPtr UserStreamParam,
            IntPtr CallackParam);
        public void WriteMiniDumpToFile(String filename)
        { 
            //todo: check file exist
            string filepath = string.Format("{0}{1}", BinPath, filename);

            FileStream fs = File.Create(filepath);
            Process proc = Process.GetCurrentProcess();

            MINIDUMP_TYPE type = MINIDUMP_TYPE.MiniDumpNormal;
            if (DumpFullMemory)
                type = MINIDUMP_TYPE.MiniDumpWithFullMemory;

            MiniDumpWriteDump(proc.Handle, proc.Id,
                fs.SafeFileHandle.DangerousGetHandle(), type, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            fs.Close();
        }
    }
}
