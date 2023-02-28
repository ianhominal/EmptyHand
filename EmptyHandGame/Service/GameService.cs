using DataService;
using Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;

namespace Service
{
    public class GameService
    {
        public static GameHeaderModel? GetGameState(string gameGuid, string userId, Context db)
        {
            var gameHeader = db.GetGameHeader(gameGuid);

            if (gameHeader == null) return null;

            var gameState = new GameHeaderModel(gameHeader);

            //si la partida tiene player 1 y 2
            if(gameState.GameHeader.Player2Id != null)
            {
                //partida pertenece a otros players
                if (gameState.GameHeader.Player2Id != userId && gameState.GameHeader.PlayerId != userId) return null;
            }
            else if(gameState.GameHeader.PlayerId != userId)
            {
                //si esta disponible el player 2  y no es el mismo que el player 1 entonces lo asigna
                gameState.GameHeader.Player2Id = userId;
                //guardar este cambio
                db.SaveChanges();
            }

            //si esta creado el juego pero aun no tiene un GameRound
            if (gameState.GameHeader.GameRound == null)
            {
                //si no tiene pero tiene player2ID entonces debería empezar un nuevo game
                if (gameState.GameHeader.Player2Id != null)
                {
                    //obtengo las cartas para la ronda
                    var newRound = GetNewGameCards();

                    //agrego al entity header los datos de la ronda creada
                    gameState.GameHeader.GameRoundId = newRound.GameRoundId;
                    gameState.GameHeader.GameRound = newRound;

                    //decido al azar que player empieza la partida
                    var playerTurnId = new Random().Next(1, 2) == 1 ? gameState.GameHeader.PlayerId : gameState.GameHeader.Player2Id;
                    newRound.PlayerTurnId = playerTurnId;

                    //obtnego el modelo que contendra los objetos de las cartas
                    gameState.GameHeader.GameRound = newRound;
                    gameState.ActualGameRound = new GameRoundModel(newRound);
                    //guardar este cambio
                    db.SaveChanges();
                }
                else
                {
                    //esperando que el player2 acepte
                    return gameState;
                }
            }


            return gameState;
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



        public static GameHeader CreateNewGame(string userId, DataService.Context db)
        {
            GameHeader gH = new GameHeader();
            gH.GameId = Guid.NewGuid();
            gH.PlayerId = userId;
            gH.PlayerRoundsWins = 0;
            gH.Player2RoundsWins = 0;
            gH.RoundsCount = 0;
            gH.PlayerPoints = 0;
            gH.Player2Points = 0;

            db.AddGameHeader(gH);

            return gH;
        }

    }
}
