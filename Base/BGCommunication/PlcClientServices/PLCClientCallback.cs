using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BGCommunication
{
    public class PLCClientCallback
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int RecvCallback(int ProtocolIndex,IntPtr buffer,int bufferLenght);
    }
}
