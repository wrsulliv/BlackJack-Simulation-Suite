using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace WpfApplication1
{
    //  Purpose:  Holds many helpful functions for dealing with the game of BlackJack
    class BlackJack
    {
        Deck deck;
        Dealer dealer;
        List<Player> players;
        String outputFile = "";
        Boolean reportByFirstTwoCards = false;
        Delegate progressUpdateCodeDelegate;
        int iterationsOfEntireDeck;
        public BlackJack(int DeckCount, Boolean H17, int PlayerCount, int hitCount, String outputFile, Boolean reportByFirstTwoCards, Delegate progressUpdateCodeDelegate, int iterationsOfEntireDeck, Deck.ShuffleType shuffleType)
        {
            deck = new Deck(DeckCount, shuffleType); 
            dealer = new Dealer(H17, this.deck);
            players = new List<Player>();
            for (int x = 0; x < PlayerCount; x++)
            {
                players.Add(new Player(hitCount));
            }
            this.outputFile = outputFile;
            this.reportByFirstTwoCards = reportByFirstTwoCards;
            this.progressUpdateCodeDelegate = progressUpdateCodeDelegate;
            this.iterationsOfEntireDeck = iterationsOfEntireDeck;
        }
        //  Tells the information about the current specific game
        public void showGameInfo()
        {
            string playersHand = " - ";
            //  As of right now, only one player with one hand. (No Splits)
            foreach (Card c in players[0].getHand(0))
            {
                playersHand += c.value + " - ";
            }
            string dealersHand = " - ";
            foreach (Card c in dealer.getHand())
            {
                dealersHand += c.value + " - ";
            }

            string result = BlackJack.getGameResult(dealer.getHand(), players[0].getHand(0)).ToString();
            System.Windows.MessageBox.Show("Players: " + playersHand + "\n\n" +
                "Dealers: " + dealersHand + "\n\n" + "Result: " + result);
        }

        //  Gives the number of expected hands per hour, given the number of players at the blackjack table.
        //  Information from "http://wizardofodds.com/ask-the-wizard/136/"
        public static int getHandsPerHour(int numPlayers)
        {
            int[] handsPerHour = new int[7] { 209, 139, 105, 84, 70, 60, 52 };
            if ((numPlayers > 7) | (numPlayers < 1))
            {
                throw new Exception("Must specify a valid number of players.");
            }

            return handsPerHour[numPlayers - 1];

        }
        
        //  Inverse of getHandsPerHour, gets Hours given x hands
        public static Double getTimeFromGamesPlayed(int numPlayers, int gamesPlayed)
        {
            int handsPerHour = getHandsPerHour(numPlayers);
            //  Hands / (Hand / Hour) =  Hands*(Hour/Hand) = Hours
            return gamesPlayed / handsPerHour;
        }
        public static BlackJack_Card convertCardToBlackJack(Card card)
        {
            switch (card.value)
            {
                case CardValue.Ace:
                    return new BlackJack_Card(BlackJack_CardValue.Ace);
                case CardValue.Two:
                    return new BlackJack_Card(BlackJack_CardValue.Two);
                case CardValue.Three:
                    return new BlackJack_Card(BlackJack_CardValue.Three);
                case CardValue.Four:
                    return new BlackJack_Card(BlackJack_CardValue.Four);
                case CardValue.Five:
                    return new BlackJack_Card(BlackJack_CardValue.Five);
                case CardValue.Six:
                    return new BlackJack_Card(BlackJack_CardValue.Six);
                case CardValue.Seven:
                    return new BlackJack_Card(BlackJack_CardValue.Seven);
                case CardValue.Eight:
                    return new BlackJack_Card(BlackJack_CardValue.Eight);
                case CardValue.Nine:
                    return new BlackJack_Card(BlackJack_CardValue.Nine);
                case CardValue.Ten:
                    return new BlackJack_Card(BlackJack_CardValue.Ten);
                case CardValue.Jack:
                    return new BlackJack_Card(BlackJack_CardValue.Ten);
                case CardValue.Queen:
                    return new BlackJack_Card(BlackJack_CardValue.Ten);
                case CardValue.King:
                    return new BlackJack_Card(BlackJack_CardValue.Ten);
                default:
                    throw new Exception("Invalid BlackJack Card");
            }
        }

        public Player_Statistics start()
        {
            try
            {
                DateTime startTime = DateTime.Now;
                Player_Statistics ps = new Player_Statistics();
                for (int x = 0; x < this.iterationsOfEntireDeck; x++)
                {
                    TimeSpan ts = DateTime.Now - startTime;
                    double minsRemaining = (ts.TotalMinutes / (double)x) * (double)(this.iterationsOfEntireDeck - x);
                    object[] obj = new object[3];
                    obj[0] = this.iterationsOfEntireDeck;
                    obj[1] = x;
                    obj[2] = minsRemaining;
                    this.progressUpdateCodeDelegate.DynamicInvoke((Object)obj);
                    deck.shuffle();
                    while (deck.DeckList.Count >= 15)
                    {
                        dealer.dealCards(players);
                        for (int y = 0; y < players.Count; y++)
                        {
                            players[y].playHand(dealer, players, ps);
                        }
                        dealer.drawTo17();
                        //showGameInfo();
                        dealer.getResults(players, ps);
                        dealer.clearTable(players);
                    }
                }
                ps.generateReport(this.outputFile);
                return ps;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return null;
            }
        }
        public static bool doesHandHaveBlackjack(List<Card> hand)
        {
            //  Check the hand for a BlackJack
            if ((((convertCardToBlackJack(hand[0]).value == BlackJack_CardValue.Ace) &
                (convertCardToBlackJack(hand[1]).value == BlackJack_CardValue.Ten)) |
                    ((convertCardToBlackJack(hand[1]).value == BlackJack_CardValue.Ace) &
                    (convertCardToBlackJack(hand[0]).value == BlackJack_CardValue.Ten))) & (hand.Count == 2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static BlackjackResult getGameResult(Card dealer1, Card dealer2, Card player1, Card player2, Card player3)
        {
            List<Card> playersCards = new List<Card>();
            List<Card> dealersCards = new List<Card>();
            playersCards.Add(player1);
            playersCards.Add(player2);
            playersCards.Add(player3);
            dealersCards.Add(dealer1);
            dealersCards.Add(dealer2);
            int playerHand = getHandValue(playersCards);
            int dealerHand = getHandValue(dealersCards);
            if (playerHand > 21) return BlackjackResult.DealerWins;
            if (dealerHand > 21) return BlackjackResult.PlayerWins;
            if (dealerHand > playerHand)
            { return BlackjackResult.DealerWins; }
            else
            {
                if (dealerHand == playerHand)
                {
                    //  Code for dealer peak.  When the dealer has a natural blackjack, players lose unless they
                    //  too have a natural blackjack.
                    if (doesHandHaveBlackjack(dealersCards) & doesHandHaveBlackjack(playersCards))
                    {
                        return BlackjackResult.Push;
                    }
                    else
                    {
                        if (doesHandHaveBlackjack(dealersCards))
                        {
                            return BlackjackResult.DealerWins;
                        }

                        if (doesHandHaveBlackjack(playersCards))
                        {
                            return BlackjackResult.PlayerWins;
                        }
                    }
                    return BlackjackResult.Push;
                }
                else
                {
                    return BlackjackResult.PlayerWins;
                }
            }
        }

        public static BlackjackResult getGameResult(Card dealer1, Card dealer2, Card player1, Card player2)
        {
            List<Card> playersCards = new List<Card>();
            List<Card> dealersCards = new List<Card>();
            playersCards.Add(player1);
            playersCards.Add(player2);
            dealersCards.Add(dealer1);
            dealersCards.Add(dealer2);
            int playerHand = getHandValue(playersCards);
            int dealerHand = getHandValue(dealersCards);
            if (playerHand > 21) return BlackjackResult.DealerWins;
            if (dealerHand > 21) return BlackjackResult.PlayerWins;
            if (dealerHand > playerHand)
            { return BlackjackResult.DealerWins; }
            else
            {
                if (dealerHand == playerHand)
                {
                    //  Code for dealer peak.  When the dealer has a natural blackjack, players lose unless they
                    //  too have a natural blackjack.
                    if (doesHandHaveBlackjack(dealersCards) & doesHandHaveBlackjack(playersCards))
                    {
                        return BlackjackResult.Push;
                    }
                    else
                    {
                        if (doesHandHaveBlackjack(dealersCards))
                        {
                            return BlackjackResult.DealerWins;
                        }

                        if (doesHandHaveBlackjack(playersCards))
                        {
                            return BlackjackResult.PlayerWins;
                        }
                    }
                    return BlackjackResult.Push;
                }
                else
                {
                    return BlackjackResult.PlayerWins;
                }
            }
        }

        public static BlackjackResult getGameResult(List<Card> dealersCards, List<Card> playersCards)
        {
            int playerHand = getHandValue(playersCards);
            int dealerHand = getHandValue(dealersCards);
            if (playerHand > 21) return BlackjackResult.DealerWins;
            if (dealerHand > 21) return BlackjackResult.PlayerWins;
            if (dealerHand > playerHand)
            { return BlackjackResult.DealerWins; }
            else
            {
                if (dealerHand == playerHand)
                {
                    //  Code for dealer peak.  When the dealer has a natural blackjack, players lose unless they
                    //  too have a natural blackjack.
                    if (doesHandHaveBlackjack(dealersCards) & doesHandHaveBlackjack(playersCards))
                    {
                        return BlackjackResult.Push;
                    }
                    else
                    {
                        if (doesHandHaveBlackjack(dealersCards))
                        {
                            return BlackjackResult.DealerWins;
                        }

                        if (doesHandHaveBlackjack(playersCards))
                        {
                            return BlackjackResult.PlayerWins;
                        }
                    }
                    return BlackjackResult.Push;
                }
                else
                {
                    return BlackjackResult.PlayerWins;
                }
            }
        }

        //  Always treats aces as '1'
        public static int getHandValue_Lower(List<Card> handCards)
        {
            //  Do this so the values of the 'handCards' list won't be changed.
            List<Card> hand = new List<Card>(handCards);

            int handValue = 0;
            while (hand.Count > 0)
            {
                if (hand[0].value == CardValue.Ace)
                {
                    handValue += 1;
                }
                else
                {
                    handValue = handValue += (int)BlackJack.convertCardToBlackJack(hand[0]).value;
                }
                hand.RemoveAt(0);
            }

            return handValue;
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
                while ((handValue > 21) & (aces > 0))
                {
                    handValue += -10;
                    aces += -1;
                }
            }

            return handValue;
        }

    }
    public enum BlackjackResult
    {
        DealerWins = 0, PlayerWins = 1, Push = 2
    }
}