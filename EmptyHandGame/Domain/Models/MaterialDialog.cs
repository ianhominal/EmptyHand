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
                Width = 600,
                Height = 200
            };


            var grdCard = new Grid();
            grdCard.RowDefinitions.Add(new RowDefinition { Height = new GridLength(70) });
            grdCard.RowDefinitions.Add(new RowDefinition { Height = new GridLength(150) });

            // Crea un objeto CardHeader de Material Design
            var _headerBorder = new Border();
            //_headerBorder.Background = PreviewKeyUpnew GridLength(70)
            _cardHeader = new TextBlock
            {
                FontSize = 12,
                Text = "tes2"
            };
            Grid.SetRow(_cardHeader, 0);
            grdCard.Children.Add(_cardHeader);

            // Crea un objeto TextBlock
            _textBlock = new TextBlock
            {
                FontSize = 18,
                Text = "test"
            };
            Grid.SetRow(_textBlock, 1);
            grdCard.Children.Add(_textBlock);


            // Agrega el TextBlock como elemento hijo del Card
            _card.Content = grdCard;


            // Agrega el Card como elemento hijo del Grid
            //grid.Children.Add(_card);

            // Agrega el Grid como elemento hijo del UserControl
            Content = _card;
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
