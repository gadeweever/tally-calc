using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace tallycalc
{
    public partial class NameOverlay : UserControl
    {
        public NameOverlay()
        {
            InitializeComponent();
            this.LayoutRoot.Height = Application.Current.Host.Content.ActualHeight;
            this.LayoutRoot.Width = Application.Current.Host.Content.ActualWidth;

            nameUseText.Text = "This name will identify the numerals group you create";
        }

        private void ClearText(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).Text = "";
        }

        private void CheckName(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBox).Text.CompareTo("") == 0)
                (sender as TextBox).Text = "New Numeral";
        }
    }
}
