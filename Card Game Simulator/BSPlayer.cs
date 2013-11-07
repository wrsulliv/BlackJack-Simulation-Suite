using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApplication1
{
    class BSPlayer
    {

        //  Holds the players money
        Double BankRoll = 0;

        //  Variable holding the players initial bet
        Double initialBet = 0;

        //  Variable to hold the players most recent win
        List<Double> playerWin =new List<double>();

        //  The players current bet
        List<Double> currentBet = new List<double>();

        //  Tells whether that player is still playing
        public bool hasQuit = false;

        //  Holds the Basic Strategy Calculator Instance
        BasicStrategyCalculator bsc;

        //  Holds the players hands, the '0' index is for the initial hand, others are split hands
        private List<List<Card>> cardList;
        public BSPlayer(BlackJackGameParams blackJackGameParams)
        {
            this.cardList = new List<List<Card>>();
            this.cardList.Add(new List<Card>());
            this.bsc = new BasicStrategyCalculator(blackJackGameParams);
        }

        //  Used to get initial bet
        public Double getInitialBet()
        {
            return this.initialBet;
        }

        //  Used to give the player money when he/she wins blackjack
        public void winBlackJack(int listIndex)
        {
            playerWin[listIndex] = currentBet[listIndex] * (Double)1.5;
            BankRoll += currentBet[listIndex] + playerWin[listIndex];
        }

        //  Used to give the player money when he/she wins normally
        public void winNormalBet(int listIndex)
        {
            playerWin[listIndex] = currentBet[listIndex];
            BankRoll += currentBet[listIndex] + playerWin[listIndex];
        }

        //  Used to retrieve the individual hands winnings from the current game
        public Double getHandWinnings(int listIndex)
        {
            return playerWin[listIndex];
        }

        //  Used to retrieve all the players winnings from the current game
        public Double getTotalHandWinnings()
        {
            Double total = 0;
            foreach (Double d in playerWin)
            {
                total += d;
            }

            return total;
        }

        //  Used to take the players money when he/she loses
        public void loseBet(int listIndex)
        {
            //  Do nothing, as the money is already taken
            playerWin[listIndex] = -currentBet[listIndex];
        }

        //  Used to give the players bet back to them if they push
        public void pushBet(int listIndex)
        {
            playerWin[listIndex] = 0;
            BankRoll += currentBet[listIndex];
        }

        //  Used to refresh the current bets back to their original state
        public void clearCurrentBets()
        {
            currentBet = new List<double>();
            playerWin = new List<double>();
            currentBet.Add(0);
            playerWin.Add(0);
        }

        //  Get the Bankroll
        public Double getBankroll()
        {
            return this.BankRoll;
        }

        //  Set the BankRoll
        public void setBankRoll(Double bankRoll)
        {
            this.BankRoll = bankRoll;
        }
        //  Used to tell how many hands the player currently has (For Splitting)
        public int getHandCount()
        {
            return cardList.Count;
        }

        //  Used by the dealer class to give the Player a card
        public void takeCard(Card card)
        {
            this.cardList[0].Add(card);
        }

        //  Used by the dealer class to clear the players hand
        public void clearHand()
        {
            this.cardList = new List<List<Card>>();
            this.cardList.Add(new List<Card>());
        }

        //  For testing only, for setting the players hand explicitly
        public void setFirstHand(List<Card> playerHand)
        {
            this.cardList[0] = playerHand;
        }

        public void playHand(BSDealer dealer, CardCounting cardCounter)
        {
            //  We know the dealer doesn't have blackjack because this is tested before this portion of the game even occurs

            Stack<Card> splitCards = new Stack<Card>(); //  Make a stack for holding the split cards
            int processesListCount = 0; //  Keeps track of how many lists have been processed (Also reports the current lists index

            //  While there are cards in the split pile, or their are unprocessed lists...
            while ((splitCards.Count > 0) | (processesListCount < this.cardList.Count))
            {
                //  If we have the same number of lists as we have processed, we must have to handle another split
                if (this.cardList.Count == processesListCount)
                {
                    //  Add the split card to the new list 
                    this.cardList.Add(new List<Card>());

                    //  Add a new wager to the new hand
                    this.currentBet.Add(this.initialBet);
                    this.playerWin.Add(0);

                    //  Add the card to the new hand
                    this.cardList[processesListCount].Add(splitCards.Pop());
                }

                //  Check if the hand has at least two cards, if it does'nt, then add one.  This happens when we split
                if (this.cardList[processesListCount].Count == 1)
                {
                    this.cardList[processesListCount].Add(dealer.takeHit());
                }

                //  Check if the higher, and lower values are both busted.  If so then we've busted this hand so move to the next.
                if ((BlackJack.getHandValue_Lower(this.cardList[processesListCount]) > 21) & (BlackJack.getHandValue_Lower(this.cardList[processesListCount]) > 21))
                {
                    processesListCount += 1;
                }
                else
                {

                    //  Get the Basic Strategy move for the current card list
                    BlackJackMove bjm = new BlackJackMove();

                    //  If the player has doubles, but not enough money to split, then find an alternative move
                    if (hasDoublesInHand(processesListCount) & hasQuit)
                    {
                        bjm = bsc.GetStrategyPairToNoPair(this.cardList[processesListCount], dealer.getUpCard());
                    }
                    else
                    {
                        bjm = bsc.GetStrategy(this.cardList[processesListCount], dealer.getUpCard());
                    }
                    switch (bjm)
                    {
                        case BlackJackMove.Stay:
                            processesListCount += 1;
                            break;
                        case BlackJackMove.Hit:
                            this.cardList[processesListCount].Add(dealer.takeHit()); //  Add a card but don't finish processing this hand
                            break;
                        case BlackJackMove.Double:
                            if (this.cardList[processesListCount].Count == 2)
                            {
                                //  If we have enough money to double then do it, otherwise leave the table after the end of the round
                                if ((BankRoll - currentBet[processesListCount]) > 0)
                                {
                                    this.cardList[processesListCount].Add(dealer.takeHit());
                                    this.BankRoll -= this.currentBet[processesListCount];
                                    this.currentBet[processesListCount] = 2 * this.currentBet[processesListCount];
                                    processesListCount += 1;
                                    break;
                                }
                                else
                                {
                                    //  Take a hit if we can't double due to money, and mark this as our last round to minimize losses
                                    hasQuit = true;
                                    this.cardList[processesListCount].Add(dealer.takeHit());
                                    break;
                                }
                            }
                            else
                            {
                                this.cardList[processesListCount].Add(dealer.takeHit()); //  Add a card but don't finish processing this hand
                                break;
                            }
                        case BlackJackMove.Double_Stay:
                            //  If we haven't hit this hand yet then double, otherwise stay
                            if (this.cardList[processesListCount].Count == 2)
                            {
                                //  If we have enough money to double then do it, otherwise leave the table.
                                if ((BankRoll - currentBet[processesListCount]) > 0)
                                {
                                    this.cardList[processesListCount].Add(dealer.takeHit());
                                    this.BankRoll -= this.currentBet[processesListCount];
                                    this.currentBet[processesListCount] = 2 * this.currentBet[processesListCount];
                                    processesListCount += 1;
                                    break;
                                }
                                else
                                {
                                    //  Stay if we can't double due to money, and mark this as our last round to minimize losses
                                    hasQuit = true;
                                    processesListCount += 1;
                                    break;
                                }
                            }
                            else
                            {
                                processesListCount += 1;
                                break;
                            }
                        case BlackJackMove.Split:
                            //  Put the card from index 0 of the current hand into the stack.
                            //  Now, don't increment the processListCount, and reprocesses this hand.
                            //  It will only have one card so we will have to add that first.
                            if (((BankRoll - this.initialBet) > 0) & (cardList.Count < 4))
                            {
                                BankRoll -= this.initialBet;
                                splitCards.Push(this.cardList[processesListCount][1]);
                                this.cardList[processesListCount].RemoveAt(1);
                                break;
                            }
                            else
                            {
                                //  If we can't split due to money, then  re-evaluate as a hard/soft hand and determine the new best move, mark 
                                //  as the last turn to minimize losses
                                hasQuit = true;
                                break;
                            }
                    }
                }
            }
            //  Even odds paid on split card blackjack
        }

        //  Used for debugging
        public void setInitialBet(Double initialBet)
        {
            currentBet[0] = initialBet;
            this.initialBet = initialBet;
            BankRoll -= initialBet;
        }
        //  Determines the players initial bet, and puts it in the "currentBet" variable
        public void PlaceInitialBet(BlackJackGameParams blackJackGameParams, CardCounting cardCounter)
        {
            if (blackJackGameParams.useBetRamp)
            {
                int betMultiple = cardCounter.calculateBetMultiple();
                initialBet = betMultiple * blackJackGameParams.minBet;
                currentBet[0] = initialBet;
                BankRoll -= initialBet;
            }
            else
            {
                //  Code to determine what the player will bet
                if (cardCounter.shouldPlayerBetHigh())
                {
                    Double highBet = blackJackGameParams.minBet * blackJackGameParams.betSpread;
                    if ((BankRoll - highBet) > 0)
                    {
                        initialBet = highBet;
                        currentBet[0] = highBet;
                        BankRoll -= highBet;
                    }
                    else
                    {
                        hasQuit = true;
                    }
                }
                else
                {
                    if ((BankRoll - blackJackGameParams.minBet) > 0)
                    {
                        initialBet = blackJackGameParams.minBet;
                        currentBet[0] = blackJackGameParams.minBet;
                        BankRoll -= blackJackGameParams.minBet;
                    }
                    else
                    {
                        hasQuit = true;
                    }
                }
            }

            if (currentBet[0] > blackJackGameParams.maxBet)
            {
                throw new Exception("Cannot bet higher than the maximum bet");
            }

        }
        public List<Card> getHand(int splitIndex)
        {
            return this.cardList[splitIndex];
        }

        //  The initial hand includes the first two cards
        //  Should be extended later to check for splits. thats why we use 0 index.
        public Boolean hasAceInInitalHand()
        {
            for (int x = 0; x < 2; x++ )
            {
                if (this.cardList[0][x].value == CardValue.Ace)
                {
                    return true;
                }
            }
            return false;
        }

        //  The initial hand includes the first two cards 
        public Boolean hasDoublesInHand(int listIndex)
        {
            if ((BlackJack.convertCardToBlackJack(this.cardList[listIndex][0]).value == BlackJack.convertCardToBlackJack(this.cardList[listIndex][1]).value) & (cardList[listIndex].Count == 2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
