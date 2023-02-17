using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Domain.Models
{
    public class MaterialDialog : UserControl
    {
        private MaterialDesignThemes.Wpf.Card _card;
        private TextBlock _cardHeader;
        private TextBlock _textBlock;
        public Button btnClose;

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(MaterialDialog), new PropertyMetadata(null, OnTitleChanged));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(MaterialDialog), new PropertyMetadata(null, OnMessageChanged));

        public MaterialDialog()
        {
            // Crea un objeto Grid
            //var grid = new Grid();


            // Crea un objeto Card de Material Design
            _card = new MaterialDesignThemes.Wpf.Card
            {
                Padding = new Thickness(16),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                //Width = 600,
                //Height = 200
            };


            var grdCard = new Grid();
            grdCard.Margin = new Thickness(20);

            var stackpanel = new StackPanel();
            
            grdCard.Children.Add(stackpanel);

            // Crea un objeto TextBlock
            _textBlock = new TextBlock
            {
                FontSize = 18,
                Text = "test",
                Height=30,
                VerticalAlignment = VerticalAlignment.Center,

            };
            Grid.SetRow(_textBlock, 1);
            stackpanel.Children.Add(_textBlock);

            btnClose = new Button();
            btnClose.Content = "Aceptar";
            btnClose.Click += Close_Dialog;
            btnClose.Margin =  new Thickness(0,15,0,0);
            btnClose.VerticalAlignment= VerticalAlignment.Bottom;
            stackpanel.Children.Add(btnClose);


            // Agrega el TextBlock como elemento hijo del Card
            _card.Content = grdCard;
            _card.VerticalAlignment = VerticalAlignment.Center;

            // Agrega el Card como elemento hijo del Grid
            //grid.Children.Add(_card);

            // Agrega el Grid como elemento hijo del UserControl
            Content = _card;
        }

        private void Close_Dialog(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dialog = (MaterialDialog)d;
            dialog._cardHeader.Text = (string)e.NewValue;
        }

        private static void OnMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dialog = (MaterialDialog)d;
            dialog._textBlock.Text = (string)e.NewValue;
        }

    }
}
