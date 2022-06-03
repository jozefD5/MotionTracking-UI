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

        //List of available/detected serial ports
        private List<String> _portList = new List<String>();

        //Range of available ports
        public int portListCount { get; set; }





        //Initilise serial driver, needs to be called before any serial 
        //functionalities are used
        public void SerialInit(SerialStmConf settings)
        {
            SerialApplySettings(settings);

            //Add data reacived handler
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        }


        //Update settings
        public void SerialUpdateSettings(SerialStmConf settings)
        {
            SerialClose();
            SerialApplySettings(settings);
            SerialOpen();
        }


        //Apply new settings
        private void SerialApplySettings(SerialStmConf settings)
        {
            //Check if all parameters are set, if not set default
            //Check serial port name
            if (string.IsNullOrEmpty(settings.PortName))
            {
                _serialPort.PortName = "COM1";
            }
            else
            {
                _serialPort.PortName = settings.PortName;
            }

            //Check baud rate
            if (settings.BaudRate <= 9600)
            {
                _serialPort.BaudRate = 9600;
            }
            else
            {
                _serialPort.BaudRate = settings.BaudRate;
            }

            //Check data bits
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




        //Locate all available serial ports
        public void DetectSerialDevices()
        {
            //Dtect serial ports and store them in list
            foreach (string s in SerialPort.GetPortNames())
            {
                _portList.Add(s);
            }

        }


        //Output list with all available serial ports
        public List<String> GetSerialList()
        {
            DetectSerialDevices();

            return _portList;
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
