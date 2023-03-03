using DataService;
using Domain.Interfaces;
using Domain.Models;
using Google.Apis.PeopleService.v1.Data;
using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;
using Service;
using System;
using System.CodeDom;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Card = Domain.Models.Card;

namespace EmptyHandGame
{
    /// <summary>
    /// Lógica de interacción para Game.xaml
    /// </summary>
    public partial class Game : Window, IGameUpdater
    {
        GameHeaderModel gameState;
        Person user;
        string userId;
        Canvas deckImg;
        bool turnStarted;

        PlayerCardsModel actualPlayerInfo;
        PlayerCardsModel enemyPlayerInfo;

        string playerTurnId;

        private SignalRService signalRClient;

        public Game(GameHeaderModel _gameState, Person _user, string _userId)
        {
            InitializeComponent();

            userId = _userId;
            gameState = _gameState;
            user = _user;
            signalRClient = new SignalRService(this);


            DrawPlayersInfo();
            UpdateGame();

        }

        public void UpdateGame()
        {
            Dispatcher.Invoke(() => {
                gameState = GameService.ToModel(Context.GetGameHeader(gameState.GameId.ToString()), userId, user);
                btnEndTurn.IsEnabled = false;

                DrawPlayerHand();
                DrawPlayerLifeCards();
                DrawEnemyHand();
                DrawEnemyLifeCards();
                GenerateDeckAndPits();

                playerTurnId = gameState.ActualGameRound.Player1Cards.PlayerTurn ? gameState.ActualGameRound.Player2Cards.PlayerId : gameState.ActualGameRound.Player1Cards.PlayerId;

                var txtTurno = playerTurnId == userId ? "Es tu turno" : "Turno del otro jugador";
                txtTurnoActual.Text = txtTurno;
                turnStarted = false;
            });
        }


        private void DrawPlayersInfo()
        {
            actualPlayerInfo = gameState.ActualGameRound.Player1Cards.PlayerId == userId ? gameState.ActualGameRound.Player1Cards : gameState.ActualGameRound.Player2Cards;
            enemyPlayerInfo = gameState.ActualGameRound.Player1Cards.PlayerId == userId ? gameState.ActualGameRound.Player2Cards : gameState.ActualGameRound.Player1Cards;

            txtPlayerName.Text = actualPlayerInfo.PlayerName;
            txtEnemyName.Text = enemyPlayerInfo.PlayerName;

            var playerPhoto = actualPlayerInfo.PlayerPhoto;
            var enemyPhoto = enemyPlayerInfo.PlayerPhoto;


            if (!string.IsNullOrEmpty(playerPhoto))
            {
                Ellipse playerImg = GetUserPhoto(playerPhoto);
                Ellipse playerBg = GetBackgroundUserEllipse();


                // Agrega el objeto "Ellipse" con la imagen y borde al objeto "Canvas".
                imgPlayer.Children.Add(playerBg);
                imgPlayer.Children.Add(playerImg);

                imgPlayer.UpdateLayout();
            }

            if (!string.IsNullOrEmpty(enemyPhoto))
            {
                Ellipse enemyImg = GetUserPhoto(enemyPhoto);
                Ellipse enemyBg = GetBackgroundUserEllipse();


                // Agrega el objeto "Ellipse" con la imagen y borde al objeto "Canvas".
                imgEnemy.Children.Add(enemyBg);
                imgEnemy.Children.Add(enemyImg);

                imgEnemy.UpdateLayout();
            }


        }

        private Ellipse GetUserPhoto(string url)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(url, UriKind.Absolute);
            bitmap.EndInit();

            // Crea un nuevo objeto "ImageBrush" y asigna la imagen cargada a su propiedad "ImageSource".
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = bitmap;



            Ellipse ellipse = new Ellipse();
            ellipse.Width = 100;
            ellipse.Height = 100;
            //Color color = (Color)ColorConverter.ConvertFromString("#FF673AB7");
            //ellipse.Stroke = new SolidColorBrush(color);
            ////21005D
            //ellipse.StrokeThickness = 4;

            // Asigna el objeto "ImageBrush" a la propiedad "Fill" del objeto "Ellipse".
            ellipse.Fill = imageBrush;

            return ellipse;

        }

