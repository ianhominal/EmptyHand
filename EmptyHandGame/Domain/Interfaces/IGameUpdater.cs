using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IGameUpdater
    {
        void UpdateGame(GameModel gameState);
        void ForceEndTurn();

        void CloseGame();
    }
}
