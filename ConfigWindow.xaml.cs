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
using System.Windows.Shapes;

namespace ParcelLockers
{
    /// <summary>
    /// Logika interakcji dla klasy ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        bool closedByStarting = false;
        public ConfigWindow()
        {
            InitializeComponent();
        }
        
        private void SetInitialValues()
        {

        }

        private void ProgramExit(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(!closedByStarting)
                Environment.Exit(0);
        }

        private void bNumPeople1OnClick(object sender, RoutedEventArgs e)
        {
            TBox1.T
        }

        private void bNumPeople2OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void bSimSpeed1OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void bSimSpeed2OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void bStartOnClick(object sender, RoutedEventArgs e)
        {
            MainWindow.simulation.Thread.Interrupt();
            closedByStarting = true;
            this.Close();
        }
    }
}
