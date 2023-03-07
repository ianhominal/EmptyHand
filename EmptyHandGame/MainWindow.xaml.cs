using Domain.Interfaces;
using Domain.Models;
using Google.Apis.PeopleService.v1.Data;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace EmptyHandGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMenuUpdater
    {

        GoogleService googleService;
        SignalRService signalRService;

        Payload user;

        string userId;


        public MainWindow()
        {
            InitializeComponent();
            googleService = new GoogleService();
            gameGuid = null;
        }

        private async void Game_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (gameGuid != null)
            {
                await signalRService.CancelCreateGame(gameGuid.Value);
            }
        }

        private async void NewGame_Click(object sender, RoutedEventArgs e)
        {
            var actualPlayer = new PlayerModel(userId, user.Name, user.Picture, user.Email);

            await signalRService.CreateGame(actualPlayer);
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
            try
            {
                googleService.GoogleLogout();

                user = await googleService.GoogleLogin();
                if (user != null)
                {
                    StkLogin.Visibility = Visibility.Collapsed;
                    GrdMainMenu.Visibility = Visibility.Visible;

                    txtUserName.Text = user.Name;

                    var photoUrl = user.Picture;
                    userId = user.JwtId;
                    // userId = user.ResourceName.Split('/')[1];

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

                    signalRService = new SignalRService("MainMenuHub");
                    signalRService.SetMenuUpdater(this);
                    await signalRService.Conectar();

                    await signalRService.Authenticate(user.JwtId);

                    //   signalRService.SetMenuUpdater(this);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private async void Logout_Click(object sender, RoutedEventArgs e)
        {
            googleService.GoogleLogout();
            StkLogin.Visibility = Visibility.Visible;
            GrdMainMenu.Visibility = Visibility.Collapsed;

            txtUserName.Text = string.Empty;
            imgUser.Children.Clear();
        }

        #region GameListRefresh
        public void RefreshGameList(List<GameModel> games)
        {
            Dispatcher.Invoke(() =>
            {
                var maxColumns = 2;

                GrdGameList.Children.Clear();
                GrdGameList.HorizontalAlignment = HorizontalAlignment.Left;
                GrdGameList.Margin = new Thickness(20, 10, 20, 10);
                //                GrdGameList.Height = ((int)(games.Count / maxColumns) + 1) * 150;
                //this.Height = RowEnemy.Height + RowDeck.Height + RowPlayerHand.Height;

                if (games.Count > maxColumns)
                {
                    GrdGameList.Width = maxColumns * 150;

                    for (int i = 0; i < (int)(games.Count / maxColumns) + 1; i++)
                    {
                        GrdGameList.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(150) });
                    }

                    for (int i = 0; i < maxColumns; i++)
                    {
                        GrdGameList.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(150) });
                    }
                }
                else
                {
                    GrdGameList.Width = games.Count * 150;
                    for (int i = 0; i < games.Count; i++)
                    {
                        GrdGameList.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(150) });
                    }
                }

                //defino las columnas
                var colCount = 0;
                var rowCount = 0;
                foreach (GameModel game in games)
                {
                    var container = new StackPanel();

                    var imgUser = new Grid
                    {
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center
                    };

                    if (!string.IsNullOrEmpty(game.Player1.PlayerPhoto))
                    {
                        Ellipse playerImg = PlayerModel.GetUserPhoto(game.Player1.PlayerPhoto);
                        Ellipse playerBg = PlayerModel.GetBackgroundUserEllipse();

                        imgUser.Children.Clear();
                        // Agrega el objeto "Ellipse" con la imagen y borde al objeto "Canvas".
                        imgUser.Children.Add(playerBg);
                        imgUser.Children.Add(playerImg);

                        imgUser.UpdateLayout();
                    }

                    container.Children.Add(imgUser);

                    var txtUserName = new TextBlock
                    {
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        FontWeight = FontWeights.Bold,
                        Text = game.Player1.PlayerName,
                        Margin = new Thickness(0, 10, 0, 0)
                    };
                    container.Children.Add(txtUserName);


                    container.MouseEnter += (s, e) => { GameCard_MouseEnter(game.GameId, container); };
                    container.MouseLeave += (s, e) => { GameCard_MouseLeave(game.GameId, container); };
                    container.MouseLeftButtonUp += async (s, e) => { await GameCard_MouseClick(game.GameId, container, game); };


                    Grid.SetColumn(container, colCount);
                    Grid.SetRow(container, rowCount);

                    GrdGameList.Children.Add(container);


                    if (colCount < maxColumns)
                    {
                        colCount++;
                    }
                    else
                    {
                        colCount = 0;
                        rowCount++;
                    }
                }
            });
        }

        private void GameCard_MouseEnter(Guid gameId, StackPanel selectedCard)
        {
            Mouse.OverrideCursor = Cursors.Hand;
            selectedCard.Margin = new Thickness(0, 0, 0, 10);
        }

        private void GameCard_MouseLeave(Guid gameId, StackPanel selectedCard)
        {
            Mouse.OverrideCursor = null;
            selectedCard.Margin = new Thickness(0, 0, 0, 0);
        }

        private async Task GameCard_MouseClick(Guid gameId, StackPanel selectedCard, GameModel selectedGame)
        {
            var player2 = new PlayerModel(userId, user.Name, user.Picture, user.Email);
            await signalRService.JoinGame(player2, selectedGame);
        }

        #endregion

        Guid? gameGuid;
        public void CreateNewGame(GameModel game)
        {
            Dispatcher.Invoke(() =>
            {
                gameGuid = game.GameId;
                GrdWaitingPlayer2.Visibility = Visibility.Visible;
            });
        }

        private async void BtnCancelCreateGame_Click(object sender, RoutedEventArgs e)
        {
            GrdWaitingPlayer2.Visibility = Visibility.Collapsed;
            if (gameGuid != null)
            {
                await signalRService.CancelCreateGame(gameGuid.Value);
            }
        }


        Game? gameForm = null;
        public void StartNewGame(GameModel game)
        {
            Dispatcher.Invoke(() =>
            {
                if (gameForm == null)
                {
                    GrdWaitingPlayer2.Visibility = Visibility.Collapsed;
                    gameForm = new Game(game, userId);
                    gameForm.ShowDialog();
                }
            });

        }

        public void GameClosed(string enemyPlayer)
        {
            Dispatcher.Invoke(() =>
            {
                if(enemyPlayer != user.Name)
                {
                    txtGameClosed.Text = $"El jugador {enemyPlayer} ha salido";
                    GrdGameClosed.Visibility = Visibility.Visible;
                }
            });
        }

        public void btnAceptarGameClosed_Click(object sender, RoutedEventArgs e)
        {
            GrdGameClosed.Visibility = Visibility.Collapsed;
        }
        
    }
}
