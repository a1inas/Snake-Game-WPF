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

namespace Snake
{
    public partial class RulesWindow : Window
    {
        MediaPlayer player;
        public RulesWindow(MediaPlayer player)
        {
            InitializeComponent();
            this.player = player;
        }
        private void Back(object sender, RoutedEventArgs e)
        {
            MenuWindow menuWindow = new MenuWindow(player);

            menuWindow.Left = this.Left;
            menuWindow.Top = this.Top;

            menuWindow.Show();

            Window.GetWindow(this)?.Close();
        }

    }
}
