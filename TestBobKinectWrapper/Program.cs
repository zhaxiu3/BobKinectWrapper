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
        
        [DllImport("D:\\VCProjects\\BobKinectWrapper\\Debug\\BobKinectWrapper.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public extern static void PollSkeletonData(float[] SkeletonBuffer, int BufferSize);
    }
    class Program
    {
        static bool isRunning = true;
        static void Run() {
            float[] skeletonBuffer = new float[480];
            CFunction.StartKinect();
            while (isRunning) {
                CFunction.PollSkeletonData( skeletonBuffer,480);
                string arraystr = string.Empty;
                for (int i = 0; i < 480; i++) {
                    arraystr += skeletonBuffer[i] + " ";
                }
                    Console.WriteLine(arraystr);
                Thread.Sleep(20);
            }
            CFunction.StopKinect();
        }
        static void Main(string[] args)
        {
            Thread testThread = new Thread(Run);
            testThread.Start();
            //Console.ReadKey();
            Thread.Sleep(100000);
            isRunning = false;
            testThread.Join();
        }
    }
}
