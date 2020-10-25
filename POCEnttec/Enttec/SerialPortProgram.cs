using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace POCEnttec
{
    public class SerialPortProgram : IDemo
    {
        private const int PacketSize = 512;
        private const int DataOffset = 4;

        private const int FlatJardinOffset = 1;
        private const int FlatCourOffset = 9;
        private const int ParLEDJardinOffset = 17;
        private const int ParLEDCourOffset = 25;

        private const int RefreshFrequency = 20;

        private byte[] data;
        private byte[] resetData;
        private SerialPort serialPort;

        private object mutex = new object();

        public async Task Demo(CancellationToken token)
        {
            string[] portNames = SerialPort.GetPortNames();
            if (!portNames.Any())
                throw new InvalidOperationException("No COM port found");
            else if (portNames.Length > 1)
                throw new InvalidOperationException("Multiple COM ports found");

            string portName = portNames.Single();
            int baudRate = 57600;

            try
            {
                serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);

                serialPort.DataReceived += OnDataReceived;
                serialPort.PinChanged += OnPinChanged;
                serialPort.ErrorReceived += OnErrorReceived;

                Console.WriteLine($"Opening Serial Port: {portName}");
                serialPort.Open();
                Console.WriteLine($"Serial Port opened: {portName}");
                Console.WriteLine();

                InitDmx();
                InitData();

                CreateRefreshLoopback(token);

                await ChaserDemo(token);

                Console.WriteLine($"Closing Serial Port: {portName}");
                serialPort.Close();
                Console.WriteLine($"Serial Port closed: {portName}");
            }
            finally
            {
                serialPort?.Dispose();
            }
        }

        private async Task ChaserDemo(CancellationToken token)
        {
            const int delay = 1000;

            int step = 0;
            while (!token.IsCancellationRequested)
            {
                lock (mutex)
                {
                    ResetData();

                    switch (step % 4)
                    {
                        case 0:
                            data[DataOffset + ParLEDJardinOffset + 0] = 0xff;
                            data[DataOffset + ParLEDJardinOffset + 1] = 0xff;
                            data[DataOffset + ParLEDJardinOffset + 2] = 0xff;
                            data[DataOffset + ParLEDJardinOffset + 4] = 0xff;

                            Console.WriteLine("PAR LED Jardin");
                            break;

                        case 1:
                            data[DataOffset + FlatJardinOffset + 0] = 0xff;
                            data[DataOffset + FlatJardinOffset + 1] = 0xff;
                            data[DataOffset + FlatJardinOffset + 2] = 0xff;

                            Console.WriteLine("Flat Jardin");
                            break;

                        case 2:
                            data[DataOffset + FlatCourOffset + 0] = 0xff;
                            data[DataOffset + FlatCourOffset + 1] = 0xff;
                            data[DataOffset + FlatCourOffset + 2] = 0xff;

                            Console.WriteLine("Flat Cour");
                            break;

                        case 3:
                            data[DataOffset + ParLEDCourOffset + 0] = 0xff;
                            data[DataOffset + ParLEDCourOffset + 1] = 0xff;
                            data[DataOffset + ParLEDCourOffset + 2] = 0xff;
                            data[DataOffset + ParLEDCourOffset + 4] = 0xff;

                            Console.WriteLine("PAR LED Cour");
                            break;
                    } 
                }

                await Task.Delay(delay);
                step++;
            }
        }

        private void CreateRefreshLoopback(CancellationToken token)
        {
            Task task = Task.Delay(RefreshFrequency, token)
                .ContinueWith(
                    _ =>
                    {
                        SendFrame();
                        Console.WriteLine("Frame sent");
                        CreateRefreshLoopback(token);

                    }, TaskContinuationOptions.NotOnCanceled
                )
                .ContinueWith(
                    _ =>
                    {
                        Console.WriteLine("RefreshTask Cancelled");
                    },
                    TaskContinuationOptions.OnlyOnCanceled
                );
        }

        private void InitData()
        {
            data = new byte[PacketSize + 6];
            resetData = new byte[PacketSize];

            data[0] = Enttec.StartOfMessage;
            data[1] = Enttec.SendDMXPacket;
            data[2] = (PacketSize + 1).LSB();
            data[3] = (PacketSize + 1).MSB();
            data[4] = Enttec.StartCode;

            data[data.Length - 1] = Enttec.EndOfMessage;
        }

        private void ResetData()
        {
            lock (mutex)
            {
                Array.Copy(resetData, 0, data, DataOffset, PacketSize); 
            }
        }

        private void InitDmx()
        {
            Console.WriteLine("Initializing DMX");

            byte[] initMsg1 = new byte[]
            {
                Enttec.StartOfMessage,
                0x03,
                0x02,
                0x00,
                0x00,
                0x00,
                Enttec.EndOfMessage
            };

            PrintFrame(initMsg1);
            serialPort.Write(initMsg1, 0, initMsg1.Length);

            byte[] initMsg2 = new byte[]
            {
                Enttec.StartOfMessage,
                0x10,
                0x02,
                0x00,
                0x00,
                0x00,
                Enttec.EndOfMessage
            };

            PrintFrame(initMsg2);
            serialPort.Write(initMsg2, 0, initMsg2.Length);

            Console.WriteLine("DMX Initialized");
            Console.WriteLine();
        }

        private void PrintFrame(byte[] frame)
        {
            for (int i = 0; i < frame.Length; i++)
                Console.WriteLine($"[{i}]: 0x{frame[i]:X2}");
            Console.WriteLine();
        }

        private void SendFrame()
        {
            lock (mutex)
            {
                serialPort.Write(data, 0, data.Length); 
            }
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine($"OnDataReceived: {e.EventType}");
            Console.WriteLine();
            Console.ResetColor();
        }

        private void OnPinChanged(object sender, SerialPinChangedEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine($"OnPinReceived: {e.EventType}");
            Console.WriteLine();
            Console.ResetColor();
        }

        private void OnErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine();
            Console.WriteLine($"OnErrorReceived: {e.EventType}");
            Console.WriteLine();
            Console.ResetColor();
        }
    }
}