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
using Demo.Protocol;
using System.Windows.Threading;
using Demo.Utilities;

namespace Demo.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModel _viewModel;
        private IController _controller;
        
        public string seqfile = "loltest.seq";

        private delegate void oneatgthingie(string s);
        
        

        public MainWindow()
        {
            _viewModel = new ViewModel();
            DataContext = _viewModel;
            InitializeComponent();
            _controller = new Controller(_viewModel, this);
           

            // Loading config.
            //
            //  Tries to find and load a config file named
            //  server.config.json. 
            //  If it fails to find it, we have provided a 
            //  Default json string for config in the project resources.
            //


            dynamic config = DynamicConfig.GetConfig(
               "client.config.txt",
               Demo.Client.Properties.Resources.default_config);

            


            try
            {
                _controller.Connect((string)config.server.ip, (int)config.server.port);
            }
            catch(Exception e) 
            {
                MessageBox.Show(HUMBUG(e));
            }

        }

        public static string HUMBUG(Exception e) {
            if (e.InnerException != null)
            {
                return String.Format(
                    "{0}\n\r\n\r{1}", e.Message,
                    HUMBUG(e.InnerException)
                 );
            }
            else {
                return e.Message;
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.StartExecution(seqfile);

            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new oneatgthingie(_controller.StartExecution), seqfile); 


            //((Controller)_controller).Test();    

        }

        
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.StopExecution(seqfile);
        }

        private void ResumeButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.ResumeExecution(seqfile);
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            _controller.PauseExecution(seqfile);
        }




    }
}
