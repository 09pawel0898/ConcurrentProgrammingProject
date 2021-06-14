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

namespace ParcelLockers
{

    public partial class MainWindow : Window
    {
        public static Simulation simulation;
        public MainWindow()
        {
            InitializeComponent();
            simulation = new Simulation(MainCanvas,this);
        }

        private void ProgramExit(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
       
    }
}
