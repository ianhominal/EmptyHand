using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using DataService;

namespace Domain.Models
{
    public partial class GameRoundModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public GameRoundModel(GameRound gameRound, string player1Id, string player2Id, int player1RoundsWin, int player2RoundsWin, int player1Points, int player2Points, string player1Name, string player2Name, string player1PhotoURL, string player2PhotoURL)
        {
            GameRoundId = gameRound.GameRoundId;
            PlayerTurnId = gameRound.PlayerTurnId;

            TurnStarted = false;

            Player1Cards = new PlayerCardsModel(gameRound.PlayerCards, gameRound.PlayerLifeCards, player1Id, player1Points, player1RoundsWin, player1Name, player1PhotoURL);
            Player2Cards = new PlayerCardsModel(gameRound.Player2Cards, gameRound.Player2LifeCards, player2Id, player2Points, player2RoundsWin, player2Name, player2PhotoURL);

            AvailableCardsObj = Card.GetCards(gameRound.AvailableCards);

            var pitsSplit = gameRound.CardPits.Split('|');
            var pitCount = 0;

            CardPitsObj = new Dictionary<int, List<Card>>();
            foreach (var pit in pitsSplit)
            {
                CardPitsObj[pitCount] = Card.GetCards(pit);
                pitCount++;
            }
        }

        public Guid GameRoundId { get; set; }
        public PlayerCardsModel Player1Cards { get; set; }
        public PlayerCardsModel Player2Cards { get; set; }

        public string PlayerTurnId { get; set; }

        public List<Card> AvailableCardsObj { get; set; }
        public Dictionary<int, List<Card>> CardPitsObj { get; set; }

        private bool _turnStarted;
        public bool TurnStarted
        {
            get { return _turnStarted; }
            set
            {
                if (_turnStarted != value)
                {
                    _turnStarted = value;
                    OnPropertyChanged(nameof(TurnStarted));
                }
            }
        }
    }
}