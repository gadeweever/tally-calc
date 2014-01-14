using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Controls.Primitives;

namespace tallycalc
{
    public partial class TallyGroupPage : PhoneApplicationPage
    {
        private Popup popup;
        NameOverlay ovr;
        public bool isPicking;

        public TallyGroupPage()
        {
            ovr = new NameOverlay();
            this.popup = new Popup();
            isPicking = false;
            InitializeComponent();
            tallyItemList.Height = Application.Current.Host.Content.ActualHeight * 0.6625;
            tallyItemList.Width = Application.Current.Host.Content.ActualWidth;
        }

        

        private void CreateNewTallyItem(object sender, EventArgs e)
        {
            //the application bar button has been selected but we are not currently in an overlay
            if (!isPicking)
            {
                if (ovr.numeralNameBox.Text.CompareTo("New Numeral") != 0)
                    ovr.numeralNameBox.Text = "";
                else
                    ovr.numeralNameBox.Text = "New Item";
                isPicking = !isPicking;
                //set visible properties of the overlay
                this.LayoutRoot.Opacity = 1;
                this.popup.Child = ovr;
                this.popup.Opacity = .8;
                this.popup.IsOpen = true;
                SystemTray.IsVisible = false; //to hide system tray
            }
            //the applciation bar button has been selected and we are currently in overlay
            else
            {
                if (Globals.GetNumeralItemIndexByName(ovr.numeralNameBox.Text) != -1)
                {
                    MessageBox.Show("You already have a numeral with the same name!");
                    return;
                }

                
                Globals.CurrentTally.tallyItems.Add(new TallyItem(ovr.numeralNameBox.Text));
                Globals.CurrentTallyItem = Globals.CurrentTally.tallyItems[Globals.CurrentTally.tallyItems.Count - 1];
                Globals.SaveStorageData();

                ResetList();

                Return();
              
            }
        }
        #region Navigation Overrides
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (isPicking)
                Return();
            base.OnBackKeyPress(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            tallyGroupPageTitle.Text = Globals.CurrentTally.name;
            tallyItemList.ItemsSource = Globals.CurrentTally.tallyItems;
            base.OnNavigatedTo(e);
        }

        public void Return()
        {
            this.popup.IsOpen = false;
            this.LayoutRoot.Opacity = 1;
            SystemTray.IsVisible = true;
            isPicking = !isPicking;

        }
        #endregion

        private void NavigateTallyItem(object sender, SelectionChangedEventArgs e)
        {
            Globals.CurrentTallyItem = tallyItemList.SelectedItem as TallyItem;
            NavigationService.Navigate(new Uri("/TallyItemPage.xaml",UriKind.Relative));
        }

        #region Voting Handlers
        private void UpvoteTallyItem(object sender, RoutedEventArgs e)
        {
            //while this is correct, it is the worst thing ever
            StackPanel content = ((sender as Button).Parent as StackPanel);
            string wow = (content.Children[content.Children.Count - 1] as TextBlock).Text;
            Globals.CurrentTallyItem = Globals.CurrentTally.tallyItems[(Globals.GetNumeralItemIndexByName(wow))];
            
            Globals.CurrentTallyItem.count++;
            
            ResetList();

        }

        private void DownVoteTallyItem(object sender, RoutedEventArgs e)
        {
            //while this is correct, it is the worst thing ever
            StackPanel content = ((sender as Button).Parent as StackPanel);
            string wow = (content.Children[content.Children.Count - 1] as TextBlock).Text;
            Globals.CurrentTallyItem = Globals.CurrentTally.tallyItems[(Globals.GetNumeralItemIndexByName(wow))];
            
            if(Globals.CurrentTallyItem.count > 0)
                Globals.CurrentTallyItem.count--;
            
            ResetList();

        }

        private void ResetList()
        {
            tallyItemList.ItemsSource = null;
            tallyItemList.ItemsSource = Globals.CurrentTally.tallyItems;
        }
        #endregion

    }
}