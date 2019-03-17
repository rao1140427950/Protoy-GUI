using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace Protoy
{
    class Serial
    {
        private string DefaultPortName = "COM4";
        private int DefaultBaudRate = 115200;
        private Parity DefaultParity = Parity.Even;

        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public Parity Parity { get; set; }

        private static SerialPort serialPort = new SerialPort();


        public Serial()  // Construct
        {
            //serialPort = new SerialPort();
            serialPort.ReadTimeout = 1000;
            serialPort.WriteTimeout = 500;
        }

        ~Serial()
        {
            serialPort.Close();
            serialPort.Dispose();
        }

        public string[] GetAvaiablePorts()
        {
            return SerialPort.GetPortNames();
        }

        public void Close()
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
                Console.WriteLine(DateTime.Now.ToString() + " : Close port <" + PortName + ">");
            }
        }

        public bool Open()  // Open in default
        {
            if (serialPort.IsOpen)
            {
                Console.WriteLine(DateTime.Now.ToString() + " : Port <" + PortName + "> already opened");
                return false;
            }

            PortName = DefaultPortName;
            BaudRate = DefaultBaudRate;
            Parity = DefaultParity;

            serialPort.PortName = PortName;
            serialPort.BaudRate = BaudRate;
            serialPort.Parity = Parity;
            serialPort.Open();

            if (serialPort.IsOpen)
            {
                Console.WriteLine(DateTime.Now.ToString() + " : Open port <" + PortName + ">");
                return true;
            }
            else
            {
                Console.WriteLine(DateTime.Now.ToString() + " : Failed to open port <" + PortName + ">");
                return false;
            }
            //return serialPort.IsOpen;
        }

        public bool Open(string portName, int baudRate, Parity parity)
        {
            PortName = portName;
            BaudRate = baudRate;
            Parity = parity;

            serialPort.PortName = PortName;
            serialPort.BaudRate = BaudRate;
            serialPort.Parity = Parity;
            serialPort.Open();

            if (serialPort.IsOpen)
            {
                Console.WriteLine(DateTime.Now.ToString() + " : Open port <" + PortName + ">");
                return true;
            }
            else
            {
                Console.WriteLine(DateTime.Now.ToString() + " : Failed to open port <" + PortName + ">");
                return false;
            }
            //return serialPort.IsOpen;
        }

        public string ReadTest()
        {
            Byte[] bytes = new Byte[10];
            int n;

            try
            {
                n = serialPort.Read(bytes, 0, 10);
                foreach (byte s in bytes)
                {
                    Console.Write(s);
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
            catch (TimeoutException)
            {
                Console.WriteLine(DateTime.Now.ToString() + " : Failed to read port <" + PortName + ">");
                // TODO
            }
            

            return bytes.ToString();
        }


    }
}
