using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using tallycalc.Resources;
using System.Windows.Controls.Primitives;
using System.IO.IsolatedStorage;
using System.IO;

namespace tallycalc
{
    public partial class MainPage : PhoneApplicationPage
    {
        private Popup popup;
        NameOverlay ovr;
        public bool isPicking;
        private int tallyIndex;
        // Constructor
        public MainPage()
        {
            ovr = new NameOverlay();
            this.popup = new Popup();
            isPicking = false;
            tallyIndex = -1;
            InitializeComponent();
            Globals.CurrentTallies = new List<TallyGroup>();
            LoadStoredData();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void CreateNewNumeral(object sender, EventArgs e)
        {
            //the application bar button has been selected but we are not currently in an overlay
            if (!isPicking)
            {
                if(ovr.numeralNameBox.Text.CompareTo("New Numeral") != 0)
                    ovr.numeralNameBox.Text = "";
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
                if(Globals.GetNumeralIndexByName(ovr.numeralNameBox.Text) != -1)
                {
                    MessageBox.Show("You already have a numeral with the same name!");
                    return;
                }

                TallyGroup newGroup = new TallyGroup(ovr.numeralNameBox.Text);
                Globals.CurrentTallies.Add(newGroup);
                Globals.CurrentTally = Globals.CurrentTallies[Globals.CurrentTallies.Count - 1];
                Globals.SaveStorageData();

                tallyList.ItemsSource = null;
                tallyList.ItemsSource = Globals.CurrentTallies;

                Return();
                NavigationService.Navigate(new Uri("/TallyGroupPage.xaml",UriKind.Relative));   
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
            base.OnNavigatedTo(e);

            tallyList.SelectedIndex = -1;
        }
        #endregion

        public void Return()
        {
            this.popup.IsOpen = false;
            this.LayoutRoot.Opacity = 1;
            SystemTray.IsVisible = true;
            isPicking = !isPicking;

        }

        private int LoadStoredData()
        {
            using (var filesystem = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!filesystem.FileExists("tallies.dat"))
                {
                    return 1;
                }
                else
                {
                    using (var fs = new IsolatedStorageFileStream(
                     "tallies.dat", FileMode.Open, filesystem))
                    {
                        var serializer = new System.Runtime.Serialization
                          .Json.DataContractJsonSerializer(typeof(
                          List<TallyGroup>));
                        Globals.CurrentTallies = serializer.ReadObject(fs) as
                          List<TallyGroup>;
                    }
                    //reset menu list to the stored assignments
                    tallyList.ItemsSource = null;
                    tallyList.ItemsSource = Globals.CurrentTallies;

                }
            }
            return 0;

        }

        private void NavigateToTallyGroup(object sender, SelectionChangedEventArgs e)
        {
            if (tallyList.SelectedIndex < 0)
                return;
            Globals.CurrentTally = Globals.CurrentTallies[tallyList.SelectedIndex];
            NavigationService.Navigate(new Uri("/TallyGroupPage.xaml", UriKind.Relative));
        }

        private void DeleteTallyGroup(object sender, RoutedEventArgs e)
        {
            if (tallyIndex < 0)
                return;
            Globals.CurrentTallies.RemoveAt(tallyIndex);
            tallyList.ItemsSource = null;
            tallyList.ItemsSource = Globals.CurrentTallies;
            Globals.SaveStorageData();
        }

        private void GetSelectedIndexByName(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string tallyIndexName = (sender as TextBlock).Text;
            tallyIndex = Globals.GetNumeralIndexByName(tallyIndexName);
        }

    }
}