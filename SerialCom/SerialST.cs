using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Motion_Tracking_UI;
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
        public SerialPort serialPort = new();

        //List of available/detected serial ports
        private List<String> _portList = new List<String>();




        //Initilise serial driver, needs to be called before any serial functionalities are used
        public void SerialInit(SerialStmConf settings)
        {
            //Apply settings
            SerialApplySettings(settings);
        }


        //Update settings, should be used if serial settings are nneded to be alter in run time
        public void SerialUpdateSettings(SerialStmConf settings)
        {
            SerialClose();
            SerialApplySettings(settings);
        }


        //Apply new settings
        private void SerialApplySettings(SerialStmConf settings)
        {
            //Check if all parameters are set, if not set default
            //Check serial port name
            if (string.IsNullOrEmpty(settings.PortName))
            {
                serialPort.PortName = "COM1";
            }
            else
            {
                serialPort.PortName = settings.PortName;
            }

            //Check baud rate
            if (settings.BaudRate <= 9600)
            {
                serialPort.BaudRate = 9600;
            }
            else
            {
                serialPort.BaudRate = settings.BaudRate;
            }

            //Check data bits
            if (settings.DataBits < 8)
            {
                serialPort.DataBits = 8;
            }
            else
            {
                serialPort.DataBits = settings.DataBits;
            }


            serialPort.Parity = settings.ParityBits;
            serialPort.StopBits = settings.StopBits;
        }




        //Open serial port
        public void SerialOpen()
        {
            serialPort.Open();
        }


        //Close serial port
        public void SerialClose()
        {
            serialPort.Close();
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