        private Ellipse GetBackgroundUserEllipse()
        {
            Ellipse ellipsePlayerBg = new Ellipse();
            ellipsePlayerBg.Width = 106;
            ellipsePlayerBg.Height = 106;
            //ellipsePlayerBg.Margin = new Thickness(0, 0, 0, 0);

            Color color = (Color)ColorConverter.ConvertFromString("#21005D");

            ellipsePlayerBg.Fill = new SolidColorBrush(color);

            return ellipsePlayerBg;
        }


        private void GenerateDeckAndPits()
        {
            RowDeck.Children.Clear();

            var deckGrid = new Grid();

            deckGrid.HorizontalAlignment = HorizontalAlignment.Center;
            deckGrid.VerticalAlignment = VerticalAlignment.Center;

            deckGrid.ColumnDefinitions.Add(new ColumnDefinition());

            deckImg = Card.CreateCardImage("Diamonds", "A", false, gameState.ActualGameRound?.AvailableCardsObj.Count > 0);
            deckImg.Margin = new Thickness(5, 5, 20, 5);

            deckImg.MouseLeftButtonUp += (s, e) => { GetCardsFromDeck(); };

            Grid.SetColumn(deckImg, 0);

            deckGrid.Children.Add(deckImg);


            var cardCount = 1;

            foreach (var pit in gameState.ActualGameRound.CardPitsObj)
            {
                deckGrid.ColumnDefinitions.Add(new ColumnDefinition());

                var lastPitCard = pit.Value.Last();
                lastPitCard.CanBePlayed = false;

                var cardImage = lastPitCard.Image;
                cardImage.Margin = new Thickness(5);

                int pitRow = pit.Key < 3 ? 0 : 1;

                Grid.SetColumn(cardImage, cardCount);


                // Obtener el elemento padre del Canvas
                var parent = VisualTreeHelper.GetParent(cardImage) as Grid;

                // Remover el Canvas del elemento padre si existe
                if (parent != null)
                {
                    parent.Children.Remove(cardImage);
                }

                
                deckGrid.Children.Add(cardImage);

                cardCount++;

            }

            RowDeck.Children.Add(deckGrid);

        }

        private void DrawEnemyHand()
        {
            RowEnemyHand.Children.RemoveRange(0, RowEnemyHand.Children.Count);
            var handGrid = new Grid();

            handGrid.HorizontalAlignment = HorizontalAlignment.Center;
            handGrid.VerticalAlignment = VerticalAlignment.Bottom;

            var handCards = enemyPlayerInfo.PlayerCardsObj;


            //defino las columnas
            var cardCount = 0;
            foreach (var card in handCards)
            {
                card.CanBePlayed = false;
                handGrid.ColumnDefinitions.Add(new ColumnDefinition());

                var cardImage = Card.CreateCardImage("Diamonds", "A", false, gameState.ActualGameRound?.AvailableCardsObj.Count > 0);
                cardImage.Margin = new Thickness(5);
                Grid.SetColumn(cardImage, cardCount);

                // Obtener el elemento padre del Canvas
                var parent = VisualTreeHelper.GetParent(cardImage) as Grid;

                // Remover el Canvas del elemento padre si existe
                if (parent != null)
                {
                    parent.Children.Remove(cardImage);
                }

                handGrid.Children.Add(cardImage);

                cardCount++;
            }

            RowEnemyHand.Children.Add(handGrid);
        }

        private void DrawEnemyLifeCards()
        {
            RowEnemyLifes.Children.RemoveRange(0, RowEnemyLifes.Children.Count);
            var playerLifeGrid = new Grid();

            playerLifeGrid.HorizontalAlignment = HorizontalAlignment.Right;
            playerLifeGrid.VerticalAlignment = VerticalAlignment.Bottom;

            var lifeCards = enemyPlayerInfo.PlayerLifeCardsObj;

            //for (int i = 0; i <= 3; i++)
            //{
            //    handGrid.RowDefinitions.Add(new RowDefinition());
            //}

            //defino las columnas
            var cardCount = 0;
            foreach (var card in lifeCards)
            {
                playerLifeGrid.ColumnDefinitions.Add(new ColumnDefinition());

                card.CanBePlayed = false;

                var cardImage = Card.CreateCardImage("Diamonds", "A", false, gameState.ActualGameRound?.AvailableCardsObj.Count > 0);

                cardImage.Margin = new Thickness(5);
                Grid.SetColumn(cardImage, cardCount);

                // Obtener el elemento padre del Canvas
                var parent = VisualTreeHelper.GetParent(cardImage) as Grid;

                // Remover el Canvas del elemento padre si existe
                if (parent != null)
                {
                    parent.Children.Remove(cardImage);
                }
                playerLifeGrid.Children.Add(cardImage);

                cardCount++;
            }

            RowEnemyLifes.Children.Add(playerLifeGrid);
        }

