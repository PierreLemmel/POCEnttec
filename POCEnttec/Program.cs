using System;
using System.IO.Ports;
using System.Linq;

namespace POCEnttec
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Available ports:");
            
            string[] portNames = SerialPort.GetPortNames();
            if (!portNames.Any())
                throw new InvalidOperationException("No COM port found");
            else if (portNames.Length > 1)
                throw new InvalidOperationException("Multiple COM ports found");

            string portName = portNames.Single();
            int baudRate = 57600;

            using SerialPort serialPort = new SerialPort(portName, baudRate);

            Console.WriteLine($"Opening Serial Port: {portName}");
            serialPort.Open();
            Console.WriteLine($"Serial Port opened: {portName}");

            Console.WriteLine($"Closing Serial Port: {portName}");
            serialPort.Close();
            Console.WriteLine($"Serial Port closed: {portName}");
        }
    }
}