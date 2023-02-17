using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class GameHeader
    {
        public string GameId { get; set; }
        public string PlayerId { get; set; }
        public int PlayerPoints { get; set; }
        public int PlayerRoundsWins { get; set; }
        public string Player2Id { get; set; }
        public int Player2Points { get; set; }
        public int Player2RoundsWins { get; set; }
        public int RoundsCount { get; set; }
        public string ActualRoundId { get; set; }
        public GameRound ActualRound { get; set; }  
    }
}
