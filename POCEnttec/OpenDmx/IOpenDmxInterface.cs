﻿using System;
using System.Collections.Generic;
using System.Text;

namespace POCEnttec.OpenDmx
{
    public interface IOpenDmxInterface : IDisposable
    {
        void Start();
        void Stop();

        byte this[int index] { get; set; }

        void ClearFrame();
        void CopyData(int channelOffset, byte[] data, int length);
    }
}
