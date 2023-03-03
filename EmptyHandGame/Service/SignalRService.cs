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

        public SignalRService(IGameUpdater gameUpdater)
        {
            //var handler = new HttpClientHandler();
            //handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            //var httpClient = new HttpClient(handler);

            //_connection = new HubConnectionBuilder()
            //    .WithUrl("https://localhost:44331/GameHub", options =>
            //    {
            //        options.HttpMessageHandlerFactory = _ => handler;
            //    }).Build();

            _connection = new HubConnectionBuilder()
                .WithUrl("https://signalrtest20230303170401.azurewebsites.net/GameHub")
                .Build();

            _gameUpdater = gameUpdater;


            // Define un método para manejar el evento "UpdateGameState"
            _connection.On<string>("UpdateGameState", gameGuid =>
            {
                //Context.RefreshGameData(gameGuid);

                _gameUpdater.UpdateGame();
            });

        }

        public async Task Conectar()
        {

            if (_connection.State == HubConnectionState.Disconnected)
            {
                await _connection.StartAsync();
            }
        }

        public async Task EndTurn(string gameGuid)
        {
            if(_connection.State != HubConnectionState.Connected)
            {
                await Conectar();
            }
            await _connection.InvokeAsync("EndTurn", gameGuid);
        }
    }

}
