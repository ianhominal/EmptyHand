using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataService;

namespace Domain.Models
{
    public partial class GameHeaderModel
    {
        public GameHeaderModel(GameHeader header)
        {
            GameHeader = header;

            if(header.GameRound != null)
            {
                ActualGameRound = new GameRoundModel(header.GameRound);
            }
            
        }

        public GameHeader GameHeader { get; set; }
        public GameRoundModel? ActualGameRound { get; set; }

    }
}
