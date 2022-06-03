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
        int serialCount = 0;
        String serialData = "";
   

        //STM32 Serial driver object and settings
        SerialStmConf serialConf = new();
        SerialStm serialDriver = new();


        public MainWindow()
        {
            InitializeComponent();

            ConfigureSerialComm();
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



        //Click button event, adds new data to serial text box and scrolls to end
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            String newData = String.Format("\nData:{0}", serialCount);

            serialData += newData;
            SerialTextBlock.Text = serialData;
            serialCount++;

            SerialScroller.ScrollToEnd();
        }


        //Refresh serial port lis button
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateComboBox();
        }

        //Combobox selection change
        private void cbxPortSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
 
            serialConf.PortName = (String)cbxPortSelector.SelectedItem;
            Trace.WriteLine($"Selected port: {serialConf.PortName}");

            serialDriver.SerialUpdateSettings(serialConf);
        }
    }
}
