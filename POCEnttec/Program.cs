using POCEnttec.OpenDmx;
using POCEnttec.OpenDmx.FTD2XX;
using POCEnttec.Utils;
using System;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace POCEnttec
{
    public static class Program
    {
        //public static void Main(string[] args)
        //{
        //    Test_Open();
        //}

        public static async Task Main(string[] args)
        {
            //IDemo demo = new SerialPortProgram();
            IDemo demo = new FTD2XXProgram();

            const int totalDuration = 20_000;

            CancellationTokenSource cts = new CancellationTokenSource(totalDuration);
            await demo.Demo(cts.Token);
        }
    }
}