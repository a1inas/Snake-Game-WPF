using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
    public partial class MenuWindow : Window
    {
        private MediaPlayer player;
        public MenuWindow()
        {
            InitializeComponent();
            player = new MediaPlayer();
            player.Open(new Uri("Assets/music.mp3", UriKind.Relative));
            player.MediaEnded += PlayerLoop;
            player.Play(); 
        }
        void PlayerLoop(object sender, EventArgs args)
        {
            player.Position = TimeSpan.Zero;
            player.Play();
        }
        public MenuWindow(MediaPlayer player)
        {
            InitializeComponent();
            this.player = player;
        }


        private void Music(object sender, RoutedEventArgs e)
        {
            player.Play();
          
        }

        private void No_Music(object sender, RoutedEventArgs e)
        {
            player.Pause();
        }

        private void Play(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(player);

            mainWindow.Left = this.Left;
            mainWindow.Top = this.Top;

            mainWindow.Show();

            Close();
        }

        private void Play2(object sender, RoutedEventArgs e)
        {
            MainWindow2 mainWindow = new MainWindow2(player);

            mainWindow.Left = this.Left;
            mainWindow.Top = this.Top;

            mainWindow.Show();

            Close();
        }

        private void Rules(object sender, RoutedEventArgs e)
        {
            RulesWindow rulesWindow = new RulesWindow(player);

            rulesWindow.Left = this.Left;
            rulesWindow.Top = this.Top;

            rulesWindow.Show();

            Window.GetWindow(this)?.Close();
        }


        private void Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
