using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Motion_Tracking_UI.SerialCom;
using System.IO.Ports;
using System.Diagnostics;

namespace Motion_Tracking_UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //STM32 Serial driver object and settings
        SerialStmConf serialConf = new();
        SerialStm serialDriver = new();


        public MainWindow()
        {
            InitializeComponent();

            ConfigureSerialComm();
        }


        


        //Click button event, open serial port
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                serialDriver.SerialOpen();
            }
            catch
            {
                PrintToTerminal("Error: Oppening serial port");
            }
        }



        //Refresh serial port lis button
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateComboBox();
        }


        //Update Com port for serial driver when new item is selected from combobox
        private void cbxPortSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            serialConf.PortName = (String)cbxPortSelector.SelectedItem;
            serialDriver.SerialUpdateSettings(serialConf);
        }





        //Print string to terminal windoe in application
        public void PrintToTerminal(String str)
        {
            str += "\n";
            SerialTextBlock.Text += str;
            SerialScroller.ScrollToEnd();
        }


        //Configure initial serial settings
        public void ConfigureSerialComm()
        {
            //Serial port settings
            serialConf.PortName = "COM0";
            serialConf.BaudRate = 115200;
            serialConf.DataBits = 8;
            serialConf.ParityBits = Parity.None;
            serialConf.StopBits = StopBits.One;


            //Initiate serial driver
            serialDriver.SerialInit(serialConf);

            //Add data reacived handler
            serialDriver.serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialDataReceivedHandler);


            //Get all available serial ports and update combobox
            UpdateComboBox();
        }


        //Refresh port list and update combobox
        public void UpdateComboBox()
        {
            //Clear combobox
            cbxPortSelector.ItemsSource = null;
            cbxPortSelector.Items.Refresh();
            //cbxPortSelector.DataContext = null;
            /*
            //Detect com ports
            serialDriver.DetectSerialDevices();

            Trace.WriteLine($"\n\nAvailable Serial Ports: {serialDriver.portListCount}");

            for (int i = 0; i < serialDriver.portListCount; i++)
            {
                string str = serialDriver.GetSerialPort(i);
                Trace.WriteLine($"Serial port: {str}");

                cbxPortSelector.Items.Add(str);
            }
            */
            cbxPortSelector.ItemsSource = serialDriver.GetSerialList();


            cbxPortSelector.Items.Refresh();
        }


        //Data Reacive hanndler, handles serial input data
        private void SerialDataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadLine();
            //Console.WriteLine(indata);

            this.Dispatcher.Invoke(() =>
            {
                PrintToTerminal(indata);
            });

            
        }



    }
}
