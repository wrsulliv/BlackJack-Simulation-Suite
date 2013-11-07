using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApplication1
{
    class Card
    {
        public CardValue value;
        public CardSuit suit;
        public Card(CardValue value, CardSuit suit)
        {
            this.value = value;
            this.suit = suit;
        }
    }

    public enum CardValue
    {
        Ace = 1, Two = 2, Three = 3, Four = 4, Five = 5, Six = 6, Seven = 7, Eight = 8, Nine = 9,
        Ten = 10, Jack = 11, Queen = 12, King = 13
    }

    public enum CardSuit
    {
        Spades = 1, Clubs = 2, Diamonds = 3, Hearts = 4
    }
}

