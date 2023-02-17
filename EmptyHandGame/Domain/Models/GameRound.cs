using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class GameRound
    {
        public string GameRoundId { get; set; }
        public string AvailableCards { get; set; }
        public string PlayerCards { get; set; }
        public string PlayerLifeCards { get; set; }
        public string Player2Cards { get; set; }
        public string Player2LifeCards { get; set; }
        public int PlayerTurnId { get; set; }
        public string CardPits { get; set; }


        public List<Card> AvailableCardsObj { get; set; }
        public List<Card> PlayerCardsObj { get; set; }
        public List<Card> PlayerLifeCardsObj { get; set; }
        public List<Card> Player2CardsObj { get; set; }
        public List<Card> Player2LifeCardsObj { get; set; }
        public Dictionary<int, List<Card>> CardPitsObj { get; set; }

    }
}
