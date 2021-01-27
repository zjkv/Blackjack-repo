using System;
using System.IO;
using System.Reflection;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace Blackjack
{
    public class Deck
    {
        private Card[] Cards = new Card[52];
        private Image[] Images = new Image[52];

        public Deck()
        {
            InitDeck();
        }

        public Card[] GetCards()
        {
            return Cards;
        }

        public Card GetCard(int CardID)
        {
            return Cards[CardID];
        }

        public Image GetCardImage(int CardID)
        {
            return Images[CardID];
        }
        private void InitDeck()
        {

            SKBitmap resourceBitmap;
            Assembly assembly = GetType().GetTypeInfo().Assembly;

            using (Stream stream = assembly.GetManifestResourceStream(Core.CARD_SHEET_RESOURCE))
            {
                resourceBitmap = SKBitmap.Decode(stream);
            }

            for ( int i=0; i<52; i++) {
                int cardSuiteIndex = i / 13;  
                int cardValueIndex = i % 13; 
                int cardNumValue = (i+1) % 13;  
                if (cardValueIndex > 9)  
                    cardNumValue = 10;

                Cards[i] = new Card(i, Core.Suits[cardSuiteIndex], Core.Values[cardValueIndex], cardNumValue );

                SKBitmap card = new SKBitmap(Core.CARD_WIDTH, Core.CARD_HEIGHT);

                SKRect dest = new SKRect(0, 0, Core.CARD_WIDTH, Core.CARD_HEIGHT);
                SKRect source = new SKRect(cardValueIndex * Core.CARD_WIDTH/2, cardSuiteIndex * Core.CARD_HEIGHT/2, (cardValueIndex + 1) * Core.CARD_WIDTH/2, (cardSuiteIndex + 1) * Core.CARD_HEIGHT/2);

                using (SKCanvas canvas = new SKCanvas(card))
                {
                    canvas.DrawBitmap(resourceBitmap, source, dest);
                }
                Image image = new Image
                {
                    Source = (SKBitmapImageSource)card
                };
                Images[i] = image;
            }
        }
    }
}
