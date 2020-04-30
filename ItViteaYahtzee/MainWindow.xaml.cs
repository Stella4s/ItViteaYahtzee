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

namespace ItViteaYahtzee
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Declare private variables.
        private Random rnd = new Random();

        //Declare public variables
        public List<int> diceResults;

        public MainWindow()
        {
            InitializeComponent();
            diceResults = new List<int>
            {
                5,
                1,
                4,
                4,
                3
            };
            iCtrlDice.ItemsSource = diceResults;
        }
        
    }
}
