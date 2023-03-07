using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Domain.Models
{
    public partial class GameModel
    {
     
        public Guid GameId { get; set; }
        public PlayerModel Player1 { get; set; }
        public PlayerModel Player2 { get; set; }

        public string PlayerTurnId { get; set; }

        public string InitialPlayerTurnId { get; set; }

        public List<Card> AvailableCards { get; set; }
        public Dictionary<int, List<Card>> CardPits { get; set; }
    }
}