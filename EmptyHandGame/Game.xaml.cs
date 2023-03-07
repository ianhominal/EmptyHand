using Domain.Interfaces;
using Domain.Models;
using Service;
using System;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Card = Domain.Models.Card;

namespace EmptyHandGame
{
    /// <summary>
    /// Lógica de interacción para Game.xaml
    /// </summary>
    public partial class Game : Window, IGameUpdater
    {
        GameModel gameState;

        string userId;
        Canvas deckImg;
        bool turnStarted;
        bool playerPlayed;

        PlayerModel actualPlayerInfo;
        PlayerModel enemyPlayerInfo;

        private SignalRService signalRClient;

        public Game(GameModel _gameState, string _userId)
        {
            InitializeComponent();

            userId = _userId;

            turnStarted = false;
            gameState = _gameState;
            playerPlayed = false;
            //UpdateGame(_gameState);
            turnSoundPlayed = false;

        }

        public void UpdateGame(GameModel newGameState)
        {
            Dispatcher.Invoke(() =>
            {
                gameState = newGameState;
                DataContext = gameState;

                actualPlayerInfo = gameState.Player1.PlayerId == userId ? gameState.Player1 : gameState.Player2;
                enemyPlayerInfo = gameState.Player1.PlayerId == userId ? gameState.Player2 : gameState.Player1;


                DrawPlayersInfo();
                DrawPlayerHand();
                DrawPlayerLifeCards();
                GenerateDeckAndPits();
                DrawEnemyHand();
                DrawEnemyLifeCards();


                btnEndTurn.IsEnabled = playerPlayed;


                GrdWaiting.Visibility = Visibility.Collapsed;

                PlayTurnSound();


            });
        }



        bool turnSoundPlayed;
        private void PlayTurnSound()
        {
            if(actualPlayerInfo.PlayerId ==  gameState.PlayerTurnId && !turnStarted && !turnSoundPlayed)
            {
                turnSoundPlayed = true;

                SoundPlayer player = new SoundPlayer();
                player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\Sound\\Player-Turn.wav";
                player.Play();
            }
        }

        private void DrawPlayersInfo()
        {
            txtPlayerName.Text = actualPlayerInfo.PlayerName;
            txtPlayerPoints.Text = $"{actualPlayerInfo.PlayerPoints}";
            // txtPlayerRounds.Text = $"{actualPlayerInfo.PlayerRoundsWins}";


            txtEnemyName.Text = enemyPlayerInfo.PlayerName;
            txtEnemyPoints.Text = $"{enemyPlayerInfo.PlayerPoints}";
            // txtEnemyRounds.Text = $"{enemyPlayerInfo.PlayerRoundsWins}";

            txtTurnoActual.Text = gameState.PlayerTurnId == userId ? "Es tu turno" : "Turno del otro jugador";

            var playerPhoto = actualPlayerInfo.PlayerPhoto;
            var enemyPhoto = enemyPlayerInfo.PlayerPhoto;


            if (!string.IsNullOrEmpty(playerPhoto))
            {
                Ellipse playerImg = PlayerModel.GetUserPhoto(playerPhoto);
                Ellipse playerBg = PlayerModel.GetBackgroundUserEllipse();


                imgPlayer.Children.Clear();
                // Agrega el objeto "Ellipse" con la imagen y borde al objeto "Canvas".
                imgPlayer.Children.Add(playerBg);
                imgPlayer.Children.Add(playerImg);

                imgPlayer.UpdateLayout();
            }

            if (!string.IsNullOrEmpty(enemyPhoto))
            {
                Ellipse enemyImg = PlayerModel.GetUserPhoto(enemyPhoto);
                Ellipse enemyBg = PlayerModel.GetBackgroundUserEllipse();


                imgEnemy.Children.Clear();
                // Agrega el objeto "Ellipse" con la imagen y borde al objeto "Canvas".
                imgEnemy.Children.Add(enemyBg);
                imgEnemy.Children.Add(enemyImg);

                imgEnemy.UpdateLayout();
            }
        }



