using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IMenuUpdater
    {
        void RefreshGameList(List<GameModel> games);
        void CreateNewGame(GameModel game);

        void StartNewGame(GameModel game);

        void GameClosed(string enemyPlayer);
    }
}
