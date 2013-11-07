using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApplication1
{
    class BlackJack_Testing
    {
        static Deck deck;
        static int wins;
        static int total;

        //  We are assuming that there is an equal chance of hitting any other card, including the drawn cards
        //  Check what happens if the player stands with two cards
        public static double startTest(Card one, Card two, Card dealer)
        {
            List<Card> playersCards;
            List<Card> dealersCards;
            playersCards = new List<Card>();
            dealersCards = new List<Card>();
            playersCards.Add(one);
            playersCards.Add(two);
            dealersCards.Add(dealer);
            deck = new Deck(1, Deck.ShuffleType.Durstenfeld);
            wins = 0;
            total = 0;
            start(playersCards, dealersCards);
            return (Double)wins / (Double)total;
        }
        private static void start(List<Card> playersCards, List<Card> dealersCards)
        {

            //  Copy the paramaters so we don't edit the references...
            List<Card> PlayersCards = new List<Card>();
            List<Card> DealersCards = new List<Card>();
            foreach (Card c in playersCards)
            {
                PlayersCards.Add(c);
            }
            foreach (Card c in dealersCards)
            {
                DealersCards.Add(c);
            }
            //  Use this to track how many hands the dealer has which can actually be played
            
            foreach (Card c in deck.DeckList)
            {
                DealersCards.Add(c);
                if (getHandValue(DealersCards) >= 17)
                {
                    total += 1;
                    if (!dealerWins(DealersCards, PlayersCards))
                    {
                        wins += 1;
                        foreach (Card b in DealersCards)
                        {
                            System.Diagnostics.Debug.Print(b.value.ToString());
                        }
                        System.Diagnostics.Debug.Print("Win\n\n");


                    }
                    else
                    {
                        foreach (Card b in DealersCards)
                        {
                            System.Diagnostics.Debug.Print(b.value.ToString());
                        }
                        System.Diagnostics.Debug.Print("Lose\n\n");
                    }
                }
                else
                {
                    start(PlayersCards, DealersCards);
                }
                DealersCards.Remove(c);
            }
        }

        public static Boolean dealerWins(List<Card> dealersCards, List<Card> playersCards)
        {
            int playerHand = getHandValue(playersCards);
            int dealerHand = getHandValue(dealersCards);
            if (playerHand > 21) return true;
            if (dealerHand > 21) return false;
            if (dealerHand > playerHand)
            { return true; }
            else
            { return false; }
        }

        public static int getHandValue(List<Card> handCards)
        {
            //  Do this so the values of the 'handCards' list won't be changed.
            List<Card> hand = new List<Card>();
            foreach (Card c in handCards)
            {
                hand.Add(c);
            }
            int handValue = 0;
            int aces = 0;
            while (hand.Count > 0)
            {
                if (hand[0].value == CardValue.Ace)
                {
                    aces += 1;
                }
                handValue = handValue += (int)BlackJack.convertCardToBlackJack(hand[0]).value;
                hand.RemoveAt(0);
                if ((handValue > 21) && (aces > 0))
                {
                    handValue += -10;
                    aces += -1;
                }
            }

            return handValue;
        }

       
    }
}
