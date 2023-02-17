using Domain.Models;
using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Service;
using System;
using System.CodeDom;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Card = Domain.Models.Card;

namespace EmptyHandGame
{
    /// <summary>
    /// Lógica de interacción para Game.xaml
    /// </summary>
    public partial class Game : Window
    {
        GameHeader gameInstance;
        User player;
        Canvas deckImg;

        bool turnStarted;

        public Game()
        {
            InitializeComponent();

            player = GameService.Login("", "");

            gameInstance = GameService.GetActualGame("", player.UserId);

            DrawPlayerHand();
            DrawPlayerLifeCards();
            DrawEnemyHand();
            DrawEnemyLifeCards();
            GenerateDeckAndPits();

            turnStarted = false;
        }

        private void GenerateDeckAndPits()
        {
            RowDeck.Children.RemoveRange(0, RowDeck.Children.Count);

            var deckGrid = new Grid();

            deckGrid.HorizontalAlignment = HorizontalAlignment.Center;
            deckGrid.VerticalAlignment = VerticalAlignment.Center;

            deckGrid.ColumnDefinitions.Add(new ColumnDefinition());
            deckImg = Card.CreateCardImage("Diamonds", "A", false, gameInstance.ActualRound.AvailableCards.Count() > 0);
            deckImg.Margin = new Thickness(5, 5, 20, 5);

            deckImg.MouseLeftButtonUp += (s, e) => {  GetCardsFromDeck(); };

            Grid.SetColumn(deckImg, 0);

            deckGrid.Children.Add(deckImg);
            

            var cardCount = 1;

            foreach (var pit in gameInstance.ActualRound.CardPitsObj)
            {
                deckGrid.ColumnDefinitions.Add(new ColumnDefinition());

                var lastPitCard = pit.Value.Last();

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
            var handGrid = new Grid();

            handGrid.HorizontalAlignment = HorizontalAlignment.Center;
            handGrid.VerticalAlignment = VerticalAlignment.Bottom;

            var handCards = gameInstance.ActualRound.Player2CardsObj;

            //for (int i = 0; i <= 3; i++)
            //{
            //    handGrid.RowDefinitions.Add(new RowDefinition());
            //}

            //defino las columnas
            var cardCount = 0;
            foreach (var card in handCards)
            {
                handGrid.ColumnDefinitions.Add(new ColumnDefinition());


                var cardImage = card.Image;
                cardImage.Margin = new Thickness(5);
                Grid.SetColumn(cardImage, cardCount);

                handGrid.Children.Add(cardImage);

                cardCount++;
            }

            RowEnemyHand.Children.Add(handGrid);
        }

        private void DrawEnemyLifeCards()
        {
            var playerLifeGrid = new Grid();

            playerLifeGrid.HorizontalAlignment = HorizontalAlignment.Right;
            playerLifeGrid.VerticalAlignment = VerticalAlignment.Bottom;

            var lifeCards = gameInstance.ActualRound.Player2LifeCardsObj;

            //for (int i = 0; i <= 3; i++)
            //{
            //    handGrid.RowDefinitions.Add(new RowDefinition());
            //}

            //defino las columnas
            var cardCount = 0;
            foreach (var card in lifeCards)
            {
                playerLifeGrid.ColumnDefinitions.Add(new ColumnDefinition());


                var cardImage = card.Image;
                cardImage.Margin = new Thickness(5);
                Grid.SetColumn(cardImage, cardCount);

                playerLifeGrid.Children.Add(cardImage);

                cardCount++;
            }

            RowEnemyLifes.Children.Add(playerLifeGrid);
        }

        private void DrawPlayerLifeCards()
        {
            var playerLifeGrid = new Grid();

            playerLifeGrid.HorizontalAlignment = HorizontalAlignment.Left;
            playerLifeGrid.VerticalAlignment = VerticalAlignment.Bottom;

            var lifeCards = gameInstance.ActualRound.PlayerLifeCardsObj;

            //for (int i = 0; i <= 3; i++)
            //{
            //    handGrid.RowDefinitions.Add(new RowDefinition());
            //}

            //defino las columnas
            var cardCount = 0;
            foreach (var card in lifeCards)
            {
                playerLifeGrid.ColumnDefinitions.Add(new ColumnDefinition());


                var cardImage = card.Image;
                cardImage.Margin = new Thickness(5);
                Grid.SetColumn(cardImage, cardCount);

                playerLifeGrid.Children.Add(cardImage);

                cardCount++;
            }

            RowPlayerLifes.Children.Add(playerLifeGrid);
        }

        private void DrawPlayerHand()
        {
            //if(playerHandGrid == null) playerHandGrid = new Grid();

            RowPlayerHand.HorizontalAlignment = HorizontalAlignment.Center;
            RowPlayerHand.VerticalAlignment = VerticalAlignment.Bottom;

            var maxColums = 20;

            var handCards = gameInstance.ActualRound.PlayerCardsObj;

            RowPlayerHand.Height = ((int)(handCards.Count / maxColums) +1) * 150;
            //this.Height = RowEnemy.Height + RowDeck.Height + RowPlayerHand.Height;

            if (handCards.Count > maxColums)
            {
                RowPlayerHand.Width = maxColums * 110;

                for (int i = 0; i < (int)(handCards.Count / maxColums) +1; i++)
                {
                    RowPlayerHand.RowDefinitions.Add(new RowDefinition() { Height =  new GridLength(150) });
                }

                for (int i = 0; i < maxColums; i++)
                {
                    RowPlayerHand.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(110) });
                }
            }
            else
            {
                RowPlayerHand.Width = handCards.Count * 110;
                for ( int i = 0; i < handCards.Count; i++)
                {
                    RowPlayerHand.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(110) });
                }
            }

            //defino las columnas
            var cardCount = 0;
            var rowCount = 0;
            foreach (var card in handCards)
            {
                var cardImage = card.Image;
                cardImage.Margin = new Thickness(5);
                Grid.SetColumn(cardImage, cardCount);
                Grid.SetRow(cardImage, rowCount);

                if (cardImage.Parent == null)
                {
                    RowPlayerHand.Children.Add(cardImage);
                    cardImage.MouseEnter += (s, e) => { CardImage_MouseEnter(card.Number); };
                    cardImage.MouseLeave += CardImage_MouseLeave;
                    cardImage.MouseLeftButtonUp += (s, e) => { HandCard_Click(cardImage,card); };
                }

                if(cardCount < maxColums)
                {
                    cardCount++;
                }
                else
                {
                    cardCount = 0;
                    rowCount ++;
                }
            }
        }

        private void HandCard_Click(Canvas cardImage, Card card)
        {
            var availablePit = CardCanBePlayed(card.Number);
            if (availablePit >= 0)
            {
                gameInstance.ActualRound.PlayerCardsObj.Remove(card);
                gameInstance.ActualRound.CardPitsObj[availablePit].Add(card);

                DrawPlayerHand();
                GenerateDeckAndPits();
            }
        }

        private void CardImage_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Mouse.OverrideCursor = null;
        }

        private void CardImage_MouseEnter(int number)
        {
            if(CardCanBePlayed(number) >= 0)
            {
                Mouse.OverrideCursor = Cursors.Hand;
            }

        }

        private int CardCanBePlayed(int cardNumber)
        {
            int nextCard = cardNumber + 1;
            int previousCard = cardNumber - 1;

            if(nextCard > 12) { nextCard = 0;}
            if(previousCard < 0) { previousCard = 12; }


            var pitWhereCardCanBePlayed = gameInstance.ActualRound.CardPitsObj.Where(p => p.Value.Last().Number == nextCard || p.Value.Last().Number == previousCard).ToList();
            if(pitWhereCardCanBePlayed.Count > 0)
            {
                return pitWhereCardCanBePlayed.FirstOrDefault().Key;
            }

            return -1;
        }

        private void GetCardsFromDeck()
        {
            if(gameInstance.ActualRound.PlayerTurnId == player.UserId)
            {
                //if(turnStarted == false)
                //{
                    if (gameInstance.ActualRound.AvailableCardsObj.Count == 0)
                    {
                        var cardColor = Card.GetDisabledCard();
                        deckImg.Children.Add(cardColor);
                        deckImg.IsEnabled = false;
                    }
                    else
                    {
                        var cards = gameInstance.ActualRound.AvailableCardsObj.Take(2).ToList();

                        gameInstance.ActualRound.PlayerCardsObj.AddRange(cards);

                        foreach (var card in cards)
                        {
                            gameInstance.ActualRound.AvailableCardsObj.Remove(card);
                        }

                        DrawPlayerHand();
                    }
                    turnStarted = true;
                //}
                //else
                //{
                //    var dialogExample = new MaterialDialog() { Title = "test", Message = "Ya has juntado las 2 cartas de tu turno." };
                //    GrdPrincipal.Children.Add(dialogExample);
                //}

                

            }
        }

        private void Game_Loaded(object sender, RoutedEventArgs e)
        {
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
