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
            tallyItemList.Height = Application.Current.Host.Content.ActualHeight * 0.525;
            tallyItemList.Width = Application.Current.Host.Content.ActualWidth;
            listDivider.Width = Application.Current.Host.Content.ActualWidth * .9;
            CheckList();
            UpdateHighest();
            
        }

        

        private void CreateNewTallyItem(object sender, EventArgs e)
        {
            //the application bar button has been selected but we are not currently in an overlay
            if (!isPicking)
            {
                ovr.numeralNameBox.Text = "New Item";

                isPicking = true;
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

                //enable buttons
                highestUpvoteButton.IsEnabled = true;
                highestDownvoteButton.IsEnabled = true;

                Return();
                CheckList();
              
            }
        }
        #region Navigation Overrides
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (isPicking)
            {
                Return();
                e.Cancel = true;
            }

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            tallyGroupPageTitle.Text = Globals.CurrentTally.name;
            tallyItemList.ItemsSource = Globals.CurrentTally.tallyItems;
            CheckList();
            base.OnNavigatedTo(e);
        }

        public void Return()
        {
            this.popup.IsOpen = false;
            this.LayoutRoot.Opacity = 1;
            SystemTray.IsVisible = true;
            isPicking = false;

        }
        #endregion

        #region Navigation Handlers
        private void NavigateTallyItem(object sender, SelectionChangedEventArgs e)
        {
            //if (tallyItemList.SelectedIndex < 0)
            //    return;
            //Globals.CurrentTallyItem = tallyItemList.SelectedItem as TallyItem;
            //NavigationService.Navigate(new Uri("/TallyItemPage.xaml",UriKind.Relative));
        }
        #endregion

        #region Voting Handlers
        private void UpvoteTallyItem(object sender, RoutedEventArgs e)
        {
            //while this is correct, it is the worst thing ever
            StackPanel content = ((sender as Button).Parent as StackPanel);
            string wow = (content.Children[content.Children.Count - 1] as TextBlock).Text;
            Globals.CurrentTallyItem = Globals.CurrentTally.tallyItems[(Globals.GetNumeralItemIndexByName(wow))];
            
            Globals.CurrentTallyItem.count++;
            Globals.CurrentTally.total++;
            
            UpdateHighest();
            ResetList();

        }

        private void DownVoteTallyItem(object sender, RoutedEventArgs e)
        {
            //while this is correct, it is the worst thing ever
            StackPanel content = ((sender as Button).Parent as StackPanel);
            string wow = (content.Children[content.Children.Count - 1] as TextBlock).Text;
            Globals.CurrentTallyItem = Globals.CurrentTally.tallyItems[(Globals.GetNumeralItemIndexByName(wow))];

            if (Globals.CurrentTallyItem.count > 0)
            {
                Globals.CurrentTallyItem.count--;
                Globals.CurrentTally.total--;
            }
            UpdateHighest();
            ResetList();

            //Globals.SaveStorageData();
            

        }

        private void ResetList()
        {
            tallyItemList.ItemsSource = null;
            tallyItemList.ItemsSource = Globals.CurrentTally.tallyItems;
        }

        private void UpdateHighest()
        {
            if (Globals.CurrentTally.tallyItems.Count < 1)
                return;

            TallyItem highest = Globals.CurrentTally.GetHighestCount();
            highestCount.Text = highest.count + "";
            highestName.Text = highest.name + "";
        }
        

        private void UpvoteHighestItem(object sender, RoutedEventArgs e)
        {
            Globals.CurrentTallyItem = Globals.CurrentTally.tallyItems[(Globals.GetNumeralItemIndexByName(highestName.Text))];
            
            Globals.CurrentTallyItem.count++;
            Globals.CurrentTally.total++;

            highestCount.Text = Globals.CurrentTallyItem.count + "";

            ResetList();

            //Globals.SaveStorageData();
        }

        private void DownVoteHighestItem(object sender, RoutedEventArgs e)
        {
            Globals.CurrentTallyItem = Globals.CurrentTally.tallyItems[(Globals.GetNumeralItemIndexByName(highestName.Text))];
            if (Globals.CurrentTallyItem.count > 0)
            {
                Globals.CurrentTallyItem.count--;
                Globals.CurrentTally.total--;
            }
            highestCount.Text = Globals.CurrentTallyItem.count + "";

            ResetList();

            //Globals.SaveStorageData();
        }

        #endregion

        #region Deletion Handlers
        private void GetNumeralItemIndexByName(object sender, System.Windows.Input.GestureEventArgs e)
        {
            StackPanel content = sender as StackPanel;
            Globals.CurrentTallyItem = Globals.CurrentTally.tallyItems[Globals.GetNumeralItemIndexByName((content.Children[content.Children.Count - 1] as TextBlock).Text)];
        }
        
        private void DeleteTallyItem(object sender, RoutedEventArgs e)
        {
            Globals.CurrentTally.tallyItems.Remove(Globals.CurrentTallyItem);
            ResetList();
            UpdateHighest();
            CheckList();

            //Globals.SaveStorageData();
        }

        private void CheckList()
        {
            if (Globals.CurrentTally.tallyItems.Count == 0 || Globals.GetNumeralItemIndexByName(highestName.Text) == -1)
            {
                highestUpvoteButton.IsHitTestVisible = false;
                highestDownvoteButton.IsHitTestVisible = false;
                highestName.Text = "";
                highestCount.Text = "";
            }
            else
            {
                highestUpvoteButton.IsHitTestVisible = true;
                highestDownvoteButton.IsHitTestVisible = true;
            }
        }

        #endregion


    }
}