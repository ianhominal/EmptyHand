using DataService;
using Domain.Interfaces;
using Domain.Models;
using Google.Apis.PeopleService.v1.Data;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace Service
{
    public class SignalRService
    {

        private readonly HubConnection _connection;
        private readonly IGameUpdater _gameUpdater;
        private readonly string _userId;

        public SignalRService(IGameUpdater gameUpdater, string userId)
        {
            _userId = userId;

            _connection = new HubConnectionBuilder()
                .WithUrl("https://signalrtest20230303170401.azurewebsites.net/GameHub")
                .Build();

            _gameUpdater = gameUpdater;


            // Define un método para manejar el evento "UpdateGameState"
            _connection.On<string,GameHeaderModel>("UpdateGameState", (gameGuid,gameState) =>
            {
                //actualizo el game si pertenece al player de la instancia
                if(_userId == gameState.ActualGameRound?.Player1?.PlayerId || _userId == gameState.ActualGameRound?.Player2?.PlayerId)
                {
                    _gameUpdater.UpdateGame(gameState);
                }
            });
        }

        public async Task Conectar()
        {

            if (_connection.State == HubConnectionState.Disconnected)
            {
                await _connection.StartAsync();
            }
        }

        public async Task EndTurn(GameHeaderModel gameState)
        {
            if(_connection.State != HubConnectionState.Connected)
            {
                await Conectar();
            }
            await _connection.InvokeAsync("EndTurn", gameState);
        }

        public async Task CreateGame(PlayerModel player)
        {
            if (_connection.State != HubConnectionState.Connected)
            {
                await Conectar();
            }
            await _connection.InvokeAsync("GreateGame", player);
        }


        public async Task JoinGame(PlayerModel player, GameHeaderModel gameHeader)
        {
            if (_connection.State != HubConnectionState.Connected)
            {
                await Conectar();
            }
            await _connection.InvokeAsync("GreateGame", player, gameHeader);
        }
    }

}
