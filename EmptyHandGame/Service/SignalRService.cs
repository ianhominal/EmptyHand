using DataService;
using Domain.Interfaces;
using Google.Apis.PeopleService.v1.Data;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace Service
{
    public class SignalRService
    {

        private readonly HubConnection _connection;
        private readonly IGameUpdater _gameUpdater;

        public SignalRService(Context db, IGameUpdater gameUpdater)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:44331/GameHub")
                .Build();

            _gameUpdater = gameUpdater;


            // Define un método para manejar el evento "UpdateGameState"
            _connection.On<string>("UpdateGameState", gameGuid =>
            {
                GameHeader headerToRefresh = db.GetGameHeader(gameGuid);
                GameRound gameRoundToRefresh = headerToRefresh.GameRound;
                var entity = db.GetContext().Entry(headerToRefresh);
                var entityRound = db.GetContext().Entry(gameRoundToRefresh);
                entity.Reload();
                entityRound.Reload();
                _gameUpdater.UpdateGame();
            });

        }

        public async Task Conectar()
        {
            if(_connection.State == HubConnectionState.Disconnected)
            {
                await _connection.StartAsync();
            }
        }

        public async Task EndTurn(string gameGuid)
        {
            await _connection.InvokeAsync("EndTurn", gameGuid);
        }
    }

}
