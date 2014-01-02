using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestBobKinectWrapper
{

    public class CFunction
    {
        [DllImport("D:\\VCProjects\\BobKinectWrapper\\Debug\\BobKinectWrapper.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public extern static void StartKinect();

        [DllImport("D:\\VCProjects\\BobKinectWrapper\\Debug\\BobKinectWrapper.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public extern static void PollColorData(byte[] ppColorBuffer, int BufferSize);

        [DllImport("D:\\VCProjects\\BobKinectWrapper\\Debug\\BobKinectWrapper.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public extern static void StopKinect();
    }
    class Program
    {
        static void Main(string[] args)
        {
            byte[] colors = new byte[1228800];
            Console.ReadKey();
            CFunction.StartKinect();
            Thread.Sleep(2000);
            CFunction.PollColorData(colors, 1228800);
            CFunction.StopKinect();
            Console.WriteLine("stopped");
        }
    }
}
