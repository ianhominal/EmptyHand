using DataService;
using Domain.Models;
using Google.Apis.PeopleService.v1.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using static Domain.Models.GameRoundModel;

namespace Service
{
    public class GameService
    {
        public static GameHeaderModel? GetGameState(string gameGuid, string userId, Person user, Context db)
        {
            var gameHeader = db.GetGameHeader(gameGuid);

            if (gameHeader == null) return null;

            var gameState = ToModel(gameHeader, userId, user, db);

            return gameState;
        }

        public static GameHeaderModel ToModel(GameHeader gameHeader, string userId, Person user, Context db)
        {

            var gameState = new GameHeaderModel(gameHeader, userId);
            //si la partida tiene player 1 y 2
            if (gameState.GameHeader.Player2Id != null)
            {
                //partida pertenece a otros players
                if (gameState.GameHeader.Player2Id != userId && gameState.GameHeader.PlayerId != userId) return null;
            }
            else if (gameState.GameHeader.PlayerId != userId)
            {
                //si esta disponible el player 2  y no es el mismo que el player 1 entonces lo asigna
                gameState.GameHeader.Player2Id = userId;

                gameState.GameHeader.Player2Name = user.Names.FirstOrDefault()?.DisplayName;
                gameState.GameHeader.Player2Photo = user.Photos.FirstOrDefault()?.Url;
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
                    gameState.ActualGameRound = new GameRoundModel(newRound, gameState.GameHeader.PlayerId == userId ? ActualPlayerEnum.Player : ActualPlayerEnum.Player2);
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

        public static void EndTurn(GameHeaderModel gameState, Context db)
        {
            gameState.GameHeader.GameRound.CardPits = string.Join('|', gameState.ActualGameRound.CardPitsObj.Select(p => Card.ToStringList(p.Value)));
            gameState.GameHeader.GameRound.AvailableCards = Card.ToStringList(gameState.ActualGameRound.AvailableCardsObj);

            switch (gameState.ActualGameRound.ActualPlayer)
            {
                case ActualPlayerEnum.Player:

                    gameState.GameHeader.GameRound.PlayerCards = Card.ToStringList(gameState.ActualGameRound.PlayerCardsObj);
                    gameState.GameHeader.GameRound.PlayerLifeCards = Card.ToStringList(gameState.ActualGameRound.PlayerLifeCardsObj);
                    gameState.GameHeader.GameRound.PlayerTurnId = gameState.GameHeader.Player2Id;
                    break;
                case ActualPlayerEnum.Player2:

                    gameState.GameHeader.GameRound.Player2Cards = Card.ToStringList(gameState.ActualGameRound.PlayerCardsObj);
                    gameState.GameHeader.GameRound.Player2LifeCards = Card.ToStringList(gameState.ActualGameRound.PlayerLifeCardsObj);
                    gameState.GameHeader.GameRound.PlayerTurnId = gameState.GameHeader.PlayerId;
                    break;
            }

            db.SaveChanges();

        }

        public static GameHeader CreateNewGame(string userId, Person user, DataService.Context db)
        {
            GameHeader gH = new GameHeader();
            gH.GameId = Guid.NewGuid();
            gH.PlayerId = userId;
            gH.PlayerRoundsWins = 0;
            gH.Player2RoundsWins = 0;
            gH.RoundsCount = 0;
            gH.PlayerPoints = 0;
            gH.Player2Points = 0;
            gH.PlayerName = user.Names.FirstOrDefault()?.DisplayName;
            gH.PlayerPhoto = user.Photos.FirstOrDefault()?.Url;

            db.AddGameHeader(gH);

            return gH;
        }



    }
}
