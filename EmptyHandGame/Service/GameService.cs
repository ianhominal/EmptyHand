using Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Service
{
    public class GameService
    {
        public static User Login(string username, string password)
        {
            string json = File.ReadAllText(@"D:\Repo\EmptyHand\EmptyHandGame\Service\MockData\UserMock.json");
            var user = JsonConvert.DeserializeObject<User>(json);
            return user;
        }

        public static GameHeader GetActualGame(string gameGuid, int userId)
        {
            string jsonHeader = File.ReadAllText(@"D:\Repo\EmptyHand\EmptyHandGame\Service\MockData\GameHeaderMock.json");
            string jsonRound = File.ReadAllText(@"D:\Repo\EmptyHand\EmptyHandGame\Service\MockData\RoundInfoMock.json");

            var header = JsonConvert.DeserializeObject<GameHeader>(jsonHeader);
            var round = JsonConvert.DeserializeObject<GameRound>(jsonRound);

            round.PlayerCardsObj = Card.GetCards(round.PlayerCards);
            round.Player2CardsObj = Card.GetCards(round.Player2Cards,false);
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
    }
}
