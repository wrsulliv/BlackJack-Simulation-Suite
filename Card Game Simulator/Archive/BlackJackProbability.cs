using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApplication1
{
    class BlackJackProbability
    {
        //  Only for Hard Hands for now
        Double masterWinProbSum = 1;
        Double masterPushProbSum = 1;
        Double masterLoseProbSum = 1;

        int winCount = 0;
        int pushCount = 0;
        int bustCount = 0;

        List<Card> simpleCardList = new List<Card>();
        int currentHitCount = 0;
        List<Double> EV_List = new List<Double>();
        public BlackJackProbability()
        {
            simpleCardList = new List<Card>();
            simpleCardList.Add(new Card(CardValue.Ace, CardSuit.Hearts));
            simpleCardList.Add(new Card(CardValue.Two, CardSuit.Hearts));
            simpleCardList.Add(new Card(CardValue.Three, CardSuit.Hearts));
            simpleCardList.Add(new Card(CardValue.Four, CardSuit.Hearts));
            simpleCardList.Add(new Card(CardValue.Five, CardSuit.Hearts));
            simpleCardList.Add(new Card(CardValue.Six, CardSuit.Hearts));
            simpleCardList.Add(new Card(CardValue.Seven, CardSuit.Hearts));
            simpleCardList.Add(new Card(CardValue.Eight, CardSuit.Hearts));
            simpleCardList.Add(new Card(CardValue.Nine, CardSuit.Hearts));
            simpleCardList.Add(new Card(CardValue.Ten, CardSuit.Hearts));
        }
        public string ComputeStrategy(Card player1, Card player2, Card dealersUp, int deckCount)
        {
            List<Card> playersHand = new List<Card>();
            playersHand.Add(player1); playersHand.Add(player2);

            List<Card> dealersCards = new List<Card>();
            dealersCards.Add(dealersUp);

            //  Increase the number of cards in the players hand unless the percentage of losing is greater or equal to 80%, and we've already hit at least 3 times.
            //  In this case, even is the player started with a 2 and a 2, then with an average card value of 7, they should be above 21 fairly quick
            while(!((1 - masterPushProbSum - masterWinProbSum >= .8) & (currentHitCount > 2)))
            {
                masterWinProbSum = 0;
                masterPushProbSum = 0;
                masterLoseProbSum = 0;

                winCount = 0;
                pushCount = 0;
                bustCount = 0;
                ComputeEV_List_Recursive(playersHand, dealersCards, 1);
                //  If you lose 4/10 times and win 3/10 times, then you tie 3/10 times.
                //  That means that when betting one unit, you will gain a unit 3/10 times, lose one 4/10 times, and keep the same 3/10 times
                //  To determine actual money gained, take the number of wins, subtract losses, which cancles the money you've wagered / loss, and 
                //  you will see the number of profitable games.  If this number is positive, you've gained money, if it's negative, then
                //  you've lost money.
              // System.Windows.MessageBox.Show((masterLoseProbSum + masterPushProbSum + masterWinProbSum).ToString());
               EV_List.Add(masterWinProbSum - masterLoseProbSum);
               return EV_List[0].ToString();
                currentHitCount += 1;
            }

            //  Determine if it's better to hit or stay.
            Double staySum = EV_List[0];
            Double hitSum = EV_List[0];
            for (int x = 1; x < EV_List.Count; x++)
            {
                if (EV_List[x] > hitSum)
                {
                    hitSum = EV_List[x];
                }
            }

            if (hitSum > staySum)
            {
                //  If hitting only one card is the best option, then double
                if (EV_List[1] >= hitSum)
                {
                    if (EV_List[1] >= 0)
                    {
                        //System.Windows.MessageBox.Show("Double");
                        EV_List[1] = EV_List[1] * 2; //  Since we're doubling, you will recieve 2 times the expected return
                        return "D";
                    }
                    else
                    {
                        //System.Windows.MessageBox.Show("Hit");
                        return "H";
                    }
                }
                else
                {
                    //System.Windows.MessageBox.Show("Hit");
                    return "H";
                }
            }
            else
            {
                //System.Windows.MessageBox.Show("Stay");
                return "S";
            }

            string allPossible = "";
            foreach (Double d in EV_List)
            {
                allPossible += d.ToString() + "\n";
            }

            System.Windows.MessageBox.Show(allPossible);
           
        }

        public void ComputeEV_List_Recursive(List<Card> playersHand, List<Card> dealersCards, Double previousProb)
        {
            foreach (Card c in simpleCardList)
            {
                //  Check to see if the current card is still in the deck.  If not, then skip it.
                // if (!(hasCardRunOut(playersHand, dealersCards, c, deckCount)))
                // {
                //  Check if the players hand has the right number of cards
                if (playersHand.Count < currentHitCount + 2)
                {
                    List<Card> playersHand_clone = new List<Card>(playersHand); //  Clone playersHand so can be passed "ByVal"
                    playersHand_clone.Add(c);
                    Double currentProb = getInfiniteDeckProb(c);
                    if (BlackJack.getHandValue(playersHand_clone) <= 21)
                    {
                        ComputeEV_List_Recursive(playersHand_clone, dealersCards, previousProb * currentProb);
                    }
                    else
                    {
                        //  Since the hand is over 21, don't bother checking the dealer combinations as they will always win, even when they bust.
                        masterLoseProbSum += currentProb * previousProb;
                    }
                }
                else //  Player hand has all cards, so move onto dealer
                {
                    List<Card> dealersCards_clone = new List<Card>(dealersCards); //  Clone playersHand so can be passed "ByVal"
                    dealersCards_clone.Add(c);
                    if (BlackJack.getHandValue(dealersCards_clone) > 16) // TODO:  only S17
                    {
                        BlackjackResult bjr = BlackJack.getGameResult(dealersCards_clone, playersHand);
                        //  A push or a win is a win
                        if (bjr == BlackjackResult.PlayerWins)
                        {
                            masterWinProbSum = masterWinProbSum + (previousProb * getInfiniteDeckProb(c, dealersCards));
                            winCount += 1;
                        }
                        else if (bjr == BlackjackResult.Push)
                        {
                            masterPushProbSum = masterPushProbSum + (previousProb * getInfiniteDeckProb(c, dealersCards));
                            pushCount += 1;
                        }
                        else
                        {
                            masterLoseProbSum = masterLoseProbSum + (previousProb * getInfiniteDeckProb(c, dealersCards));
                            bustCount += 1;
                        }
                    }
                    else
                    {
                        Double curentProb = getInfiniteDeckProb(c, dealersCards);
                        ComputeEV_List_Recursive(playersHand, dealersCards_clone, previousProb * curentProb);
                    }
                }
                //}

            }

        }


        public void RecursiveComputer_Hit(List<Card> playersHand, List<Card> dealersCards, Double previousProb, int deckCount)
        {
            foreach (Card c in simpleCardList)
            {
                //  Check to see if the current card is still in the deck.  If not, then skip it.
                if (!(hasCardRunOut(playersHand, dealersCards, c, deckCount)))
                {
                    if (playersHand.Count < 3)
                    {
                        List<Card> playersHand_clone = new List<Card>(playersHand); //  Clone playersHand so can be passed "ByVal"
                        playersHand_clone.Add(c);
                        Double curentProb = getProb(playersHand, dealersCards, c, deckCount);
                        RecursiveComputer_Hit(playersHand_clone, dealersCards, previousProb * curentProb, deckCount);
                    }
                    else //  Player hand has all cards, so move onto dealer
                    {
                        List<Card> dealersCards_clone = new List<Card>(dealersCards); //  Clone playersHand so can be passed "ByVal"
                        dealersCards_clone.Add(c);
                        if (BlackJack.getHandValue(dealersCards_clone) > 16) // TODO:  only S17
                        {
                            BlackjackResult bjr = BlackJack.getGameResult(dealersCards_clone, playersHand);
                            //  A push or a win is a win
                            if (bjr == BlackjackResult.PlayerWins)
                            {
                                masterWinProbSum = masterWinProbSum + (previousProb * getProb(playersHand, dealersCards, c, deckCount));
                            }
                            else if (bjr == BlackjackResult.Push)
                            {
                                masterPushProbSum = masterPushProbSum + (previousProb * getProb(playersHand, dealersCards, c, deckCount));
                            }
                        }
                        else
                        {
                            Double curentProb = getProb(playersHand, dealersCards, c, deckCount);
                            RecursiveComputer_Hit(playersHand, dealersCards_clone, previousProb * curentProb, deckCount);
                        }
                    }
                }
            }

        }

        public void RecursiveComputer_Stay(List<Card> playersHand, List<Card> dealersCards, Double previousProb, int deckCount)
        {
            foreach (Card c in simpleCardList)
            {
                //  Check to see if the current card is still in the deck.  If not, then skip it.
                if (!(hasCardRunOut(playersHand, dealersCards, c, deckCount)))
                {
                    List<Card> dealersCards_clone = new List<Card>(dealersCards); //  Clone playersHand so can be passed "ByVal"
                    dealersCards_clone.Add(c);
                    if (BlackJack.getHandValue(dealersCards_clone) > 16) // TODO:  only S17
                    {
                        BlackjackResult bjr = BlackJack.getGameResult(dealersCards_clone, playersHand);
                        //  A push or a win is a win
                        if (bjr == BlackjackResult.PlayerWins)
                        {
                            masterWinProbSum = masterWinProbSum + (previousProb * getProb(playersHand, dealersCards, c, deckCount));
                        }
                        else if (bjr == BlackjackResult.Push)
                        {
                            masterPushProbSum = masterPushProbSum + (previousProb * getProb(playersHand, dealersCards, c, deckCount));
                        }
                    }
                    else
                    {
                        Double curentProb = getProb(playersHand, dealersCards, c, deckCount);
                        RecursiveComputer_Stay(playersHand, dealersCards_clone, previousProb * curentProb, deckCount);
                    }
                }
            }

        }

        //  Reports whether the specified card still remains in the deck
        bool hasCardRunOut(List<Card> playersHand, List<Card> dealersCards, Card c, int deckCount)
        {
            //  Stores the number of cards that have been taken from the deck
            int[] countArray = new int[10];

            //  Fill countArray from playersHand
            foreach (Card player_c in playersHand)
            {
                if ((int)player_c.value >= 10)
                {
                    countArray[9] += 1;
                }
                else
                {
                    countArray[((int)player_c.value) - 1] += 1;
                }
            }

            //  Fill countArray from playersHand
            foreach (Card player_c in dealersCards)
            {
                if ((int)player_c.value >= 10)
                {
                    countArray[9] += 1;
                }
                else
                {
                    countArray[((int)player_c.value) - 1] += 1;
                }
            }

            //  Make an array that holds the total in the entire deck of the cards 0 = Ace, 9 = 10, J, Q, K
            int[] totalArray = new int[10];
            for (int x = 0; x < 9; x++)
            {
                totalArray[x] = deckCount * 4;
            }
            totalArray[9] = deckCount * 4 * 4; //  Includes 10, J, Q, K

            int totalCards = deckCount * 52;
            int cardIndex;

            //  Convert the Card c into an index used in out arrays
            if ((int)c.value > 9)
                cardIndex = 9;
            else
                cardIndex = (int)c.value - 1;

            int cardsLeft = totalArray[cardIndex] - countArray[cardIndex];
            if (cardsLeft > 0) { return false; } else { return true; }
        }

        private Double getInfiniteDeckProb(Card c)
        {
            if ((int)c.value > 9)
            {
                return (Double)16 / (Double)52;
            }
            else
            {
                return (Double)4 / (Double)52;
            }
        }

        //  Checks to give a different probability if the dealer starts with a 10, or an Ace, as the probability of getting blackjack is zero, as that is one of 
        //  our assumptions.
        private Double getInfiniteDeckProb(Card c, List<Card> dealersCards)
        {
            //  If the first dealer card is a ten, and he only has that one card then...
            if (((int)dealersCards[0].value > 9) & (dealersCards.Count == 1))
            {
                if ((int)c.value == 1)
                {
                    return 0;
                }
                else if (((int)c.value > 9))
                {
                    return (Double)16 / (Double)48; // Remove aces from the deck
                }
                else
                {
                    return (Double)4 / (Double)48;
                }
            }

            //  If the first dealer card is a ten, and he only has that one card then...
            if (((int)dealersCards[0].value == 1) & (dealersCards.Count == 1))
            {
                if (((int)c.value > 9))
                {
                    return 0; // Remove 10's from the deck
                }
                else
                {
                    return (Double)4 / (Double)36;
                }
            }

            if ((int)c.value > 9)
            {
                return (Double)16 / (Double)52;
            }
            else
            {
                return (Double)4 / (Double)52;
            }
        }
        //  Reports the probability as type Double of the given card c, when the players hand and dealers hands are removed from the deck
        private Double getProb(List<Card> playersHand, List<Card> dealersCards, Card c, int deckCount)
        {
            //  Stores the number of cards that have been taken from the deck
            int[] countArray = new int[10];

            //  Fill countArray from playersHand
            foreach(Card player_c in playersHand)
            {
                if ((int)player_c.value >= 10)
                {
                    countArray[9] += 1;
                }
                else
                {
                    countArray[((int)player_c.value) - 1] += 1;
                }
            }

            //  Fill countArray from playersHand
            foreach(Card player_c in dealersCards)
            {
                if ((int)player_c.value >= 10)
                {
                    countArray[9] += 1;
                }
                else
                {
                    countArray[((int)player_c.value) - 1] += 1;
                }
            }

            //  Make an array that holds the total in the entire deck of the cards 0 = Ace, 9 = 10, J, Q, K
            int[] totalArray = new int[10];
            for(int x = 0; x < 9; x++)
            {
                totalArray[x] = deckCount * 4;
            }
            totalArray[9] = deckCount * 4 * 4; //  Includes 10, J, Q, K

            int totalCards = deckCount * 52;
            int cardIndex;

            //  Convert the Card c into an index used in our arrays
            if ((int)c.value > 9)
            {
                cardIndex = 9;
                return (Double)16 / (Double)52;
            }
            else
            {
                cardIndex = (int)c.value - 1;
                return (Double)4 / (Double)52;
            }
            int totalCardsLeft = totalCards - dealersCards.Count - playersHand.Count;
            int cardsLeft_current = totalArray[cardIndex] - countArray[cardIndex];
           // return (double)cardsLeft_current / (double)totalCardsLeft;
           // return (Double)1 / (Double)totalCards;
        }
        public void PerformTests()
        {

            // Test with no cards used 
            if (hasCardRunOut(new List<Card>(), new List<Card>(), new Card(CardValue.Queen, CardSuit.Hearts), 1))
            {
                System.Windows.MessageBox.Show("FAILED! - 1");
            }
            else
            {
                System.Windows.MessageBox.Show("SUCCESS! - 1");
            }


            List<Card> testListDealer = new List<Card>();
            testListDealer.Add(new Card(CardValue.Three, CardSuit.Hearts));
            testListDealer.Add(new Card(CardValue.Three, CardSuit.Hearts));
            testListDealer.Add(new Card(CardValue.Three, CardSuit.Hearts));
            testListDealer.Add(new Card(CardValue.Three, CardSuit.Hearts));

            List<Card> testListPlayer = new List<Card>();
            testListPlayer.Add(new Card(CardValue.Six, CardSuit.Hearts));
            testListPlayer.Add(new Card(CardValue.Nine, CardSuit.Hearts));


            //  Test with 4 Three's used
            if (!hasCardRunOut(testListPlayer, testListDealer, new Card(CardValue.Three, CardSuit.Hearts), 1))
            {
                System.Windows.MessageBox.Show("FAILED! - 2");
            }
            else
            {
                System.Windows.MessageBox.Show("SUCCESS! - 2");
            }
        }
    }

}
