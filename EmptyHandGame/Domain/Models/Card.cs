using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Path = System.Windows.Shapes.Path;
using System.Globalization;
using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class Card
    {
        public string Suit { get; set; } //palo
        public int Number { get; set; }
        public bool CanBePlayed { get; set; }

        [JsonIgnore]
        public string Rank
        {
            get { return Ranks[Number]; }
        }

        public static List<string> Suits = new List<string>() { "D", "P", "T", "C" };
        public static List<string> Ranks = new List<string>() { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

        public static readonly int STARTHANDCARDSCOUNT = 7;
        public static readonly int STARTLIFECARDSCOUNT = 3;

        public static double CardWidth = 100;
        public static double CardHeight = 140;


        public static List<Card> GetCards(string cardsStr)
        {
            var cardList = new List<Card>();
            if (string.IsNullOrEmpty(cardsStr)) return cardList;

            var cards = cardsStr.Split(',');

            foreach (var card in cards)
            {
                var cardProps = card.Split('_');

                var suit = "";
                switch (cardProps[0].ToString())
                {
                    case "C":
                        suit = "Hearts";
                        break;
                    case "D":
                        suit = "Diamonds";
                        break;
                    case "T":
                        suit = "Clubs";
                        break;
                    case "P":
                        suit = "Spades";
                        break;
                }

                cardList.Add(new Card()
                {
                    Suit = suit,
                    Number = Ranks.IndexOf(cardProps[1].ToString()),
                    Image = CreateCardImage(suit, cardProps[1].ToString())
                });

            }

            return cardList;
        }

        public static Canvas CreateCardImage(string suit, string rank, bool faceUp = true, bool enable = true)
        {
            string color = (suit == "Hearts" || suit == "Diamonds") ? "Red" : "Black";

            Canvas cardCanvas = new Canvas
            {
                Width = CardWidth,
                Height = CardHeight,
                Background = Brushes.White
            };


            Border shadow = new Border
            {
                Background = Brushes.DarkGray,
                Margin = new Thickness(5, 5, 0, 0),
                BorderThickness = new Thickness(1),
                Width = CardWidth,
                Height = CardHeight,
                //CornerRadius = new CornerRadius(5)
            };
            cardCanvas.Children.Add(shadow);


            if (faceUp)
            {


                Border cardColor = new Border
                {
                    Background = Brushes.White,
                    BorderThickness = new Thickness(2),
                    Width = CardWidth,
                    Height = CardHeight
                };
                cardCanvas.Children.Add(cardColor);


                Rectangle border = new Rectangle
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,
                    Width = CardWidth,
                    Height = CardHeight
                };
                cardCanvas.Children.Add(border);

                Border border2 = new Border
                {
                    BorderBrush = Brushes.DarkGray,
                    Margin = new Thickness(5, 5, 5, 5),
                    BorderThickness = new Thickness(1),
                    Width = CardWidth - 10,
                    Height = CardHeight - 10,
                    CornerRadius = new CornerRadius(5)
                };

                cardCanvas.Children.Add(border2);


                string faceSuitChar = GetSuitChar(suit, true);

                TextBlock faceText = new TextBlock
                {
                    Text = rank,
                    FontSize = 35,
                    FontWeight = FontWeights.Bold,
                    Foreground = (Brush)new BrushConverter().ConvertFromString(color),
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Canvas.SetTop(faceText, 10);
                Canvas.SetLeft(faceText, 10);
                cardCanvas.Children.Add(faceText);

                var suitSymbol1 = GetSuitSymbol(faceSuitChar, color, 20);
                Canvas.SetBottom(suitSymbol1, 10);
                Canvas.SetLeft(suitSymbol1, 10);
                cardCanvas.Children.Add(suitSymbol1);

                var suitSymbol2 = GetSuitSymbol(faceSuitChar, color, 20);
                Canvas.SetTop(suitSymbol2, 10);
                Canvas.SetRight(suitSymbol2, 10);
                cardCanvas.Children.Add(suitSymbol2);

                var suitSymbol3 = GetSuitSymbol(faceSuitChar, color, 50);
                Canvas.SetBottom(suitSymbol3, 10);
                Canvas.SetRight(suitSymbol3, 10);
                cardCanvas.Children.Add(suitSymbol3);
            }
            else
            {
                Border cardColor = new Border
                {
                    Background = Brushes.DarkBlue,
                    BorderThickness = new Thickness(2),
                    Width = CardWidth,
                    Height = CardHeight
                };
                cardCanvas.Children.Add(cardColor);
            }


            return cardCanvas;
        }

        public static Border GetDisabledCard()
        {
            Border cardColor = new Border
            {
                Background = new SolidColorBrush(Color.FromArgb(220, 64, 64, 64)),
                BorderThickness = new Thickness(2),
                Width = CardWidth,
                Height = CardHeight
            };

            return cardColor;
        }

        private static string GetSuitChar(string suit, bool isUnicode)
        {
            switch (suit)
            {
                case "Clubs":
                    return isUnicode ? "♣" : "T";
                case "Diamonds":
                    return isUnicode ? "♦" : "D";
                case "Hearts":
                    return isUnicode ? "♥" : "C";
                case "Spades":
                    return isUnicode ? "♠" : "P";
                default:
                    throw new ArgumentException("Invalid suit value.");
            }
        }

        private static Path GetSuitSymbol(string suitChar, string color, double symbolSize = 50)
        {
            PathGeometry pathGeometry = new PathGeometry();

            Typeface typeface = new Typeface("Arial");
            Brush brush = new SolidColorBrush(color == "Black" ? Colors.Black : Colors.Red);

            FormattedText formattedText = new FormattedText(
                suitChar,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeface,
                symbolSize,
                brush
            );

            Geometry textGeometry = formattedText.BuildGeometry(new Point(0, 0));
            pathGeometry.AddGeometry(textGeometry);

            var path = new Path
            {
                Stroke = new SolidColorBrush(Colors.Black),
                Fill = new SolidColorBrush(color == "Black" ? Colors.Black : Colors.Red),
                Data = pathGeometry
            };

            return path;
        }

        public static List<string> RandomizeList(List<string> cardList)
        {
            Random _rand = new Random();
            for (int i = cardList.Count - 1; i > 0; i--)
            {
                var k = _rand.Next(i + 1);
                var value = cardList[k];
                cardList[k] = cardList[i];
                cardList[i] = value;
            }
            return cardList;
        }

        public static string ToStringList(List<Card> cardList)
        {
            List<string> cardsString = new List<string>();
            foreach (var card in cardList)
            {
                var cardSuit = GetSuitChar(card.Suit, false);

                cardsString.Add($"{cardSuit}_{Ranks[card.Number]}");
            }

            return string.Join(',', cardsString);
        }
    }
}
