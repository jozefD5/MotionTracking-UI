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
using System.Windows.Threading;

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
        bool serialMtState = false;


        public MainWindow()
        {
            InitializeComponent();

            ConfigureSerialComm();


            //Update serial connection buttons state
            UpdateSerialButtons();


            //Add exceptions handler
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }


        


        //Click button event, open serial port
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (serialDriver.SerialOpen())
                {
                    StatusTextBlock.Text = "Connected";
                    StatusTextBlock.Foreground = new SolidColorBrush(Colors.Green);
                    UpdateSerialButtons();


                }
                else
                {
                    StatusTextBlock.Text = "Disconnected";
                    StatusTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                    UpdateSerialButtons();
                }

            }
            catch
            {
                PrintToTerminal("Error: Oppening serial port");
                StatusTextBlock.Text = "Unable to connect";
                StatusTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                UpdateSerialButtons();
            }
        }


        //Refresh serial port lis button
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateComboBox();
        }


        //Clear terminal
        private void ClearSerialButton_Click(object sender, RoutedEventArgs e)
        {
            SerialTextBlock.Text = "";
        }


        //Update Com port for serial driver when new item is selected from combobox
        private void cbxPortSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            serialConf.PortName = (String)cbxPortSelector.SelectedItem;
            serialDriver.SerialUpdateSettings(serialConf);
        }


        //Activate MT by sending star command via serial
        private void StartMtButton_Click(object sender, RoutedEventArgs e)
        {
            


            if (!serialMtState)
            {
                PrintToTerminal($"MtCommand: {MTCommands.mt_start}");
                serialDriver.SerialWriteLine(MTCommands.mt_start);

                StartMtButton.Content = "Stop Mt";
                serialMtState = true;
            }
            else
            {
                PrintToTerminal($"MtCommand: {MTCommands.mt_stop}");
                serialDriver.SerialWriteLine(MTCommands.mt_stop);

                StartMtButton.Content = "Start Mt";
                serialMtState = false;

            }

        }


        //Disconnect serial port
        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            serialDriver.SerialClose(); ;

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


        public void UpdateSerialButtons()
        {
            if (serialDriver.IsSerialOpen())
            {
                //ConnectButton.IsEnabled = false;
                //DisconnectButton.IsEnabled = true;
                StartMtButton.IsEnabled = true;
            }
            else
            {
                //ConnectButton.IsEnabled = true;
                //DisconnectButton.IsEnabled = false;
                StartMtButton.IsEnabled = false;
            }

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
            string indata = "";
            SerialPort sp = (SerialPort)sender;
            

            try
            {
                indata = sp.ReadLine();
            }
            catch
            {
                this.Dispatcher.Invoke(() =>
                {
                    StatusTextBlock.Text = "Hardware was disconnected";
                    StatusTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                    UpdateSerialButtons();
                });
            }

            this.Dispatcher.Invoke(() =>
            {
                
                PrintToTerminal(indata);
            });

            
        }



        //Error handler
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            StatusTextBlock.Text = "Error";
            StatusTextBlock.Foreground = new SolidColorBrush(Colors.Red);
            e.Handled = true;
        }

        
    }
}
