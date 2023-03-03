using DataService;
using Domain.Models;
using Google.Apis.PeopleService.v1.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Policy;
using static Domain.Models.GameRoundModel;

namespace Service
{
    public class GameService
    {
        public static GameHeaderModel? GetGameState(string gameGuid, string userId, Person user)
        {
            var gameHeader = Context.GetGameHeader(gameGuid);

            if (gameHeader == null) return null;

            var gameState = ToModel(gameHeader, userId, user);

            return gameState;
        }

        public static GameHeaderModel? ToModel(GameHeader gameHeader, string userId, Person user)
        {
            bool updateEntity = false;
            //si la partida tiene player 1 y 2 y el jugador actual esta en ella
            if (gameHeader.Player2Id != null && gameHeader.Player2Id != userId && gameHeader.PlayerId != userId) return null;

            //si la partida tiene 1 solo jugador y es quien intenta acceder no puede.
            if (gameHeader.GameRound == null && gameHeader.PlayerId == userId) return new GameHeaderModel(gameHeader, userId);

            else if (gameHeader.PlayerId != userId)
            {
                //si esta disponible el player 2  y no es el mismo que el player 1 entonces lo asigna
                gameHeader.Player2Id = userId;
                gameHeader.Player2Name = user.Names?.FirstOrDefault()?.DisplayName;
                gameHeader.Player2Photo = user.Photos?.FirstOrDefault()?.Url;

                //guardar este cambio que agrega la info del player 2 a la entity ya creada
                updateEntity = true;
            }

            //si esta creado el juego pero aun no tiene un GameRound
            if (gameHeader.GameRound == null)
            {
                //obtengo las cartas para la ronda
                gameHeader.GameRound = GetNewGameCards();

                //agrego al entity header los datos de la ronda creada
                gameHeader.GameRoundId = gameHeader.GameRound.GameRoundId;

                //decido al azar que player empieza la partida
                var playerTurnId = new Random().Next(1, 2) == 1 ? gameHeader.PlayerId : gameHeader.Player2Id;
                gameHeader.GameRound.PlayerTurnId = playerTurnId;

                //guardar este cambio que contiene un nuevo gameround.
                updateEntity = true;
            }

            if (updateEntity)
            { 
                Context.UpdateGameEntity(gameHeader); 
            }

            return new GameHeaderModel(gameHeader, userId); 
        }


        public static GameRound GetNewGameCards()
        {
            var deck = new List<Card>();

            //creo un string con el mazo entero
            List<string> deckStr = new List<string>();
            foreach (var cardSuit in Card.Suits)
            {
                foreach (var rank in Card.Ranks)
                {
                    deckStr.Add($"{cardSuit}_{rank}");
                }
            }

            deckStr = Card.RandomizeList(deckStr);

            string playerCards = string.Join(",", deckStr.Take(Card.STARTHANDCARDSCOUNT).ToList());
            foreach (var card in playerCards.Split(","))
            {
                deckStr.Remove(card);
            }

            string player2Cards = string.Join(",", deckStr.Take(Card.STARTHANDCARDSCOUNT).ToList());
            foreach (var card in player2Cards.Split(","))
            {
                deckStr.Remove(card);
            }

            string playerLifeCards = string.Join(",", deckStr.Take(Card.STARTLIFECARDSCOUNT).ToList());
            foreach (var card in playerLifeCards.Split(","))
            {
                deckStr.Remove(card);
            }

            string player2LifeCards = string.Join(",", deckStr.Take(Card.STARTLIFECARDSCOUNT).ToList());
            foreach (var card in player2LifeCards.Split(","))
            {
                deckStr.Remove(card);
            }

            string pit = deckStr.Take(1).First();
            deckStr.Remove(pit);

            GameRound gameRound = new GameRound()
            {
                GameRoundId = Guid.NewGuid(),
                PlayerCards = playerCards,
                Player2Cards = player2Cards,
                PlayerLifeCards = playerLifeCards,
                Player2LifeCards = player2LifeCards,
                CardPits = pit,
                AvailableCards = string.Join(",", deckStr.ToList()),
            };


            return gameRound;
        }

        public static void EndTurn(GameHeaderModel gameState)
        {
            var header = Context.GetGameHeader(gameState.GameId.ToString());

            header.GameRound.CardPits = string.Join('|', gameState.ActualGameRound.CardPitsObj.Select(p => Card.ToStringList(p.Value)));
            header.GameRound.AvailableCards = Card.ToStringList(gameState.ActualGameRound.AvailableCardsObj);

            header.GameRound.PlayerCards = Card.ToStringList(gameState.ActualGameRound.Player1Cards.PlayerCardsObj);
            header.GameRound.PlayerLifeCards = Card.ToStringList(gameState.ActualGameRound.Player1Cards.PlayerLifeCardsObj);
            header.GameRound.Player2Cards = Card.ToStringList(gameState.ActualGameRound.Player2Cards.PlayerCardsObj);
            header.GameRound.Player2LifeCards = Card.ToStringList(gameState.ActualGameRound.Player2Cards.PlayerLifeCardsObj);

            header.GameRound.PlayerTurnId = gameState.ActualGameRound.PlayerTurnId;

            Context.UpdateGameEntity(header);
        }

        public static GameHeader CreateNewGame(string userId, Person user)
        {
            GameHeader header = new GameHeader();
            header.GameId = Guid.NewGuid();
            header.PlayerId = userId;
            header.PlayerRoundsWins = 0;
            header.Player2RoundsWins = 0;
            header.RoundsCount = 0;
            header.PlayerPoints = 0;
            header.Player2Points = 0;
            header.PlayerName = user.Names.FirstOrDefault()?.DisplayName;
            header.PlayerPhoto = user.Photos.FirstOrDefault()?.Url;

            Context.AddGameHeader(header);

            return header;
        }
    }
}
