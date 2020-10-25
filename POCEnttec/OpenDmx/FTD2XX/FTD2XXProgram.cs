using POCEnttec.OpenDmx.FTD2XX;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace POCEnttec.OpenDmx
{
    public class FTD2XXProgram : IDemo
    {
        private const int RefreshFrequency = 20;

        private const int FlatJardinOffset = 1;
        private const int FlatCourOffset = 9;
        private const int ParLEDJardinOffset = 17;
        private const int ParLEDCourOffset = 25;

        public async Task Demo(CancellationToken token)
        {
            using FTD2XXInterface ftd2xxInterface = new FTD2XXInterface();

            Console.WriteLine("Starting FTD2XX Interface");
            ftd2xxInterface.Start();
            Console.WriteLine("FTD2XX Interface Started");

            CreateRefreshLoopback(token, ftd2xxInterface);

            await ChaserDemo(token, ftd2xxInterface);

            Console.WriteLine("Stopping FTD2XX Interface");
            ftd2xxInterface.Stop();
            Console.WriteLine("OpenDmx FTD2XX Interface");
        }

        private async Task FlickerDemo(CancellationToken token, FTD2XXInterface openDmx)
        {
            const int delay = 1000;

            int step = 0;

            while (!token.IsCancellationRequested)
            {
                byte dmxVal = step % 2 == 0 ? byte.MaxValue : byte.MinValue;

                openDmx[ParLEDJardinOffset + 0] = dmxVal;
                openDmx[ParLEDJardinOffset + 1] = dmxVal;
                openDmx[ParLEDJardinOffset + 2] = dmxVal;
                openDmx[ParLEDJardinOffset + 4] = dmxVal;

                openDmx[FlatJardinOffset + 0] = dmxVal;
                openDmx[FlatJardinOffset + 1] = dmxVal;
                openDmx[FlatJardinOffset + 2] = dmxVal;

                openDmx[FlatCourOffset + 0] = dmxVal;
                openDmx[FlatCourOffset + 1] = dmxVal;
                openDmx[FlatCourOffset + 2] = dmxVal;

                openDmx[ParLEDCourOffset + 0] = dmxVal;
                openDmx[ParLEDCourOffset + 1] = dmxVal;
                openDmx[ParLEDCourOffset + 2] = dmxVal;
                openDmx[ParLEDCourOffset + 4] = dmxVal;

                await Task.Delay(delay);
                step++;
            }
        }

        private async Task ChaserDemo(CancellationToken token, FTD2XXInterface openDmx)
        {
            const int delay = 1000;

            int step = 0;
            while (!token.IsCancellationRequested)
            {
                openDmx.ClearFrame();

                switch (step % 4)
                {
                    case 0:
                        openDmx[ParLEDJardinOffset + 0] = 0xff;
                        openDmx[ParLEDJardinOffset + 1] = 0xff;
                        openDmx[ParLEDJardinOffset + 2] = 0xff;
                        openDmx[ParLEDJardinOffset + 4] = 0xff;

                        Console.WriteLine("PAR LED Jardin");
                        break;

                    case 1:
                        openDmx[FlatJardinOffset + 0] = 0xff;
                        openDmx[FlatJardinOffset + 1] = 0xff;
                        openDmx[FlatJardinOffset + 2] = 0xff;

                        Console.WriteLine("Flat Jardin");
                        break;

                    case 2:
                        openDmx[FlatCourOffset + 0] = 0xff;
                        openDmx[FlatCourOffset + 1] = 0xff;
                        openDmx[FlatCourOffset + 2] = 0xff;

                        Console.WriteLine("Flat Cour");
                        break;

                    case 3:
                        openDmx[ParLEDCourOffset + 0] = 0xff;
                        openDmx[ParLEDCourOffset + 1] = 0xff;
                        openDmx[ParLEDCourOffset + 2] = 0xff;
                        openDmx[ParLEDCourOffset + 4] = 0xff;

                        Console.WriteLine("PAR LED Cour");
                        break;
                }

                await Task.Delay(delay);
                step++;
            }
        }

        private void CreateRefreshLoopback(CancellationToken token, FTD2XXInterface openDmx)
        {
            Task task = Task.Delay(RefreshFrequency, token)
                .ContinueWith(
                    _ =>
                    {
                        openDmx.SendDmxFrame();
                        CreateRefreshLoopback(token, openDmx);

                    }, TaskContinuationOptions.NotOnCanceled
                )
                .ContinueWith(
                    _ =>
                    {
                        Console.WriteLine("RefreshTask Cancelled");
                        openDmx.ClearFrame();
                        openDmx.SendDmxFrame();
                        Console.WriteLine("DMX Frame Cleared");
                    },
                    TaskContinuationOptions.OnlyOnCanceled
                );
        }
    }
}