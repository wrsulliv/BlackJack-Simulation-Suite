using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApplication1
{
    class CardCounting
    {
        private int runningCount = 0;
        private BlackJackGameParams blackJackGameParams;
        public CardCounting(BlackJackGameParams blackJackGameParams)
        {
            this.blackJackGameParams = blackJackGameParams;
            reset();
        }
        
        //  Resets the count
        public void reset()
        {
            this.runningCount = getIRC(blackJackGameParams);
        }

        //  Used to determine the bet multiple using the systems betting ramp
        public int calculateBetMultiple()
        {

            if (blackJackGameParams.countingMethod == CountingMethod.KO)
            {
                if (blackJackGameParams.numDecks == 6)
                {
                    //	<=-4	-3	-2	-1	0	1	2	3	4
                    //Bet	5	10	10	25	25	25	50	50	75
                    if (this.runningCount <= -4) return 1;
                    else if (this.runningCount == -3) return 2;
                    else if (this.runningCount == -2) return 2;
                    else if (this.runningCount == -1) return 5;
                    else if (this.runningCount == 0) return 5;
                    else if (this.runningCount == 1) return 5;
                    else if (this.runningCount == 2) return 10;
                    else if (this.runningCount == 3) return 10;
                    else return 15;
                }
            }
            else if (blackJackGameParams.countingMethod == CountingMethod.NONE)
            {
                return 1;
            }

            //  Catch All
            throw new Exception("Could not detremine a valid bet multiple");
        }
        //  Used to count the value of a card
        public void countCard(Card newCard)
        {
            if (blackJackGameParams.countingMethod == CountingMethod.KO)
            {
                int blackJackCardValue = (int)BlackJack.convertCardToBlackJack(newCard).value;
                if (blackJackCardValue >= 10)
                { 
                    runningCount -= 1;
                }
                else if (blackJackCardValue <= 7)
                {
                    runningCount += 1;
                }
                else
                { 
                    //  Do not add or remove anything
                }

                //  Code for debugging the cardCounter - PASSED
                //System.Diagnostics.Debug.WriteLine("Card:" + newCard.value.ToString());
                //System.Diagnostics.Debug.WriteLine("Count:" + runningCount.ToString() + "\n");

            }
            else
            { 
            
            }
        }

        public static int getIRC(BlackJackGameParams blackJackGameParams)
        {
            if (blackJackGameParams.countingMethod == CountingMethod.KO)
            {

                int IRC = getCardCountingValueArray_KO()[blackJackGameParams.numDecks].IRC;
                return IRC;
            }
            else
            {
                return -1;
            }
        }

        //  Shows the player whether or not to bet high - PASSED
        public bool shouldPlayerBetHigh()
        {
            if (blackJackGameParams.countingMethod == CountingMethod.KO)
            {

                if (runningCount >= getKeyCount(blackJackGameParams))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public int getCount(BlackJackGameParams blackJackGameParams)
        {
            if (blackJackGameParams.countingMethod == CountingMethod.KO)
            {

                return runningCount;
            }
            else
            {
                return -1;
            }
        }
        public static int getKeyCount(BlackJackGameParams blackJackGameParams)
        {
            if (blackJackGameParams.countingMethod == CountingMethod.KO)
            {

                return getCardCountingValueArray_KO()[blackJackGameParams.numDecks].KeyCount;
            }
            else
            {
                return -1;
            }
        }

        //  Initializes the arrays which hold the IRC, and Key Counts of different counting methods
        private static CardCoutingValues[]  getCardCountingValueArray_KO()
        {
            CardCoutingValues[] KO_CardCountingValues = new CardCoutingValues[10];  // 1,2,6,8 decks

            //  These values from http://koblackjack.com  
            KO_CardCountingValues[1] = new CardCoutingValues(0, 2);
            KO_CardCountingValues[2] = new CardCoutingValues(-4, 1);
            KO_CardCountingValues[6] = new CardCoutingValues(-20, -4);
            KO_CardCountingValues[8] = new CardCoutingValues(-28, -6);

            return KO_CardCountingValues;
        }

        //  Include IRC and KeyCount
        struct CardCoutingValues
        {
            public CardCoutingValues(int IRC, int KeyCount)
            {
                this.IRC = IRC;
                this.KeyCount = KeyCount;
            }
            public int IRC;
            public int KeyCount;
        }


    }
}
