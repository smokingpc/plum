using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using Plum;

namespace plum_csharp_test2
{
    class Program
    {
        static void Main(string[] args)
        {
            CPlum handler = new CPlum(true, true);
            Thread work = new Thread(WorkThread);
            work.Start();

            work.Join();
        }

        private static void WorkThread()
        {
            throw new Exception("Thread exception");
        }
    }
}
