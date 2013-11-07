using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApplication1
{

    //  Purpose:  Holds the information regarding how a BlackJack game will be played, for example,
    //  Choose which type of game to be played, the number of players, the bet spread, the table position, etc...
    struct BlackJackGameParams
    {
        //  We always use DealerPeak
        public int numPlayers;
        public int tablePosition;
        public bool H17; //  True = H17, False = S17
        public bool DAS; //  True = Double After Split is Allowed, False = Double After Split Not Allowed
        public bool RSA; //  True = Resplit Aces Allowed, False = Resplit Aces Not Allowed
        public bool Surrender;//  True = Surrender Allowed, False = Surrender Not Allowed
        public int bankroll;  //  Starting Money
        public int minBet;
        public int maxBet;
        public int betSpread; //  Multiple of the minBet which the player is allowed to go
        public Double hoursOfPlay; //  How many hours to play for?
        public int numDecks; //  How many decks to use?
        public double deckPenetration; // Percentage of the deck to go through before cutting the cards.
        public CountingMethod countingMethod; //  What method to use to count the cards
        public bool useBetRamp; //  Used to set whether a bet ramp is used, or a simple 1-Max spread
    }

    enum CountingMethod
    { 
        KO = 0, HILO = 1, NONE = 2
    }


}
