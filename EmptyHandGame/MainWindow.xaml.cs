using DataService;
using Domain.Models;
using Google.Apis.PeopleService.v1.Data;
using MaterialDesignThemes.Wpf;
using Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security.Policy;
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

        GoogleService googleService;

        Context dbConnection;

        Person user;

        string userId;


        public MainWindow()
        {
            InitializeComponent();
            googleService = new GoogleService();
            dbConnection = new Context();

        }


        private void BtnTest_Click(object sender, RoutedEventArgs e)
        {

            //Game game = new Game();
            //this.Visibility = Visibility.Collapsed;
            //game.ShowDialog();
            //game.Closing += Game_Closing;


        }

        private void Game_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Visibility = Visibility.Visible;
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            GameService.CreateNewGame(userId, user, dbConnection);
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            var gameState = GameService.GetGameState(txtGameCode.Text, userId, user, dbConnection);

            if (gameState == null)
            {
                ShowDialog("No se encontro la partida especificada.");
                return;
            }

            if (gameState.GameHeader.Player2Id == null)
            {
                ShowDialog("Esperando que el player 2 acepte la partida.");
                return;
            }

            Game game = new Game(gameState, user, userId, dbConnection);
            // this.Visibility = Visibility.Collapsed;
            game.ShowDialog();
            game.Closing += Game_Closing;
        }

        MaterialDialog dialogExample;
        public void ShowDialog(string text)
        {
            if (dialogExample == null && !string.IsNullOrEmpty(text))
            {
                dialogExample = new MaterialDialog() { Message = $"{text}" };
                dialogExample.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetRowSpan(dialogExample, 3);
                GrdPrincipal.Children.Add(dialogExample);
                dialogExample.btnClose.Click += (sender, args) => { dialogExample = null; };
            }
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            user = await googleService.GoogleLogin();
            if (user != null)
            {
                StkLogin.Visibility = Visibility.Collapsed;
                StkMainMenu.Visibility = Visibility.Visible;
                btnLogout.Visibility = Visibility.Visible;

                txtUserName.Text = user.Names.FirstOrDefault()?.DisplayName;

                var photoUrl = user.Photos.FirstOrDefault()?.Url;
                userId = user.ResourceName.Split('/')[1];

                if (!string.IsNullOrEmpty(photoUrl))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(photoUrl, UriKind.Absolute);
                    bitmap.EndInit();

                    // Crea un nuevo objeto "ImageBrush" y asigna la imagen cargada a su propiedad "ImageSource".
                    ImageBrush imageBrush = new ImageBrush();
                    imageBrush.ImageSource = bitmap;

                    Ellipse ellipseBg = new Ellipse();
                    ellipseBg.Width = 100;
                    ellipseBg.Height = 100;
                    ellipseBg.Margin = new Thickness(0, 5, 0, 0);

                    Color color = (Color)ColorConverter.ConvertFromString("#DDA0A0A0");

                    ellipseBg.Fill = new SolidColorBrush(color);


                    Ellipse ellipse = new Ellipse();
                    ellipse.Width = 100;
                    ellipse.Height = 100;
                    ellipse.Stroke = new SolidColorBrush(Colors.DarkGray);
                    ellipse.StrokeThickness = 1;

                    // Asigna el objeto "ImageBrush" a la propiedad "Fill" del objeto "Ellipse".
                    ellipse.Fill = imageBrush;

                    // Agrega el objeto "Ellipse" con la imagen y borde al objeto "Canvas".
                    imgUser.Children.Add(ellipseBg);
                    imgUser.Children.Add(ellipse);

                    imgUser.UpdateLayout();
                }
            }




        }
        private async void Logout_Click(object sender, RoutedEventArgs e)
        {
            googleService.GoogleLogout();
            StkLogin.Visibility = Visibility.Visible;
            StkMainMenu.Visibility = Visibility.Collapsed;
            btnLogout.Visibility = Visibility.Collapsed;

            txtUserName.Text = string.Empty;
            imgUser.Children.Clear();
        }

    }
}
