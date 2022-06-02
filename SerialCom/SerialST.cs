using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Diagnostics;

namespace Motion_Tracking_UI.SerialCom
{
    //Structure for configurating serial settings
    public struct SerialStmConf
    {
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public int DataBits { get; set; }
        public Parity ParityBits { get; set; }
        public StopBits StopBits { get; set; }
    }

    public class SerialStm
    {
        //Local serial IO port instance
        private SerialPort _serialPort = new();



        //Constructor
        public SerialStm(SerialStmConf settings)
        {
            if (string.IsNullOrEmpty(settings.PortName))
            {
                _serialPort.PortName = "COM1";
            }
            else
            {
                _serialPort.PortName = settings.PortName;
            }

            if (settings.BaudRate <= 9600)
            {
                _serialPort.BaudRate = 9600;
            }
            else
            {
                _serialPort.BaudRate = settings.BaudRate;
            }

            if (settings.DataBits < 8)
            {
                _serialPort.DataBits = 8;
            }
            else
            {
                _serialPort.DataBits = settings.DataBits;
            }

            _serialPort.Parity = settings.ParityBits;
            _serialPort.StopBits = settings.StopBits;

            _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

        }


        //Open serial port
        public void SerialOpen()
        {
            _serialPort.Open();
        }

        //Close serial port
        public void SerialClose()
        {
            _serialPort.Close();
        }



        //Data Reacive hanndler, handles serial input data
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            Console.WriteLine(indata);
        }







        //Print all available serial ports
        public static void DisplayAvailablePorts()
        {
            int serCommPorts = SerialPort.GetPortNames().Length;
            Trace.WriteLine($"Available Serial Ports: {serCommPorts}");

            foreach (string s in SerialPort.GetPortNames())
            {
                Trace.WriteLine($"SerialCom: {s}");
            }
            Trace.WriteLine(" ");
        }


    }
}
