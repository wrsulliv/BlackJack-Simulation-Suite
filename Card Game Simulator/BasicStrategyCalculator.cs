using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WpfApplication1
{
    //  Purpose:  This class doens't actually do combinatorics to compute the Basic Strategy, but instead parses CSV files which 
    //  contain the strategy for different types of games.  The class also uses these files to determine the best move for a player.
    class BasicStrategyCalculator
    {
        //  Globals
        BlackJackMove[,] HardStrategy = new BlackJackMove[22, 12]; //  [Player, Dealer] player has 5 - 21, dealer has 2 - 11
        BlackJackMove[,] SoftStrategy = new BlackJackMove[22, 12];
        BlackJackMove[,] PairsStrategy = new BlackJackMove[22, 12];

        //  Initializes the class by parsing the CSV file
        public BasicStrategyCalculator(BlackJackGameParams blackJackGameParams)
        {
            //  Mohegan Sun 5$/10$ Table
            //  Check for H17, DAS - Allowed, RSA - Not Allowed, Surrender - Allowed
            //if ((blackJackGameParams.DAS) & (blackJackGameParams.H17) & (!blackJackGameParams.RSA) & (blackJackGameParams.Surrender))
            // {
            //  Found his helpful Code from StackOverflow --  Understand how this works exactly...
            //string path = AppDomain.CurrentDomain.BaseDirectory + "/test.csv";
            ParseCSV("D:/test.csv");
            //  }
        }


        //  Returns the best BlackJackMove from Basic Strategy for the given situation with dealer and players hands taken into account
        public BlackJackMove GetStrategy(List<Card> playersCards, Card dealersCard)
        {
            //  First check if the player has a Pair
            BlackJack_Card playerCard0 = BlackJack.convertCardToBlackJack(playersCards[0]);
            BlackJack_Card playerCard1 = BlackJack.convertCardToBlackJack(playersCards[1]);
            BlackJack_Card dealersCard0 = BlackJack.convertCardToBlackJack(dealersCard);
            //  We can only have a pair when the player has 2 cards only
            if ((playersCards.Count == 2) & ( playerCard0.value ==  playerCard1.value))
            {
                return PairsStrategy[(int)playerCard0.value, (int)dealersCard0.value];
            }

            //  Check if we have a soft hand:  The lower and upper values must be different, and the lower value must be less than 12 (2 - 11) otherwise
            //  it's actually a hard hand.  (Soft 12 or 22  is really just a hard 12)
            if ((BlackJack.getHandValue_Lower(playersCards) < BlackJack.getHandValue(playersCards)) & (BlackJack.getHandValue_Lower(playersCards) < 12))
            { 
                //  Example:  If the player has an (A,4) then we will pull from index [4, dealersCard].  The lowest hand value will be 5, so the index
                //  is LowestHandValue - 1.  For when the Lowest hand value is 2, we will choose index [11, dealersCard], which is case of (A,A)
                int lowestHand = BlackJack.getHandValue_Lower(playersCards);
                if (lowestHand == 2) //  Special case where we have an Ace, so we want to use the 11 index instead of 2.  Do this to keep consistent
                {
                    return SoftStrategy[11, (int)dealersCard0.value];
                }
                else
                {
                    return SoftStrategy[lowestHand - 1, (int)dealersCard0.value];
                }
            }

            //  Must be a Hard Hand
            return HardStrategy[(int)BlackJack.getHandValue(playersCards), (int)dealersCard0.value];
        }

        //  Returns the best BlackJackMove from Basic Strategy for the given situation, but must input a hand with a PAIR 
        //  Use this function when the player doesn't have enough to pay for a split, but must still make a 
        //  decision.
        public BlackJackMove GetStrategyPairToNoPair(List<Card> playersCards, Card dealersCard)
        {
            //  First check if the player has a Pair
            BlackJack_Card playerCard0 = BlackJack.convertCardToBlackJack(playersCards[0]);
            BlackJack_Card playerCard1 = BlackJack.convertCardToBlackJack(playersCards[1]);
            BlackJack_Card dealersCard0 = BlackJack.convertCardToBlackJack(dealersCard);
            if ((playersCards.Count == 2) & (playerCard0.value == playerCard1.value))
            {
                if (playerCard0.value == BlackJack_CardValue.Ace)
                {
                    return SoftStrategy[11, (int)dealersCard0.value];
                }
                else
                {
                    return HardStrategy[(int)playerCard0.value * 2, (int)dealersCard0.value];
                }
            }
            else
            {
                throw new Exception("Invalid use of function:  The function 'GetStrategyPairToNoPair'" +
                    "is only to be used when the player knowingly has a pair, and only a pair");
            }
        }
        //  Parse a CSV file with Basic Strategy Information, into a group of 3 arrays
        private void ParseCSV(string csvPath)
        {
            string part = "";
            int playerNumber = 0;
            var lines = File.ReadAllLines(csvPath).Select(a => a.Split(','));
            foreach (var l in lines)
            {
                int dealerNumber = 0;
                foreach (var peice in l)
                {
                    //  Check if we've moved onto a new portion of the csv file (There are 3)
                    if ((peice == "HARD") | (peice == "SOFT") | (peice == "PAIRS"))
                    {
                        part = peice;
                    }
                    else
                    {
                        Int16 tryParseTest = 0;
                        if (Int16.TryParse(peice, out tryParseTest))
                        {
                            playerNumber = tryParseTest;
                            dealerNumber = 2;
                        }
                        else
                        {
                            if (part == "HARD")
                            {
                                HardStrategy[playerNumber, dealerNumber] = CSVMoveToBlackJackMove(peice);
                            }
                            else if (part == "SOFT")
                            {
                                SoftStrategy[playerNumber, dealerNumber] = CSVMoveToBlackJackMove(peice);
                            }
                            else //  PAIRS
                            {
                                PairsStrategy[playerNumber, dealerNumber] = CSVMoveToBlackJackMove(peice);
                            }

                            dealerNumber += 1;
                        }
                    }
                }
            }

        }

        //  Converts a move from the CSV file (S, H, D, DS, P, etc...) to a BlackJackMove
        private BlackJackMove CSVMoveToBlackJackMove(string csvMove)
        {
            if (csvMove == "H")
            {
                return BlackJackMove.Hit;
            }
            else if (csvMove == "S")
            {
                return BlackJackMove.Stay;
            }
            else if (csvMove == "D")
            {
                return BlackJackMove.Double;
            }
            else if (csvMove == "DS")
            {
                return BlackJackMove.Double_Stay;
            }
            else //  Split
            {
                return BlackJackMove.Split;
            }
        }
    }
}
