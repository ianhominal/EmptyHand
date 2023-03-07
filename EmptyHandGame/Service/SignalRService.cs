using Domain.Interfaces;
using Domain.Models;
using Google.Apis.PeopleService.v1.Data;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Numerics;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;


namespace Service
{
    public class SignalRService
    {
        private readonly HubConnection _connection;
        private static IGameUpdater gameUpdater;
        private static IMenuUpdater menuUpdater;

        public SignalRService(string hubName)
        {
            // _menuUpdater = menuUpdater;

#if !RELEASE
            _connection = new HubConnectionBuilder()
                .WithUrl($"https://signalrtest20230303170401.azurewebsites.net/{hubName}")
                .Build();
#else
            _connection = new HubConnectionBuilder()
                            .WithUrl($"https://localhost:44331/{hubName}")
                            .WithAutomaticReconnect(new[] { TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10) })
                            .Build();
#endif
        }


        public void SetMenuUpdater(IMenuUpdater _menuUpdater)
        {
            menuUpdater = _menuUpdater;

            _connection.On<string>("UpdateOnlineGames", (createdGamesJson) =>
            {
                var createdGames = JsonConvert.DeserializeObject<List<GameModel>>(createdGamesJson);

                menuUpdater.RefreshGameList(createdGames);
            });


            _connection.On<string>("GameCreatedByUser", (createdGameJson) =>
            {
                var createdGame = JsonConvert.DeserializeObject<GameModel>(createdGameJson);
                menuUpdater.CreateNewGame(createdGame);
            });


            _connection.On<string>("StartGame", (createdGameJson) =>
            {
                var createdGame = JsonConvert.DeserializeObject<GameModel>(createdGameJson);
                menuUpdater.StartNewGame(createdGame);
            });



        }


        public void SetGameUpdater(IGameUpdater _gameUpdater)
        {
            gameUpdater = _gameUpdater;

            _connection.On<string>("UpdateGameState", (gameStateJson) =>
            {
                var gameState = JsonConvert.DeserializeObject<GameModel>(gameStateJson);
                gameUpdater.UpdateGame(gameState);
            });

            _connection.On<bool>("ForceEndTurn", (endTurn) =>
            {
                gameUpdater.ForceEndTurn();
            });

            _connection.On<string>("CloseGame", (enemyPlayer) =>
            {
                gameUpdater.CloseGame();
                menuUpdater.GameClosed(enemyPlayer);
            });
        }

        public async Task Conectar()
        {
            if (_connection.State == HubConnectionState.Disconnected)
            {
                await _connection.StartAsync();
            }
        }

        public async Task Authenticate(string token)
        {
            await _connection.InvokeAsync("Authenticate", token);
        }

        public async Task CreateGame(PlayerModel player)
        {
            // var cts = new CancellationTokenSource();
            await _connection.InvokeAsync("CreateNewGame", JsonConvert.SerializeObject(player));//, cts.Token);
        }

        public async Task JoinGame(PlayerModel player2, GameModel gameToStart)
        {
            await _connection.InvokeAsync("JoinGame", JsonConvert.SerializeObject(player2), JsonConvert.SerializeObject(gameToStart));
        }

        public async Task RegisterGameGroup(GameModel gameState)
        {
            await _connection.InvokeAsync("RegisterGameGroup", JsonConvert.SerializeObject(gameState));
        }

        public async Task UpdateGameState(GameModel gameState)
        {

            await _connection.InvokeAsync("UpdateGameState", JsonConvert.SerializeObject(gameState));
        }


        public async Task CheckIfPlayerCanPlay(GameModel gameState)
        {

            await _connection.InvokeAsync("CheckIfPlayerCanPlay", JsonConvert.SerializeObject(gameState));
        }



        public async Task EndTurn(GameModel gameState)
        {
            await _connection.InvokeAsync("EndTurn", JsonConvert.SerializeObject(gameState));
        }

        public async Task CancelCreateGame(Guid guid)
        {

            await _connection.InvokeAsync("CancelCreateGame", guid);
        }


        public async Task CloseGame(GameModel gameState)
        {
            await _connection.InvokeAsync("CloseGame", JsonConvert.SerializeObject(gameState));
        }
    }

}
