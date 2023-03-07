using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Security.Policy;
using System.IO;

namespace Domain.Models
{
    public class PlayerModel
    {

        public PlayerModel(string playerId, string playerName, string playerPhotoURL, string playerMail)
        {
            this.PlayerId = playerId;
            this.PlayerName = playerName;
            this.PlayerPhoto = playerPhotoURL;

            PlayerPoints = 0;
            PlayerRoundsWins = 0;
            PlayerMail = playerMail;
        }

        public string PlayerId { get; set; }
        public string PlayerHubId { get; set; }
        public List<Card> PlayerCards { get; set; }
        public List<Card> PlayerLifeCards { get; set; }
        public int PlayerPoints { get; set; }
        public int PlayerRoundsWins { get; set; }
        public string PlayerName { get; set; }
        public string PlayerPhoto { get; set; }
        public string PlayerMail { get; set; }



        public static Ellipse GetUserPhoto(string url)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(url, UriKind.Absolute);
            bitmap.EndInit();

            // Crea un nuevo objeto "ImageBrush" y asigna la imagen cargada a su propiedad "ImageSource".
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = bitmap;



            Ellipse ellipse = new Ellipse();
            ellipse.Width = 100;
            ellipse.Height = 100;
            //Color color = (Color)ColorConverter.ConvertFromString("#FF673AB7");
            //ellipse.Stroke = new SolidColorBrush(color);
            ////21005D
            //ellipse.StrokeThickness = 4;

            // Asigna el objeto "ImageBrush" a la propiedad "Fill" del objeto "Ellipse".
            ellipse.Fill = imageBrush;

            return ellipse;

        }

        public static Ellipse GetBackgroundUserEllipse()
        {
            Ellipse ellipsePlayerBg = new Ellipse();
            ellipsePlayerBg.Width = 106;
            ellipsePlayerBg.Height = 106;
            //ellipsePlayerBg.Margin = new Thickness(0, 0, 0, 0);

            Color color = (Color)ColorConverter.ConvertFromString("#21005D");

            ellipsePlayerBg.Fill = new SolidColorBrush(color);

            return ellipsePlayerBg;
        }
    }
}
