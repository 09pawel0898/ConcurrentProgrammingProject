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

namespace paczkomaty
{

    public partial class MainWindow : Window
    {
        Simulation m_Simulation;
        public MainWindow()
        {
            InitializeComponent();
            m_Simulation = new Simulation(MainCanvas);
        }

        private void B1OnClick(object sender, RoutedEventArgs e)
        {

        }
        private void ProgramExit(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