        private void DrawPlayerLifeCards()
        {
            RowPlayerLifes.Children.RemoveRange(0, RowPlayerLifes.Children.Count);

            var playerLifeGrid = new Grid();

            playerLifeGrid.HorizontalAlignment = HorizontalAlignment.Left;
            playerLifeGrid.VerticalAlignment = VerticalAlignment.Bottom;

            var lifeCards = actualPlayerInfo.PlayerLifeCardsObj;

            var cardCount = 0;
            foreach (var card in lifeCards)
            {
                playerLifeGrid.ColumnDefinitions.Add(new ColumnDefinition());

                card.CanBePlayed = true;

                var cardImage = Card.CreateCardImage("Diamonds", "A", false, gameState.ActualGameRound?.AvailableCardsObj.Count > 0); ;
                cardImage.Margin = new Thickness(5);
                Grid.SetColumn(cardImage, cardCount);

                cardImage.MouseEnter += (s, e) => { LifeCard_MouseEnter(card.Number, card.Image); };
                cardImage.MouseLeave += (s, e) => { LifeCard_MouseLeave(card.Number, card.Image); };
                cardImage.MouseLeftButtonUp += (s, e) => { LifeCard_Click(cardImage, card); };

                // Obtener el elemento padre del Canvas
                var parent = VisualTreeHelper.GetParent(cardImage) as Grid;

                // Remover el Canvas del elemento padre si existe
                if (parent != null)
                {
                    parent.Children.Remove(cardImage);
                }
                playerLifeGrid.Children.Add(cardImage);

                cardCount++;
            }

            RowPlayerLifes.Children.Add(playerLifeGrid);

        }



        private async void BtnEndTurn_Click(object sender, RoutedEventArgs e)
        {
            GameService.EndTurn(gameState);
            await signalRClient.EndTurn(gameState.GameId.ToString());
            UpdateGame();
            
        }

        private void DrawPlayerHand()
        {
            //if(playerHandGrid == null) playerHandGrid = new Grid();

            RowPlayerHand.Children.RemoveRange(0, RowPlayerHand.Children.Count);

            RowPlayerHand.HorizontalAlignment = HorizontalAlignment.Center;
            RowPlayerHand.VerticalAlignment = VerticalAlignment.Bottom;

            var maxColums = 20;

            var handCards = actualPlayerInfo.PlayerCardsObj;

            RowPlayerHand.Height = ((int)(handCards.Count / maxColums) + 1) * 150;
            //this.Height = RowEnemy.Height + RowDeck.Height + RowPlayerHand.Height;

            if (handCards.Count > maxColums)
            {
                RowPlayerHand.Width = maxColums * 110;

                for (int i = 0; i < (int)(handCards.Count / maxColums) + 1; i++)
                {
                    RowPlayerHand.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(150) });
                }

