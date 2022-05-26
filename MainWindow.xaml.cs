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

namespace Motion_Tracking_UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int serialCount = 0;
        String serialData = "";
        public MainWindow()
        {
            InitializeComponent();
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
    }
}
