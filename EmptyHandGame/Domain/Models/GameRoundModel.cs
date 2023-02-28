using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataService;

namespace Domain.Models
{
    public partial class GameRoundModel
    {
        public enum ActualPlayerEnum
        {
            Player,
            Player2
        }

        public GameRoundModel(GameRound gameRound, ActualPlayerEnum actualPlayer)
        {
            GameRound = gameRound;

            //determino si esta jugando el player 1 o el 2
            ActualPlayer = actualPlayer;
            //creo los objetos tipo Card
            switch (actualPlayer)
            {
                case ActualPlayerEnum.Player:
                    PlayerCardsObj = Card.GetCards(gameRound.PlayerCards);
                    Player2CardsObj = Card.GetCards(gameRound.Player2Cards, false);
                    PlayerLifeCardsObj = Card.GetCards(gameRound.PlayerLifeCards, true);
                    Player2LifeCardsObj = Card.GetCards(gameRound.Player2LifeCards, false);
                    break;
                case ActualPlayerEnum.Player2:
                    PlayerCardsObj = Card.GetCards(gameRound.Player2Cards);
                    Player2CardsObj = Card.GetCards(gameRound.PlayerCards, false);
                    PlayerLifeCardsObj = Card.GetCards(gameRound.Player2LifeCards, true);
                    Player2LifeCardsObj = Card.GetCards(gameRound.PlayerLifeCards, false);
                    break;
            }

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

        public GameRound GameRound { get; set; }

        public ActualPlayerEnum ActualPlayer;
        public List<Card> AvailableCardsObj { get; set; }
        public List<Card> PlayerCardsObj { get; set; }
        public List<Card> PlayerLifeCardsObj { get; set; }
        public List<Card> Player2CardsObj { get; set; }
        public List<Card> Player2LifeCardsObj { get; set; }
        public Dictionary<int, List<Card>> CardPitsObj { get; set; }
    }
}
