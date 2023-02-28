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
            GameHeader = header;

            if(header.GameRound != null)
            {
                ActualGameRound = new GameRoundModel(header.GameRound, header.PlayerId == userId ? ActualPlayerEnum.Player : ActualPlayerEnum.Player2);
            }
            
        }

        public GameHeader GameHeader { get; set; }
        public GameRoundModel? ActualGameRound { get; set; }

    }
}
