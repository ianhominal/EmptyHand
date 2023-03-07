using Domain.Interfaces;
using Domain.Models;
using Microsoft.Maui.Controls.Shapes;
using Service;
using System.Drawing;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace MAUI.EmptyHand
{
    public partial class MainPage : ContentPage, IMenuUpdater
    {

        GoogleService googleService;
        SignalRService signalRService;
        Guid? gameGuid;

        Payload user;

        string userId;


        public MainPage()
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

        private async void NewGame_Click(System.Object sender, System.EventArgs e)
        {
            var actualPlayer = new PlayerModel(userId, user.Name, user.Picture, user.Email);

            await signalRService.CreateGame(actualPlayer);
        }


        //MaterialDialog dialogExample;
        //public void ShowDialog(string text)
        //{
        //    if (dialogExample == null && !string.IsNullOrEmpty(text))
        //    {
        //        dialogExample = new MaterialDialog() { Message = $"{text}" };

        //        dialogExample.VerticalAlignment = VerticalAlignment.Center;
        //        Grid.SetRowSpan(dialogExample, 3);
        //        GrdPrincipal.Children.Add(dialogExample);
        //        dialogExample.btnClose.Click += (sender, args) => { dialogExample = null; };
        //    }
        //}

        private async void Login_Click(System.Object sender, System.EventArgs e)
        {
            googleService.GoogleLogout();

            user = await googleService.GoogleLogin();
            if (user != null)
            {
                StkLogin.IsVisible = false;
                GrdMainMenu.IsVisible = true;

                txtUserName.Text = user.Name;

                var photoUrl = user.Picture;
                userId = user.JwtId;


                if (!string.IsNullOrEmpty(photoUrl))
                {
                    var img = CreateCircleImageFromUrl(photoUrl);
                }

            }

        }

        public static async Task<View> CreateCircleImageFromUrl(string imageUrl)
        {
            //    var image = new CachedImage
            //    {
            //        WidthRequest = 100,
            //        HeightRequest = 100,
            //        LoadingPlaceholder = "placeholder.png",
            //        ErrorPlaceholder = "error.png",
            //        Source = imageUrl,
            //        Transformations = new List<FFImageLoading.Work.ITransformation>
            //{
            //    new CircleTransformation()
            //}
            //    };
            //    await image.ReloadImageAsync();
            //    return image;
            return null;
        }



        private async void Logout_Click(System.Object sender, System.EventArgs e)
        {
            googleService.GoogleLogout();
            StkLogin.IsVisible = true;
            GrdMainMenu.IsVisible = false;

            txtUserName.Text = string.Empty;
            imgUser.Children.Clear();
        }

        #region GameListRefresh
        public async void RefreshGameList(List<GameModel> games)
        {
            var maxColumns = 2;

            GrdGameList.Children.Clear();
            //GrdGameList.LayoutOptions = new LayoutOptions(LayoutAlignment.Start, false);//.Left;
            GrdGameList.Margin = new Thickness(20, 10, 20, 10);

            if (games.Count > maxColumns)
            {
                //GrdGameList.Width = maxColumns * 150;

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
                //GrdGameList.Width = games.Count * 150;
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
                var container = new StackLayout();

                var imgUser = new Grid
                {
                    //LayoutOptions = new LayoutOptions(LayoutAlignment.Start, false)//.Center
                };

                if (!string.IsNullOrEmpty(game.Player1.PlayerPhoto))
                {
                    var playerImg = CreateCircleImageFromUrl(game.Player1.PlayerPhoto);
                    // Ellipse playerBg = GetBackgroundUserEllipse();
                    //todo mostrar imagen
                    //imgUser.Children.Clear();
                    //// Agrega el objeto "Ellipse" con la imagen y borde al objeto "Canvas".
                    //imgUser.Children.Add(playerImg);

                }

                container.Children.Add(imgUser);

                var txtUserName = new Label
                {
                    //LayoutOptions = new LayoutOptions(LayoutAlignment.Center, false),//.Center,
                    //FontWeight = FontWeights.Bold,
                    Text = game.Player1.PlayerName,
                    Margin = new Thickness(0, 10, 0, 0)
                };
                container.Children.Add(txtUserName);



                container.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(async () =>
                    {
                        await GameCard_MouseClick(game.GameId, container, game);
                    })
                });


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
        }



        private void GameCard_MouseEnter(Guid gameId, StackLayout selectedCard)
        {
            selectedCard.Margin = new Thickness(0, 0, 0, 10);
        }

        private void GameCard_MouseLeave(Guid gameId, StackLayout selectedCard)
        {
            selectedCard.Margin = new Thickness(0, 0, 0, 0);
        }

        private async Task GameCard_MouseClick(Guid gameId, StackLayout selectedCard, GameModel selectedGame)
        {
            var player2 = new PlayerModel(userId, user.Name, user.Picture, user.Email);
            await signalRService.JoinGame(player2, selectedGame);
        }

        #endregion

        public void CreateNewGame(GameModel game)
        {
            gameGuid = game.GameId;
            GrdWaitingPlayer2.IsVisible = true;
        }

        private async void BtnCancelCreateGame_Click(System.Object sender, System.EventArgs e)
        {
            GrdWaitingPlayer2.IsVisible = false;
            if (gameGuid != null)
            {
                await signalRService.CancelCreateGame(gameGuid.Value);
            }
        }


        Game? gameForm = null;
        public void StartNewGame(GameModel game)
        {
            //if (gameForm == null)
            //{
            //    GrdWaitingPlayer2.Visibility = Visibility.Collapsed;
            //    gameForm = new Game(game, userId);
            //    gameForm.ShowDialog();
            //}

        }

        public void GameClosed(string enemyPlayer)
        {
            if (enemyPlayer != user.Name)
            {
                txtGameClosed.Text = $"El jugador {enemyPlayer} ha salido";
                GrdGameClosed.IsVisible = true;
            }
        }

        public void btnAceptarGameClosed_Click(System.Object sender, System.EventArgs e)
        {
            GrdGameClosed.IsVisible = false;
        }

    }
}