using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace WpfApplication1
{
    //  Purpose:  This class is for simulating many blackjack games which follow the rules of the basic strategy.
    class BasicStrategySimulator
    {
        //  Globals
        BlackJackGameParams blackJackGameParams;
        CardCounting cardCounter;
        List<Double> bankRoll_vs_GamesPlayed = new List<double>();
        List<BSPlayer> players;
        Deck deck;
        BSDealer dealer;
        List<Double> playersFinalMoney;
        Double playersFinalMoneyAverage = 0;
        int numTrials = 0;

        //  Initializes the class with the game parameters
        public BasicStrategySimulator(BlackJackGameParams blackJackGameParams)
        {
            this.blackJackGameParams = blackJackGameParams;
            this.cardCounter = new CardCounting(blackJackGameParams);
            deck = new Deck(blackJackGameParams.numDecks, Deck.ShuffleType.Durstenfeld, cardCounter);
            players = new List<BSPlayer>();
            dealer = new BSDealer(blackJackGameParams.H17, deck);

            //  Initialize the players
            for (int i = 0; i < blackJackGameParams.numPlayers; i++)
            {
                BSPlayer newBSPlayer = new BSPlayer(blackJackGameParams);
                players.Add(newBSPlayer);
            }
        }

        //  Calculates the Standard Deviation
        public Double getFinalBankrollStandardDevation()
        {
            Double standardDev = 0;
            foreach (Double d in playersFinalMoney)
            {
                standardDev += (d - playersFinalMoneyAverage) * (d - playersFinalMoneyAverage);
            }

            standardDev = standardDev / this.numTrials;
            standardDev = Math.Sqrt(standardDev);
            return standardDev;
        }

        //  Returns the average of the numbers which are below the average
        public Double getAverageOfThoseBelowTheMean()
        { 
            Double belowAverage = 0;
            foreach (Double d in playersFinalMoney)
            {
                if (d < blackJackGameParams.bankroll)
                {
                    belowAverage += d;
                }
            } 

            return belowAverage / (getPercentageBelowAverage() * this.numTrials);
        }

        //  Returns the average of the numbers which are above the average
        public Double getAverageOfThoseAbove_EqualTheMean()
        {
            Double aboveAverage = 0;
            foreach (Double d in playersFinalMoney)
            {
                if (d >= blackJackGameParams.bankroll)
                {
                    aboveAverage += d;
                }
            }

            return aboveAverage / (getPercentageAbove_EqualAverage() * this.numTrials);
        }

        //  Returns the percentage of the time in which the games end below the average
        public Double getPercentageBelowAverage()
        {
            Double belowAverage = 0;
            foreach (Double d in playersFinalMoney)
            {
                if (d < blackJackGameParams.bankroll)
                {
                    belowAverage += 1;
                }
            }

            return (belowAverage) / (Double)this.numTrials;
        }

        //  Returns the percentage of the time in which the games end above the average
        public Double getPercentageAbove_EqualAverage()
        {
            Double aboveAverage = 0;
            foreach (Double d in playersFinalMoney)
            {
                if (d >= blackJackGameParams.bankroll)
                {
                    aboveAverage += 1;
                }
            }

            return (aboveAverage) / (Double)this.numTrials;
        }

        //  Returns the players final bankroll average
        public Double getFinalBankrollAverage()
        {
            return this.playersFinalMoneyAverage;
        }

        //  Begins the simulation
        public void BeginSimulation(int numTrials)
        {
            this.numTrials = numTrials;
            playersFinalMoney =  new List<double>();
            Double playersCumulativeBankroll = 0;
            for (int i = 0; i < numTrials; i++)
            {
                Double playersEndingBankRoll = PlayTimedGame();
                playersCumulativeBankroll += playersEndingBankRoll;
                playersFinalMoney.Add(playersEndingBankRoll);
            }
            playersFinalMoneyAverage = (playersCumulativeBankroll / (Double)numTrials);
        }

        //  Used by PlayTimedGame to process the players winnings
        public static void processPlayerWinnings(BSPlayer player, BSDealer dealer)
        {
            //  If this player is playing (do they have cards?)
            //  cant use "hasQuit", because that's set in the previous loop, and the player may still need to be processed
            if (player.getHand(0).Count > 0)
            {
                //  Loop through each hand the player has (Split Hands)
                for (int j = 0; j < player.getHandCount(); j++)
                {
                    BlackjackResult bjr = BlackJack.getGameResult(dealer.getHand(), player.getHand(j));

                    //  Give player the money / take it away
                    if (bjr == BlackjackResult.PlayerWins)
                    {
                        //  If the player wins, has only one hand and has blackjack then pay 3:2 instead of 1:1
                        if ((player.getHandCount() == 1) & (BlackJack.doesHandHaveBlackjack(player.getHand(j))))
                        {
                            player.winBlackJack(j);
                        }
                        else
                        {
                            player.winNormalBet(j);
                        }
                    }
                    else if (bjr == BlackjackResult.Push)
                    {
                        player.pushBet(j);
                    }
                    else // Dealer Wins
                    {
                        player.loseBet(j);
                    }
                }

            }
        }

        //  Used to display text for debugging the end of the game
        private void showGameEnd(List<BSPlayer> players, BSDealer dealer)
        {
            //  Loop through each player
            for (int i = 0; i < players.Count; i++)
            {
                //  Code for debgging the playHand function
                for (int x = 0; x < players[i].getHandCount(); x++)
                {
                    if (!players[i].hasQuit)
                    {
                        if (i == blackJackGameParams.tablePosition)
                        {
                            System.Diagnostics.Debug.WriteLine("Player Index: " + i.ToString());
                            System.Diagnostics.Debug.WriteLine("Bankroll: $" + players[i].getBankroll());
                            System.Diagnostics.Debug.WriteLine("Initial Bet: " + players[i].getInitialBet());
                            System.Diagnostics.Debug.WriteLine("List Index: " + x.ToString());
                            System.Diagnostics.Debug.Write("Dealer: ");
                            foreach (Card c in dealer.getHand())
                            {
                                System.Diagnostics.Debug.Write(c.value.ToString() + ", ");
                            }
                            System.Diagnostics.Debug.Write("\nCards: ");
                            foreach (Card c in players[i].getHand(x))
                            {
                                System.Diagnostics.Debug.Write(c.value.ToString() + ", ");
                            }
                            System.Diagnostics.Debug.WriteLine("\nWinner: " + BlackJack.getGameResult(dealer.getHand(), players[i].getHand(x)).ToString());
                            System.Diagnostics.Debug.WriteLine("Winnings: $" + players[i].getHandWinnings(x).ToString());
                            System.Diagnostics.Debug.WriteLine("Total: " + BlackJack.getHandValue(players[i].getHand(x)).ToString() + "\n");
                        }
                    }
                }
            }
        }

        //  Used to get the data for displaying the comparison chart
        public List<Double> getBankRoll_vs_GamesPlayed()
        {
            return bankRoll_vs_GamesPlayed;
        }

        //  Simulates gameplay for a specific amount of time, or until money runs out, returns the ammount of money left
        private Double PlayTimedGame()
        {
            //  Function Variables
            int gameCount = 0;

            //  Shuffle the deck
            deck.shuffle();
            cardCounter.reset();

            //  Reinitialize the players
            foreach (BSPlayer bsp in players)
            {
                bsp.setBankRoll(blackJackGameParams.bankroll); //  Set how much money each player starts with
                bsp.clearCurrentBets();
                bsp.hasQuit = false;
            }

            //  Clear the whole list
            this.bankRoll_vs_GamesPlayed = new List<double>();

            //  While we haven't finished playing...
            //REAL CODE: 
            while ((gameCount < (blackJackGameParams.hoursOfPlay * BlackJack.getHandsPerHour(blackJackGameParams.numPlayers))) & (!players[blackJackGameParams.tablePosition].hasQuit))
            {
   
                //  Have the player in question place a bet
                players[blackJackGameParams.tablePosition].PlaceInitialBet(blackJackGameParams, cardCounter);

                //  Give players their cards
                dealer.dealCards(players);

                //  Check for dealer and player blackjacks
                //  If the dealer has blackjack, loop through the players and determine wages.  Don't worry about insurance/even mmoney as 
                //  basic strategy says never take insurrance or even money
                if (!dealer.hasBlackJack())
                {

                    //  Let each player take their turn
                    for (int i = 0; i < players.Count; i++)
                    {
                        if (!players[i].hasQuit)
                        {
                            players[i].playHand(dealer, cardCounter);
                        }
                    }

                    //  Let the dealer draw until he gets H17 or S17 (Depending on parameters)
                    dealer.drawTo17(blackJackGameParams.H17);

                }

                //  Add the dealers hidden card so card counters can see
                cardCounter.countCard(dealer.getHiddenCard());

                //  Process player winnings but only for the player in question
                processPlayerWinnings(players[blackJackGameParams.tablePosition], dealer);

                //  Show debugging info
                //showGameEnd(players, dealer);

                // Add the specified players bankroll to the plot variable
                this.bankRoll_vs_GamesPlayed.Add(players[blackJackGameParams.tablePosition].getBankroll());

                //  Clear the players
                dealer.clearTable(players);


                //  Check if the cut card was hit (Penetration Percent)
                Double currentPenetrationPercent = deck.penetrationPercent();
                if (currentPenetrationPercent >= blackJackGameParams.deckPenetration)
                {
                    //  Shuffle the deck
                    deck.shuffle();

                    //  Print status message
                  //  System.Diagnostics.Debug.AutoFlush = true;
                  //  System.Diagnostics.Debug.WriteLine((Double)gameCount / ((Double)BlackJack.getHandsPerHour(blackJackGameParams.numPlayers) * blackJackGameParams.hoursOfPlay) + "% Complete");

                    //  Reset the Count
                    cardCounter.reset();
                }

                gameCount += 1;

            } //  End While Loop

            return players[blackJackGameParams.tablePosition].getBankroll();
        }

    }
}
