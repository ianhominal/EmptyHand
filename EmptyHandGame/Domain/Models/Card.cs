using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Shapes;
using Path = System.Windows.Shapes.Path;
using static System.Net.Mime.MediaTypeNames;
using System.Globalization;

namespace Domain.Models
{
    public class Card
    {
        public string Suit; //palo
        public int Number;

        public Canvas Image;

        private static List<string> Suits = new List<string>() { "Diamonds", "Spades", "Clubs", "Hearts" };
        private static List<string> Ranks = new List<string>() { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

        private static double CardWidth = 100;
        private static double CardHeight = 140;


        public static List<Card> GetCards(string cardsStr, bool faceUp= true)
        {
            var cardList = new List<Card>();

            var cards = cardsStr.Split(',');

            foreach( var card in cards) 
            {
                var cardProps = card.Split('_');

                var suit = "";
                switch(cardProps[0].ToString())
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

                cardList.Add(new Card(){ 
                    Suit = suit,
                    Number = Ranks.IndexOf(cardProps[1].ToString()),
                    Image = CreateCardImage(suit, cardProps[1].ToString(),faceUp)
                });

            }

            return cardList;
        }

            

        public static Canvas CreateCardImage(string suit, string rank, bool faceUp = true, bool enable = true)
        {
            int rankIndex = Ranks.IndexOf(rank);
            string color = (suit == "Hearts" || suit == "Diamonds") ? "Red" : "Black";
            string suitChar = GetSuitChar(suit,true);

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
                    return isUnicode ? "♣" : "C";
                case "Diamonds":
                    return isUnicode ? "♦" : "D";
                case "Hearts":
                    return isUnicode ? "♥" : "H";
                case "Spades":
                    return isUnicode ? "♠" : "S";
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


        public static List<Card> GenerateDeck()
        {
            var cards = new List<Card>();

            foreach (var suit in Suits)
            {
                foreach (var rank in Ranks)
                {
                    cards.Add(new Card() { Suit = suit, Number = Ranks.IndexOf(rank), Image = CreateCardImage(suit, rank) });

                }
            }

            return cards;
        }

    }
}
