using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Blackjack
{
    [DesignTimeVisible(false)]
    public partial class StartPage : ContentPage
    {
        private int[] CashLookup = new int[] { 10, 50, 100, 500 };
        public StartPage()
        {
            InitializeComponent();
        }
        void OnContinueButtonClicked(object sender, EventArgs e)
        {
            if (CashPicker.SelectedIndex >= 0)
            {
                int UserCashIndex = CashPicker.SelectedIndex;
                int UserCash = CashLookup[UserCashIndex];
                App.CurrentCash = UserCash;
                Application.Current.MainPage = new GamePage(UserCash);
            }
            else
            {
                DisplayAlert("Błąd", "Nie wybrano kwoty", "OK");
                return;
            }
        }
    }
}