        private void GenerateDeckAndPits()
        {
            RowDeck.Children.Clear();

            var deckGrid = new Grid();

            deckGrid.HorizontalAlignment = HorizontalAlignment.Center;
            deckGrid.VerticalAlignment = VerticalAlignment.Center;

            deckGrid.ColumnDefinitions.Add(new ColumnDefinition());

            deckImg = Card.CreateCardImage("Diamonds", "A", false, gameState.AvailableCards.Count > 0);
            deckImg.Margin = new Thickness(5, 5, 20, 5);

            deckImg.MouseLeftButtonUp += async (s, e) => { await GetCardsFromDeck(); };
            deckImg.MouseEnter += (s, e) => { DeckCard_MouseEnter(deckImg); };
            deckImg.MouseLeave += (s, e) => { DeckCard_MouseLeave(deckImg); };

            Grid.SetColumn(deckImg, 0);

            deckGrid.Children.Add(deckImg);


            var cardCount = 1;

            foreach (var pit in gameState.CardPits)
            {
                deckGrid.ColumnDefinitions.Add(new ColumnDefinition());

                var lastPitCard = pit.Value.Last();
                lastPitCard.CanBePlayed = false;

                var cardImage = Card.CreateCardImage(lastPitCard.Suit, lastPitCard.Rank); ;
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

        private void DeckCard_MouseLeave( Canvas card)
        {
            bool canBePlayed = gameState.PlayerTurnId == userId && turnStarted == false;
            Mouse.OverrideCursor = null;

            if (canBePlayed)
            {
                card.Margin = new Thickness(5, 5, 20, 5);
            }
        }

        private void DeckCard_MouseEnter( Canvas card)
        {
            bool canBePlayed = gameState.PlayerTurnId == userId && turnStarted == false;

            if (!canBePlayed)
            {
                Mouse.OverrideCursor = Cursors.No;
            }
            else
            {
                Mouse.OverrideCursor = Cursors.Hand;

                card.Margin = new Thickness(5, 5, 20, 15);
            }

        }


        private void DrawEnemyHand()
        {
            RowEnemyHand.Children.RemoveRange(0, RowEnemyHand.Children.Count);
            var handGrid = new Grid();

            handGrid.HorizontalAlignment = HorizontalAlignment.Center;
            handGrid.VerticalAlignment = VerticalAlignment.Bottom;

            var handCards = enemyPlayerInfo.PlayerCards;


            //defino las columnas
            var cardCount = 0;
            foreach (var card in handCards)
            {
                card.CanBePlayed = false;
                handGrid.ColumnDefinitions.Add(new ColumnDefinition());

                var cardImage = Card.CreateCardImage("Diamonds", "A", false, gameState.AvailableCards.Count > 0);
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

            var lifeCards = enemyPlayerInfo.PlayerLifeCards;

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

                var cardImage = Card.CreateCardImage("Diamonds", "A", false, gameState.AvailableCards.Count > 0);

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

            var lifeCards = actualPlayerInfo.PlayerLifeCards;

            var cardCount = 0;
            foreach (var card in lifeCards)
            {
                playerLifeGrid.ColumnDefinitions.Add(new ColumnDefinition());

                card.CanBePlayed = true;

                var cardImage = Card.CreateCardImage("Diamonds", "A", false, gameState.AvailableCards.Count > 0); ;
                cardImage.Margin = new Thickness(5);
                Grid.SetColumn(cardImage, cardCount);

                cardImage.MouseEnter += (s, e) => { LifeCard_MouseEnter(card.Number, cardImage); };
                cardImage.MouseLeave += (s, e) => { LifeCard_MouseLeave(card.Number, cardImage); };
                cardImage.MouseLeftButtonUp += async (s, e) => { await LifeCard_Click(cardImage, card); };

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
            turnStarted = false;
            playerPlayed = false;
            turnSoundPlayed = false;
            await signalRClient.EndTurn(gameState);
        }

        private void DrawPlayerHand()
        {
            //if(playerHandGrid == null) playerHandGrid = new Grid();

            RowPlayerHand.Children.RemoveRange(0, RowPlayerHand.Children.Count);

            RowPlayerHand.HorizontalAlignment = HorizontalAlignment.Center;
            RowPlayerHand.VerticalAlignment = VerticalAlignment.Bottom;

            var maxColums = 20;

            var handCards = actualPlayerInfo.PlayerCards;

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

                var cardImage = Card.CreateCardImage(card.Suit, card.Rank);
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
                    cardImage.MouseEnter += (s, e) => { CardImage_MouseEnter(card.Number, cardImage); };
                    cardImage.MouseLeave += (s, e) => { CardImage_MouseLeave(card.Number, cardImage); };
                    cardImage.MouseLeftButtonUp += async (s, e) => { await HandCard_Click(cardImage, card); };
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

        private async Task HandCard_Click(Canvas cardImage, Card card)
        {
            var availablePit = CardCanBePlayed(card.Number);
            if (availablePit >= 0 && turnStarted && card.CanBePlayed)
            {
                card.CanBePlayed = false;
                actualPlayerInfo.PlayerCards.Remove(card);
                gameState.CardPits[availablePit].Add(card);

                cardImage.MouseEnter -= (s, e) => { CardImage_MouseEnter(card.Number, cardImage); };
                cardImage.MouseLeave -= (s, e) => { CardImage_MouseLeave(card.Number, cardImage); };
                cardImage.MouseLeftButtonUp -= async (s, e) => { await HandCard_Click(cardImage, card); };

                //DrawPlayerHand();
                // GenerateDeckAndPits();

                playerPlayed = true;
                await UpdateGameState();
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

        private async Task LifeCard_Click(Canvas cardImage, Card card)
        {
            if (turnStarted && card.CanBePlayed)
            {
                card.CanBePlayed = false;
                actualPlayerInfo.PlayerLifeCards.Remove(card);
                gameState.CardPits.Add(gameState.CardPits.Count, new System.Collections.Generic.List<Card>() { card });

                cardImage.MouseEnter -= (s, e) => { CardImage_MouseEnter(card.Number, cardImage); };
                cardImage.MouseLeave -= (s, e) => { CardImage_MouseLeave(card.Number, cardImage); };
                cardImage.MouseLeftButtonUp -= async (s, e) => { await LifeCard_Click(cardImage, card); };

                //DrawPlayerLifeCards();
                //GenerateDeckAndPits();

                playerPlayed = true;
                await UpdateGameState();
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

        private int CardCanBePlayed(int cardNumber) //todo si hay mas de uno deberías poder elegir en cual jugarlo
        {
            int nextCard = cardNumber + 1;
            int previousCard = cardNumber - 1;

            if (nextCard > 12) { nextCard = 0; }
            if (previousCard < 0) { previousCard = 12; }

            var pitWhereCardCanBePlayed = gameState.CardPits.Where(p => p.Value.Last().Number == nextCard || p.Value.Last().Number == previousCard).ToList();
            if (pitWhereCardCanBePlayed.Count > 0)
            {
                return pitWhereCardCanBePlayed.FirstOrDefault().Key;
            }

            return -1;
        }


        MaterialDialog dialogExample;
        private async Task GetCardsFromDeck()
        {
            if (gameState.PlayerTurnId == userId)
            {
                if (turnStarted == false)
                {
                    if (gameState.AvailableCards.Count == 0)
                    {
                        var cardColor = Card.GetDisabledCard();
                        deckImg.Children.Add(cardColor);
                        deckImg.IsEnabled = false;
                    }
                    else
                    {
                        var cards = gameState.AvailableCards.Take(2).ToList();

                        actualPlayerInfo.PlayerCards.AddRange(cards);

                        foreach (var card in cards)
                        {
                            gameState.AvailableCards.Remove(card);
                        }

                        //DrawPlayerHand();
                        CheckIfPlayerCanPlay();
                    }

                    turnStarted = true;
                    await UpdateGameState();
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
                    if (gameState.PlayerTurnId == actualPlayerInfo.PlayerId)
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


        //private async void Window_Loaded(object sender, RoutedEventArgs e)
        //{

        //    await signalRClient.Conectar();

        //}


        private async Task UpdateGameState()
        {
            await signalRClient.UpdateGameState(gameState);
        }

        private async Task CheckIfPlayerCanPlay()
        {
            await signalRClient.CheckIfPlayerCanPlay(gameState);
        }


       
        


        private async void GameWindow_Loaded(object sender, RoutedEventArgs e)
        {
            signalRClient = new SignalRService("GameHub");
            signalRClient.SetGameUpdater(this);

            await signalRClient.Conectar();

            if(gameState.Player1.PlayerId == userId)
            {
                gameState.Player1.PlayerHubId = userId;
            }
            else
            {
                gameState.Player2.PlayerHubId = userId;
            }

            await signalRClient.RegisterGameGroup(gameState);

            await signalRClient.UpdateGameState(gameState);
        }

        public void ForceEndTurn()
        {
            playerPlayed = false;
            btnEndTurn.IsEnabled = false;
        }

        public void CloseGame()
        {
            Dispatcher.Invoke(async () =>
            {
                this.Close();
            });
        }

        private async void Window_Closed(object sender, EventArgs e)
        {
            await signalRClient.CloseGame(gameState);
        }
    }
}
