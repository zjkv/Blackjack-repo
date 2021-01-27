using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace Blackjack
{
    public class Core
    {
        public static int CARD_WIDTH = 2*79;
        public static int CARD_HEIGHT = 2*123;
        public static int NUM_DECKS = 8;
        public static string CARD_SHEET_RESOURCE = "Blackjack.cards.png";

        public static String[] Suits = new String[] { "Clubs", "Diamonds", "Hearts", "Spades" };
        public static String[] Values = new String[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

        private Deck deck;
        private Image backOfCardImage;

        public Core()
        {
            deck = new Deck();
            SetBackCardImage();
        }

        public Card GetCard(int cardID)
        {
            return deck.GetCard(cardID);
        }

        public Image GetCardImage(int cardID)
        {
            return deck.GetCardImage(cardID);
        }

        public Image GetBackOfCardImage()
        {
            return backOfCardImage;
        }
        private void SetBackCardImage()
        {
            SKBitmap resourceBitmap;

            Assembly assembly = GetType().GetTypeInfo().Assembly;

            using (Stream stream = assembly.GetManifestResourceStream(CARD_SHEET_RESOURCE))
            {
                resourceBitmap = SKBitmap.Decode(stream);
            }

            int row = 4;
            int col = 2;
            SKBitmap card = new SKBitmap(CARD_WIDTH, CARD_HEIGHT);

            SKRect dest = new SKRect(0, 0, CARD_WIDTH, CARD_HEIGHT);
            SKRect source = new SKRect(col * CARD_WIDTH/2, row * CARD_HEIGHT/2 , (col + 1) * CARD_WIDTH/2, (row + 1) * CARD_HEIGHT/2);

            using (SKCanvas canvas = new SKCanvas(card))
            {
                canvas.DrawBitmap(resourceBitmap, source, dest);
            }

            backOfCardImage = new Image
            {
                Source = (SKBitmapImageSource)card
            };

        }
        public Queue<Card> GetGameCards()
        {
            List<Card> shuffleCards = new List<Card>();
            Random rand = new Random();
            Card[] deckCards = deck.GetCards();

            for (int i = 0; i < NUM_DECKS; i++)
            {
                for (int j = 0; j < deckCards.Length; j++)
                {
                    int insertPosition = shuffleCards.Count > 1 ? rand.Next(shuffleCards.Count + 1) : 0;
                    Card card = deckCards[j];
                    shuffleCards.Insert(insertPosition, card);
                }
            }
            Queue<Card> readyDeck = new Queue<Card>(shuffleCards);
            return readyDeck;
        }

    }
}
