using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace Blackjack
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GamePage : ContentPage
    {
        private int userBalance;
        private Core core = new Core();
        private Queue<Card> cardList;

        Random rand = new Random();

        private int currentBet;

        private List<Card> dealerCards = new List<Card>();
        private int dealerScore;

        private List<Card> playerCards = new List<Card>();
        private int playerScore;

        private bool playerHasAce = false;
        private bool dealerHasAce = false;
        private int playerAces = 0;
        private int dealerAces = 0;


        public GamePage(int money)
        {
            userBalance = money;
            InitializeComponent();
            balance.Text = "Wygląda na to że masz " + userBalance +"zł";

            SetLayout();

            cardList = core.GetGameCards();
        }

        private void NewGame()
        {
            playerStack.Children.Clear();
            dealerStack.Children.Clear();
            playerCards.Clear();
            dealerCards.Clear();

            playerScore = 0;
            dealerScore = 0;

            cardList.Clear();
            cardList = core.GetGameCards();

            bet.Text = "0";
            balance.Text = "Wygląda na to że masz " + userBalance + "zł";
            standButton.IsVisible = false;
            hitButton.IsVisible = false;
            startButton.IsVisible = false;
            betButton.IsVisible = true;
        }

        private void SetLayout()
        {
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            int displayWidth = (int)(mainDisplayInfo.Width / mainDisplayInfo.Density);
            int displayHeight = (int)(mainDisplayInfo.Height / mainDisplayInfo.Density);
                                 
            dealerStack.VerticalOptions = LayoutOptions.StartAndExpand;
            playerStack.VerticalOptions = LayoutOptions.EndAndExpand;         

        }

        private async void OnBetClicked(object sender, EventArgs e)
        {
            try
            {
                string result = await DisplayPromptAsync("Witam", "Za ile chcesz zagrać?", initialValue: "50", keyboard: Keyboard.Numeric);
                while (Convert.ToInt32(result) > userBalance)
                {
                    result = await DisplayPromptAsync("Brak środków", "Wprowadź mniejszą kwotę", initialValue: "5", keyboard: Keyboard.Numeric);
                }

                currentBet = Convert.ToInt32(result);
                userBalance -= currentBet;
                balance.Text = "Wygląda na to że masz " + userBalance + "zł";
                bet.Text = "POSTAWIONO = " + currentBet + " zł";
                betButton.IsVisible = false;
                startButton.IsVisible = true;
            } catch(FormatException)
            {

            }
        }

        private void OnGameStart(object sender, EventArgs e)
        {
            Card card1 = cardList.Dequeue();
            playerStack.Children.Add(new Image { Source = core.GetCardImage(card1.CardID).Source });
            playerCards.Add(card1);

            Card dealerCard1 = cardList.Dequeue();
            dealerStack.Children.Add(core.GetBackOfCardImage());
            dealerCards.Add(dealerCard1);

            Card card2 = cardList.Dequeue();
            playerStack.Children.Add(new Image { Source = core.GetCardImage(card2.CardID).Source });
            playerCards.Add(card2);
            

            Card dealerCard2 = cardList.Dequeue();
            dealerStack.Children.Add(core.GetCardImage(dealerCard2.CardID));
            dealerCards.Add(dealerCard2);

            playerScore = CalculatePlayerScore();
            dealerScore = CalculateDealerScore();

            if (playerScore == 21 && dealerScore < 21)
            {
                currentBet = (int)(currentBet * 1.5);
                DealerBust();
            }
            else if (playerScore < 21 && dealerScore == 21)
            {
                dealerStack.Children.Remove(core.GetBackOfCardImage());
                dealerStack.Children.Add(core.GetCardImage(dealerCard1.CardID));
                PlayerBust();
            }
            else if (playerScore == 21 && dealerScore == 21)
            {
                TieGame();
            }
            else
            {
                startButton.IsVisible = false;
                standButton.IsVisible = true;
                hitButton.IsVisible = true;
            }
        }

        private void OnHitButtonClick(object sender, EventArgs e)
        {
            Card nextCard = cardList.Dequeue();
            playerStack.Children.Add(new Image { Source = core.GetCardImage(nextCard.CardID).Source });
            playerCards.Add(nextCard);

            playerScore = CalculatePlayerScore();

            if( playerScore > 21)
            {
                PlayerBust();    
            }
            else if( playerScore == 21)
            {
                hitButton.IsVisible = false;
            }
        }

        private void OnStandButtonClick(object sender, EventArgs e)
        {
            hitButton.IsVisible = false;
            standButton.IsVisible = false;
            dealerStack.Children.Remove(core.GetBackOfCardImage());
            dealerStack.Children.Add(core.GetCardImage( dealerCards[0].CardID));
            DealerAction();
            
        }

        private void DealerHit()
        {
            Card nextCard = cardList.Dequeue();
            dealerStack.Children.Add(core.GetCardImage(nextCard.CardID));
            dealerCards.Add(nextCard);

            dealerScore = CalculateDealerScore();
            if (dealerScore > 21)
                DealerBust();
            else
                DealerAction();
        }

        private void DealerAction()
        {
            if (dealerScore < 17)
            {
                DealerHit();
            }
            else if ( dealerScore < playerScore)
            {
                DealerHit();
            }
            else if (dealerScore > playerScore && dealerScore < 22)
            {
                PlayerBust();
            }
            else if ( dealerScore == playerScore)
            {
                if (dealerCards.Count > playerCards.Count)
                    DealerBust();
                else if (dealerCards.Count < playerCards.Count)
                    PlayerBust();
                else
                    TieGame();
            } 
        }

        private int CalculatePlayerScore()
        {
            int score = 0;
            int offset = 0;
            foreach( Card c in playerCards)
            {
                int s = c.NumericValue;
                if( s == 1 )
                {
                    s += 10;
                    offset += 10;
                }
                score += s;
            }
            while( score > 21 && offset > 0)
            {
                score -= 10;
                offset -= 10;
            }
            return score;
        }


        private int CalculateDealerScore()
        {
            int score = 0;
            int offset = 0;
            foreach (Card c in dealerCards)
            {
                int s = c.NumericValue;
                if (s == 1)
                {
                    s += 10;
                    offset += 10;
                }
                score += s;
            }
            while (score > 21 && offset > 0)
            {
                score -= 10;
                offset -= 10;
            }
            return score;
        }

        private async void PlayerBust()
        {
            bool answer = await DisplayAlert("Gra zakończona", "Krupier wygrał", "Jeszcze raz", "Odejdź");
            CheckForNewGame(answer);
        }
        private async void DealerBust()
        {
            bool answer = await DisplayAlert("Gra zakończona", "Wygrałeś!!", "Jeszcze raz", "Odejdź");
            userBalance += currentBet * 2;
            CheckForNewGame(answer);
        }
        private async void TieGame()
        {
            bool answer = await DisplayAlert("Gra zakończona", "Remis", "Jeszcze raz", "Odejdź");
            userBalance += currentBet;
            CheckForNewGame(answer);
        }

        private void CheckForNewGame(bool answer)
        {
            App.CurrentCash = userBalance;
            if(userBalance < 1)
            {
                DisplayAlert("Brak pieniędzy", "Doładuj środki", "Ok");
                Application.Current.MainPage = new StartPage();
            }
            if (answer)
            {
                NewGame();
            }
            else
            {
                Application.Current.MainPage = new StartPage();
            }

        }
    }
}