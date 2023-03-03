using DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class PlayerCardsModel
    {

        public PlayerCardsModel(string playerCards, string playerLifeCards, string playerId, int playerPoints, int playerRoundsWins, string playerName, string playerPhotoURL)
        {
            this.PlayerId = playerId;
            this.PlayerLifeCardsObj = Card.GetCards(playerLifeCards);
            this.PlayerCardsObj = Card.GetCards(playerCards);

            this.PlayerPoints = playerPoints;
            this.PlayerRoundsWins = playerRoundsWins;

            this.PlayerName = playerName;
            this.PlayerPhoto = playerPhotoURL;
        }

        public string PlayerId { get; set; }
        public List<Card> PlayerCardsObj { get; set; }
        public List<Card> PlayerLifeCardsObj { get; set; }
        public int PlayerPoints { get; set; }
        public int PlayerRoundsWins { get; set; }
        public string PlayerName { get; set; }
        public string PlayerPhoto { get; set; }
    }
}
