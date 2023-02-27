using Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Service
{
    public class GameService
    {
        public static GameHeader GetGameState(string gameGuid, int userId)
        {
            string jsonHeader = File.ReadAllText(@"C:\Repo\EmptyHand\EmptyHand\EmptyHandGame\Service\MockData\GameHeaderMock.json");
            string jsonRound = File.ReadAllText(@"C:\Repo\EmptyHand\EmptyHand\EmptyHandGame\Service\MockData\RoundInfoMock.json");

            var header = JsonConvert.DeserializeObject<GameHeader>(jsonHeader);
            GameRound round;
            if (!string.IsNullOrEmpty(header.ActualRoundId))
            {
                //si el header tiene un actual round la buscamos (partida ya iniciada)
                round = JsonConvert.DeserializeObject<GameRound>(jsonRound);
            }
            else if(header.Player2Id != null)
            {
                //si no tiene pero tiene player2ID entonces debería empezar un nuevo game
                round = Card.GetNewGameCards();
                round.GameRoundId = Guid.NewGuid().ToString();

                int playerTurn = new Random().Next(1,2);
                round.PlayerTurnId = playerTurn == 1? header.PlayerId : header.Player2Id;

                header.ActualRoundId = round.GameRoundId;
                
                //guardar este cambio
            }
            else
            {
                //esperando que el player2 acepte
                return header;
            }

            round.PlayerCardsObj = Card.GetCards(round.PlayerCards);
            round.Player2CardsObj = Card.GetCards(round.Player2Cards, false);
            round.PlayerLifeCardsObj = Card.GetCards(round.PlayerLifeCards, false);
            round.Player2LifeCardsObj = Card.GetCards(round.Player2LifeCards, false);
            round.AvailableCardsObj = Card.GetCards(round.AvailableCards);
            var pitsSplit = round.CardPits.Split('|');
            var pitCount = 0;

            round.CardPitsObj = new Dictionary<int, List<Card>>();
            foreach (var pit in pitsSplit)
            {
                round.CardPitsObj[pitCount] = Card.GetCards(pit);
                pitCount++;
            }

            header.ActualRound = round;

            return header;
        }

        public static DataService.GameHeader CreateNewGame(string userId, DataService.EmptyHandDBEntities db)
        {

            DataService.GameHeader gH = new DataService.GameHeader();
            gH.GameId = Guid.NewGuid().ToString();
            gH.PlayerId = userId;
            gH.PlayerRoundsWins = 0;
            gH.Player2RoundsWins = 0;
            gH.RoundsCount = 0;
            gH.PlayerPoints = 0;
            gH.Player2Points = 0;
            gH.ActualRound = null;

            //aca habria que guardar en la bd y esperar que acepte el otro player
            db.GameHeaders.Add(gH);
            db.SaveChanges();

            return gH;
        }

    }
}
