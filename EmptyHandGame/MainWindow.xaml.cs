using MaterialDesignThemes.Wpf;
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

namespace EmptyHandGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            BtnTest_Click(null, null);
        }


        private void BtnTest_Click(object sender, RoutedEventArgs e)
        {

            Game game = new Game();
            this.Visibility = Visibility.Collapsed;
            game.ShowDialog();
            game.Closing += Game_Closing;
            //Grid.SetRowSpan(grdFrame, 2);
            //Game game = new();
            //grdFrame.Content = game;

            
        }

        private void Game_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Visibility = Visibility.Visible;
        }
    }
}
