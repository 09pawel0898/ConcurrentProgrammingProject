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
        private const int MAX_PEOPLE = 20;
        private const int MAX_SPEED = 10;
        private bool closedByStarting = false;
        private int numPeopleInSimulation;
        private int simulationSpeed;

        public ConfigWindow()
        {
            InitializeComponent();
            SetInitialValues();
        }
        
        private void SetInitialValues()
        {
            numPeopleInSimulation = 7;
            TBox1.Text = "7";
            simulationSpeed = 5;
            TBox2.Text = "5";
        }

        private void ProgramExit(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(!closedByStarting)
                Environment.Exit(0);
        }

        private void bNumPeople1OnClick(object sender, RoutedEventArgs e)
        {
            if (numPeopleInSimulation < MAX_PEOPLE)
            {
                numPeopleInSimulation++;
                TBox1.Text = numPeopleInSimulation.ToString();
            }
        }

        private void bNumPeople2OnClick(object sender, RoutedEventArgs e)
        {
            if (numPeopleInSimulation > 7)
            {
                numPeopleInSimulation--;
                TBox1.Text = numPeopleInSimulation.ToString();
            }
        }

        private void bSimSpeed1OnClick(object sender, RoutedEventArgs e)
        {
            if (simulationSpeed < MAX_SPEED)
            {
                simulationSpeed++;
                TBox2.Text = simulationSpeed.ToString();
            }
        }

        private void bSimSpeed2OnClick(object sender, RoutedEventArgs e)
        {
            if (simulationSpeed > 1)
            {
                simulationSpeed--;
                TBox2.Text = simulationSpeed.ToString();
            }
        }

        private void bStartOnClick(object sender, RoutedEventArgs e)
        {
            MainWindow.simulation.Thread.Interrupt();
            closedByStarting = true;
            Defines.numPeopleInSimulation = numPeopleInSimulation;
            Defines.simulationSpeed = simulationSpeed;
            this.Close();  
        }
    }
}
