using System;

namespace POCEnttec.OpenDmx.FTD2XX
{
    using static FTD2XXDll;

    public unsafe class FTD2XXInterface : IOpenDmxInterface
    {
        private const int BufferSize = 513;
        private FTD2XXData data;

        private object mutex = new object();

        public byte this[int channel]
        {
            get => data.buffer[channel];
            set
            {
                lock (mutex)
                {
                    data.buffer[channel] = value;
                }
            }
        }

        public void Start()
        {
            FT_STATUS status = FT_Open(0, out data.handle);
            CheckStatus(status, nameof(FT_Open));

            this[0] = 0;  //Set DMX Start Code

            InitOpenDmx();
        }

        public void Stop() => FT_Close(data.handle);

        public void Dispose() => Stop();

        public void SendDmxFrame()
        {
            lock (mutex)
            {
                FT_STATUS status = FT_SetBreakOn(data.handle);
                CheckStatus(status, nameof(FT_SetBreakOn));

                status = FT_SetBreakOff(data.handle);
                CheckStatus(status, nameof(FT_SetBreakOff));

                fixed (byte* ptr = data.buffer)
                {
                    status = FT_Write(data.handle, ptr, BufferSize, out _);
                    CheckStatus(status, nameof(FT_Write));
                }
            }
        }

        private byte[] resetData = new byte[513];
        public void ClearFrame()
        {
            lock (mutex)
            {
                fixed (byte* src = resetData)
                fixed (byte* dest = data.buffer)
                {

                    Buffer.MemoryCopy(src, dest, BufferSize, BufferSize);
                }
            }
        }

        public void CopyData(int channelOffset, byte[] input, int length)
        {
            if (length > input.Length) throw new InvalidOperationException($"Input length must be smaller than length");
            if (channelOffset + length > BufferSize) throw new InvalidOperationException($"'channelOffset + length' mush be smaller than BufferSize (513)");

            lock (mutex)
            {
                fixed (byte* src = resetData)
                fixed (byte* bufferAddr = data.buffer)
                {
                    byte* dest = bufferAddr + channelOffset;
                    Buffer.MemoryCopy(src, dest, length, length);
                }
            }
        }

        private void InitOpenDmx()
        {
            FT_STATUS status = FT_ResetDevice(data.handle);
            CheckStatus(status, nameof(FT_ResetDevice));

            status = FT_SetDivisor(data.handle, (char)12);
            CheckStatus(status, nameof(FT_SetDivisor));

            status = FT_SetDataCharacteristics(data.handle, BITS_8, STOP_BITS_2, PARITY_NONE);
            CheckStatus(status, nameof(FT_SetDataCharacteristics));

            status = FT_SetFlowControl(data.handle, FLOW_NONE, 0, 0);
            CheckStatus(status, nameof(FT_SetFlowControl));

            status = FT_ClrRts(data.handle);
            CheckStatus(status, nameof(FT_ClrRts));

            status = FT_Purge(data.handle, PURGE_TX);
            CheckStatus(status, nameof(FT_Purge));

            status = FT_Purge(data.handle, PURGE_RX);
            CheckStatus(status, nameof(FT_Purge));

        }

        private void CheckStatus(FT_STATUS status, string function)
        {
            if (status != FT_STATUS.FT_OK)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(function);
                Console.WriteLine($"Invalid status: {status}");
                Console.WriteLine();
                Console.ResetColor();
            }
        }

        private unsafe struct FTD2XXData
        {
            public IntPtr handle;
            public fixed byte buffer[BufferSize];
        }
    }
}