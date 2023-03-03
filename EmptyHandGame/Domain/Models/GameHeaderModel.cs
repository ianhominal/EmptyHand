using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataService;
using static Domain.Models.GameRoundModel;

namespace Domain.Models
{
    public partial class GameHeaderModel
    {
        public GameHeaderModel(GameHeader header, string userId)
        {
            GameId = header.GameId;
            RoundsCount = header.RoundsCount;
            GameRoundId = header.GameRoundId;

            if (header.GameRound != null)
            {
                ActualGameRound = new GameRoundModel(header.GameRound,header.PlayerId,header.Player2Id, header.PlayerRoundsWins, header.Player2RoundsWins, header.PlayerPoints, header.Player2Points,header.PlayerName, header.Player2Name, header.PlayerPhoto, header.Player2Photo);
            }
        }

        public GameHeaderModel() { }

        public Guid GameId { get; set; }
        public int RoundsCount { get; set; }
        public Guid? GameRoundId { get; set; }

        public GameRoundModel? ActualGameRound { get; set; }

    }
}