                for (int i = 0; i < maxColums; i++)
                {
                    RowPlayerHand.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(110) });
                }
            }
            else
            {
                RowPlayerHand.Width = handCards.Count * 110;
                for (int i = 0; i < handCards.Count; i++)
                {
                    RowPlayerHand.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(110) });
                }
            }

            //defino las columnas
            var cardCount = 0;
            var rowCount = 0;
            foreach (var card in handCards)
            {
                card.CanBePlayed = true; 
                
                var cardImage = card.Image;
                cardImage.Margin = new Thickness(5);
                Grid.SetColumn(cardImage, cardCount);
                Grid.SetRow(cardImage, rowCount);

                // Obtener el elemento padre del Canvas
                var parent = VisualTreeHelper.GetParent(cardImage) as Grid;

                // Remover el Canvas del elemento padre si existe
                if (parent != null)
                {
                    parent.Children.Remove(cardImage);
                }

                if (cardImage.Parent == null)
                {
                    RowPlayerHand.Children.Add(cardImage);
                    cardImage.MouseEnter += (s, e) => { CardImage_MouseEnter(card.Number, card.Image); };
                    cardImage.MouseLeave += (s, e) => { CardImage_MouseLeave(card.Number, card.Image); };
                    cardImage.MouseLeftButtonUp += (s, e) => { HandCard_Click(cardImage, card); };
                }

                if (cardCount < maxColums)
                {
                    cardCount++;
                }
                else
                {
                    cardCount = 0;
                    rowCount++;
                }
            }
        }

        private void HandCard_Click(Canvas cardImage, Card card)
        {
            var availablePit = CardCanBePlayed(card.Number);
            if (availablePit >= 0 && turnStarted && card.CanBePlayed)
            {
                card.CanBePlayed = false;
                actualPlayerInfo.PlayerCardsObj.Remove(card);
                gameState.ActualGameRound.CardPitsObj[availablePit].Add(card);

                cardImage.MouseEnter -= (s, e) => { CardImage_MouseEnter(card.Number, card.Image); };
                cardImage.MouseLeave -= (s, e) => { CardImage_MouseLeave(card.Number, card.Image); };
                cardImage.MouseLeftButtonUp -= (s, e) => { HandCard_Click(cardImage, card); };

                DrawPlayerHand();
                GenerateDeckAndPits();

                btnEndTurn.IsEnabled = true;
            }
            else
            {
                var text = string.Empty;

                if (availablePit == 0) text = "Esta carta no puede ser jugada en ningun pozo.";
                if (!turnStarted) text = "Debes juntar 2 cartas del mazo antes de jugar.";

                if (dialogExample == null && !string.IsNullOrEmpty(text))
                {
                    dialogExample = new MaterialDialog() { Message = $"{text}" };
                    dialogExample.VerticalAlignment = VerticalAlignment.Center;
                    Grid.SetRowSpan(dialogExample, 3);
                    GrdPrincipal.Children.Add(dialogExample);
                    dialogExample.btnClose.Click += (sender, args) => { dialogExample = null; };
                }
            }
        }

        private void CardImage_MouseLeave(int number, Canvas card)
        {
            bool canBePlayed = CardCanBePlayed(number) >= 0;
            Mouse.OverrideCursor = null;

            if (turnStarted && canBePlayed)
            {
                card.Margin = new Thickness(0, 0, 0, 0);
            }
        }

        private void CardImage_MouseEnter(int number, Canvas card)
        {
            bool canBePlayed = CardCanBePlayed(number) >= 0;
            if (turnStarted == false || canBePlayed == false)
            {
                Mouse.OverrideCursor = Cursors.No;
            }
            else
            {
                Mouse.OverrideCursor = Cursors.Hand;

                card.Margin = new Thickness(0, 0, 0, 10);
            }

        }


        private void LifeCard_Click(Canvas cardImage, Card card)
        {
            if (turnStarted && card.CanBePlayed)
            {
                card.CanBePlayed = false;
                actualPlayerInfo.PlayerLifeCardsObj.Remove(card);
                gameState.ActualGameRound.CardPitsObj.Add(gameState.ActualGameRound.CardPitsObj.Count,new System.Collections.Generic.List<Card>() { card });

                cardImage.MouseEnter -= (s, e) => { CardImage_MouseEnter(card.Number, card.Image); };
                cardImage.MouseLeave -= (s, e) => { CardImage_MouseLeave(card.Number, card.Image); };
                cardImage.MouseLeftButtonUp -= (s, e) => { HandCard_Click(cardImage, card); };

                DrawPlayerLifeCards();
                GenerateDeckAndPits();

                btnEndTurn.IsEnabled = true;
            }
            else
            {
                var text = "Debes juntar 2 cartas del mazo antes de jugar.";

                if (dialogExample == null && !string.IsNullOrEmpty(text))
                {
                    dialogExample = new MaterialDialog() { Message = $"{text}" };
                    dialogExample.VerticalAlignment = VerticalAlignment.Center;
                    Grid.SetRowSpan(dialogExample, 3);
                    GrdPrincipal.Children.Add(dialogExample);
                    dialogExample.btnClose.Click += (sender, args) => { dialogExample = null; };
                }
            }
        }
        private void LifeCard_MouseLeave(int number, Canvas card)
        {
            bool canBePlayed = CardCanBePlayed(number) >= 0;
            Mouse.OverrideCursor = null;

            if (turnStarted && canBePlayed)
            {
                card.Margin = new Thickness(0, 0, 0, 0);
            }
        }

        private void LifeCard_MouseEnter(int number, Canvas card)
        {
            if (turnStarted == false)
            {
                Mouse.OverrideCursor = Cursors.No;
            }
            else
            {
                Mouse.OverrideCursor = Cursors.Hand;

                card.Margin = new Thickness(0, 0, 0, 10);
            }

        }

        private int CardCanBePlayed(int cardNumber)
        {
            int nextCard = cardNumber + 1;
            int previousCard = cardNumber - 1;

            if (nextCard > 12) { nextCard = 0; }
            if (previousCard < 0) { previousCard = 12; }

            var pitWhereCardCanBePlayed = gameState.ActualGameRound.CardPitsObj.Where(p => p.Value.Last().Number == nextCard || p.Value.Last().Number == previousCard).ToList();
            if (pitWhereCardCanBePlayed.Count > 0)
            {
                return pitWhereCardCanBePlayed.FirstOrDefault().Key;
            }

            return -1;
        }


        MaterialDialog dialogExample;
        private void GetCardsFromDeck()
        {
            if (playerTurnId == userId)
            {
                if (turnStarted == false)
                {
                    if (gameState.ActualGameRound.AvailableCardsObj.Count == 0)
                    {
                        var cardColor = Card.GetDisabledCard();
                        deckImg.Children.Add(cardColor);
                        deckImg.IsEnabled = false;
                    }
                    else
                    {
                        var cards = gameState.ActualGameRound.AvailableCardsObj.Take(2).ToList();

                        actualPlayerInfo.PlayerCardsObj.AddRange(cards);

                        foreach (var card in cards)
                        {
                            gameState.ActualGameRound.AvailableCardsObj.Remove(card);
                        }

                        DrawPlayerHand();
                    }
                    turnStarted = true;
                }
                else
                {
                    if (dialogExample == null)
                    {
                        dialogExample = new MaterialDialog() { Message = "Ya has juntado las 2 cartas de tu turno." };
                        dialogExample.VerticalAlignment = VerticalAlignment.Center;
                        Grid.SetRowSpan(dialogExample, 3);
                        GrdPrincipal.Children.Add(dialogExample);
                        dialogExample.btnClose.Click += (sender, args) => { dialogExample = null; };
                    }
                }
            }
            else
            {
                if (dialogExample == null)
                {
                    string turnoStr;
                    if (playerTurnId == actualPlayerInfo.PlayerId) 
                    {
                        turnoStr = actualPlayerInfo.PlayerName;
                    }
                    else
                    {
                        turnoStr = enemyPlayerInfo.PlayerName;
                    }
                    dialogExample = new MaterialDialog() { Message = $"Es el turno de {turnoStr}." };
                    dialogExample.VerticalAlignment = VerticalAlignment.Center;
                    Grid.SetRowSpan(dialogExample, 3);
                    GrdPrincipal.Children.Add(dialogExample);
                    dialogExample.btnClose.Click += (sender, args) => { dialogExample = null; };
                }
            }
        }


        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {

            await signalRClient.Conectar();

        }


        //private void DrawDeck()
        //{
        //    var deckGrid = new Grid();

        //    deckGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
        //    deckGrid.VerticalAlignment = VerticalAlignment.Stretch;

        //    for (int i = 0; i <= 3; i++)
        //    {
        //        deckGrid.RowDefinitions.Add(new RowDefinition());
        //    }

        //    for (int i = 0; i <= 12; i++)
        //    {
        //        deckGrid.ColumnDefinitions.Add(new ColumnDefinition());
        //    }


        //    foreach (var card in deck)
        //    {

        //        var cardImage = card.Image;

        //        Grid.SetColumn(cardImage, card.Number);

        //        switch (card.Suit)
        //        {
        //            case "Diamonds":
        //                {
        //                    Grid.SetRow(cardImage, 0);
        //                    break;
        //                }
        //            case "Spades":
        //                {
        //                    Grid.SetRow(cardImage, 1);
        //                    break;
        //                }
        //            case "Clubs":
        //                {
        //                    Grid.SetRow(cardImage, 2);
        //                    break;
        //                }
        //            case "Hearts":
        //                {
        //                    Grid.SetRow(cardImage, 3);
        //                    break;
        //                }
        //        }

        //        deckGrid.Children.Add(cardImage);
        //    }

        //    RowDeck.Children.Add(deckGrid);
        //}



    }
}
