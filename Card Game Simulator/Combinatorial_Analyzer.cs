using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApplication1
{
    enum BlackJackMove
    { 
        Stay = 0, Hit = 1, Double = 2, Double_Stay = 3, Split = 4, Unknown = 5
    }
    struct BSInfo
    {
        public BSInfo(Double ev, BlackJackMove blackJackMove)
        {
            this.ev = ev;
            this.blackJackMove = blackJackMove;
        }
        public Double ev;
        public BlackJackMove blackJackMove;
    }
    class Combinatorial_Analyzer
    {


        public BSInfo[,] BSInfoArray_Hard = new BSInfo[11,22]; //  Hold [DealerCard][PlayerHandTotal]
        public BSInfo[,] BSInfoArray_Hard_HitOnly = new BSInfo[11, 22]; //  Hold [DealerCard][PlayerHandTotal]
        public BSInfo[,] BSInfoArray_Soft = new BSInfo[11, 22]; //  Hold [DealerCard][PlayerHandTotal]
        public BSInfo[,] BSInfoArray_Soft_HitOnly = new BSInfo[11, 22]; //  Hold [DealerCard][PlayerHandTotal]

        //  Only for Hard Hands for now
        Double masterWinProbSum = 1;
        Double masterPushProbSum = 1;
        Double masterLoseProbSum = 1;
        Double EV_Total = 0;


        List<Card> simpleCardList = new List<Card>();
        List<List<Card>> playersHand = new List<List<Card>>();

        //List<Double> EV_List_Hard = new List<Double>();
        //List<Double> EV_List_Soft = new List<Double>();
        public Combinatorial_Analyzer()
        {
            simpleCardList = new List<Card>();
            simpleCardList.Add(new Card(CardValue.Two, CardSuit.Hearts));
            simpleCardList.Add(new Card(CardValue.Three, CardSuit.Hearts));
            simpleCardList.Add(new Card(CardValue.Four, CardSuit.Hearts));
            simpleCardList.Add(new Card(CardValue.Five, CardSuit.Hearts));
            simpleCardList.Add(new Card(CardValue.Six, CardSuit.Hearts));
            simpleCardList.Add(new Card(CardValue.Seven, CardSuit.Hearts));
            simpleCardList.Add(new Card(CardValue.Eight, CardSuit.Hearts));
            simpleCardList.Add(new Card(CardValue.Nine, CardSuit.Hearts));
            simpleCardList.Add(new Card(CardValue.Ten, CardSuit.Hearts));
            simpleCardList.Add(new Card(CardValue.Ace, CardSuit.Hearts));

            //  Make an instance of each card
            Card Two_Card = new Card(CardValue.Two, CardSuit.Hearts);
            Card Three_Card = new Card(CardValue.Three, CardSuit.Hearts);
            Card Four_Card = new Card(CardValue.Four, CardSuit.Hearts);
            Card Five_Card = new Card(CardValue.Five, CardSuit.Hearts);
            Card Six_Card = new Card(CardValue.Six, CardSuit.Hearts);
            Card Seven_Card = new Card(CardValue.Seven, CardSuit.Hearts);
            Card Eight_Card = new Card(CardValue.Eight, CardSuit.Hearts);
            Card Nine_Card = new Card(CardValue.Nine, CardSuit.Hearts);
            Card Ten_Card = new Card(CardValue.Ten, CardSuit.Hearts);
            Card Ace_Card = new Card(CardValue.Ace, CardSuit.Hearts);


            List<Card> TwentyOneList = new List<Card>();
            TwentyOneList.Add(Nine_Card);
            TwentyOneList.Add(Ten_Card);
            TwentyOneList.Add(Two_Card);
            playersHand.Add(TwentyOneList);

            List<Card> TwentyList = new List<Card>();
            TwentyList.Add(Ten_Card);
            TwentyList.Add(Ten_Card);
            playersHand.Add(TwentyList);

            List<Card> NineteenList = new List<Card>();
            NineteenList.Add(Nine_Card);
            NineteenList.Add(Ten_Card);
            playersHand.Add(NineteenList);

            List<Card> EighteenList = new List<Card>();
            EighteenList.Add(Nine_Card);
            EighteenList.Add(Nine_Card);
            playersHand.Add(EighteenList);

            List<Card> SeventeenList = new List<Card>();
            SeventeenList.Add(Eight_Card);
            SeventeenList.Add(Nine_Card);
            playersHand.Add(SeventeenList);

            List<Card> SixteenList = new List<Card>();
            SixteenList.Add(Nine_Card);
            SixteenList.Add(Seven_Card);
            playersHand.Add(SixteenList);

            List<Card> FifteenList = new List<Card>();
            FifteenList.Add(Nine_Card);
            FifteenList.Add(Six_Card);
            playersHand.Add(FifteenList);

            List<Card> FourteenList = new List<Card>();
            FourteenList.Add(Nine_Card);
            FourteenList.Add(Five_Card);
            playersHand.Add(FourteenList);

            List<Card> ThirteenList = new List<Card>();
            ThirteenList.Add(Nine_Card);
            ThirteenList.Add(Four_Card);
            playersHand.Add(ThirteenList);

            List<Card> TwelveList = new List<Card>();
            TwelveList.Add(Three_Card);
            TwelveList.Add(Nine_Card);
            playersHand.Add(TwelveList);

            List<Card> ElevenList = new List<Card>();
            ElevenList.Add(Two_Card);
            ElevenList.Add(Nine_Card);
            playersHand.Add(ElevenList);

            List<Card> TenList = new List<Card>();
            TenList.Add(Six_Card);
            TenList.Add(Four_Card);
            playersHand.Add(TenList);

            List<Card> NineList = new List<Card>();
            NineList.Add(Seven_Card);
            NineList.Add(Two_Card);
            playersHand.Add(NineList);

            List<Card> EightList = new List<Card>();
            EightList.Add(Five_Card);
            EightList.Add(Three_Card);
            playersHand.Add(EightList);

            List<Card> SevenList = new List<Card>();
            SevenList.Add(Three_Card);
            SevenList.Add(Four_Card);
            playersHand.Add(SevenList);

            List<Card> SixList = new List<Card>();
            SixList.Add(Two_Card);
            SixList.Add(Four_Card);
            playersHand.Add(SixList);

            List<Card> FiveList = new List<Card>();
            FiveList.Add(Two_Card);
            FiveList.Add(Three_Card);
            playersHand.Add(FiveList);
        }


        private Double getHandEV_BSList(List<Card> playersHand, List<Card> dealersCards)
        {
            //  If the hand has busted, then return is always -1
            if (BlackJack.getHandValue(playersHand) > 21)
            {
                return -1;
            }


            //  Get the BS results for this hand
            int playerHandValue = BlackJack.getHandValue(playersHand);
            int dealerHandValue = (int)dealersCards[0].value;
            BSInfo BSResults = BSInfoArray_Hard_HitOnly[dealerHandValue, playerHandValue];
            if (BSResults.blackJackMove != BlackJackMove.Unknown)
            {
                return BSResults.ev;
            }
            else
            {
                throw new Exception("Cannot evaluate unknown BS_Result object");
            }
        }
        private Double getHandEV_BSList_Soft(List<Card> playersHand, List<Card> dealersCards)
        {

            //  Get the BS results for this hand
            int playerHandValue = BlackJack.getHandValue_Lower(playersHand);
            int dealerHandValue = (int)dealersCards[0].value;

            BSInfo BSResults = BSInfoArray_Soft_HitOnly[dealerHandValue, playerHandValue];

            if (BSResults.blackJackMove != BlackJackMove.Unknown)
            {
                return BSResults.ev;
            }
            else
            {
                throw new Exception("Cannot evaluate unknown BS_Result object");
            }
        }

        public void ComputeStandEV( List<Card> playersHand, List<Card> dealersCards, Double previousProb)
        {
            foreach (Card c in simpleCardList)
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
                    }
                    else if (bjr == BlackjackResult.Push)
                    {
                        masterPushProbSum = masterPushProbSum + (previousProb * getInfiniteDeckProb(c, dealersCards));
                    }
                    else
                    {
                        masterLoseProbSum = masterLoseProbSum + (previousProb * getInfiniteDeckProb(c, dealersCards));
                    }
                }
                else
                {
                    Double curentProb = getInfiniteDeckProb(c, dealersCards);
                    ComputeStandEV(playersHand, dealersCards_clone, previousProb * curentProb);
                }

            }

        }
        public void ComputeHitEV_BS(List<Card> playersHand, List<Card> dealersCards, Double previousProb)
        {
            foreach (Card c in simpleCardList)
            {
                List<Card> playersHand_Clone = new List<Card>(playersHand);
                playersHand_Clone.Add(c);

                //  If the upper and lower totals are different, then, we must have a hand with an Ace in it which cannot be directly deteremined.
                if (BlackJack.getHandValue(playersHand_Clone) != BlackJack.getHandValue_Lower(playersHand_Clone)) 
                {
                    Double EV = getHandEV_BSList_Soft(playersHand_Clone, dealersCards);
                    Double probability =  getInfiniteDeckProb(c);
                    EV_Total += (EV * probability);
                }
                else
                {
                    Double EV = getHandEV_BSList(playersHand_Clone, dealersCards);
                    Double probability = getInfiniteDeckProb(c);
                    EV_Total += (EV * probability);
                }
            }

        }
        public void ComputeHitEV_Double_Soft(List<Card> playersHand, List<Card> dealersCards, Double previousProb)
        {
            foreach (Card c in simpleCardList)
            {
                //  If the player hasn't drawn a card...
                if (playersHand.Count < 4)
                {
                    List<Card> playersHand_Clone = new List<Card>(playersHand);
                    playersHand_Clone.Add(c);
                    ComputeHitEV_Double(playersHand_Clone, dealersCards, getInfiniteDeckProb(c));
                }

                else
                {
                    //  Special case where we havent drawn a card, but the player has 3 cards, when they have 21

                    List<Card> dealersCards_clone = new List<Card>(dealersCards); //  Clone playersHand so can be passed "ByVal"
                    dealersCards_clone.Add(c);
                    if (BlackJack.getHandValue(dealersCards_clone) > 16) // TODO:  only S17
                    {
                        BlackjackResult bjr = BlackJack.getGameResult(dealersCards_clone, playersHand);
                        //  A push or a win is a win
                        if (bjr == BlackjackResult.PlayerWins)
                        {
                            masterWinProbSum = masterWinProbSum + (previousProb * getInfiniteDeckProb(c, dealersCards));
                        }
                        else if (bjr == BlackjackResult.Push)
                        {
                            masterPushProbSum = masterPushProbSum + (previousProb * getInfiniteDeckProb(c, dealersCards));
                        }
                        else
                        {
                            masterLoseProbSum = masterLoseProbSum + (previousProb * getInfiniteDeckProb(c, dealersCards));
                        }
                    }
                    else
                    {
                        Double curentProb = getInfiniteDeckProb(c, dealersCards);
                        ComputeHitEV_Double(playersHand, dealersCards_clone, previousProb * curentProb);
                    }

                }

            }
        }

        public void ComputeHitEV_Double(List<Card> playersHand, List<Card> dealersCards, Double previousProb)
        {
            foreach (Card c in simpleCardList)
            {
                //  If the player hasn't drawn a card...
                if (playersHand.Count < 3)
                {
                    List<Card> playersHand_Clone = new List<Card>(playersHand);
                    playersHand_Clone.Add(c);
                    ComputeHitEV_Double(playersHand_Clone, dealersCards, getInfiniteDeckProb(c));
                }
    
                else 
                {
                    //  Special case where we havent drawn a card, but the player has 3 cards, when they have 21
                    if ((playersHand[0].value == CardValue.Nine) & (playersHand[1].value == CardValue.Ten) & (playersHand[2].value == CardValue.Two) & (playersHand.Count == 3))
                    {
                        List<Card> playersHand_Clone = new List<Card>(playersHand);
                        playersHand_Clone.Add(c);
                        ComputeHitEV_Double(playersHand_Clone, dealersCards, getInfiniteDeckProb(c));
                    }
                    //  See how the probabilities turn out for all dealer cards
                    else
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
                            }
                            else if (bjr == BlackjackResult.Push)
                            {
                                masterPushProbSum = masterPushProbSum + (previousProb * getInfiniteDeckProb(c, dealersCards));
                            }
                            else
                            {
                                masterLoseProbSum = masterLoseProbSum + (previousProb * getInfiniteDeckProb(c, dealersCards));
                            }
                        }
                        else
                        {
                            Double curentProb = getInfiniteDeckProb(c, dealersCards);
                            ComputeHitEV_Double(playersHand, dealersCards_clone, previousProb * curentProb);
                        }
                    }
                }

            }
        }
        public BSInfo[,] ComputeStrategy()
        {
            //  Reset the BSInfoArray Object
            for (int x = 0; x < 11; x++)
            {
                for (int y = 0; y < 22; y++)
                {
                    BSInfoArray_Hard[x, y].blackJackMove = BlackJackMove.Unknown;
                }
            }

            //  First computer hard hands from 11 - 21.  When an Ace is added, we must choose +1
            foreach (List<Card> playerHand in playersHand)
            {
                foreach (Card dealerCard in simpleCardList)
                {
                    List<Card> dealersHand = new List<Card>();
                    dealersHand.Add(dealerCard);

                    if ((BlackJack.getHandValue(playerHand) == 5) & (dealerCard.value == CardValue.Nine))
                    {
                        int t = 0;
                    }
                    if (BlackJack.getHandValue(playerHand) >= 11)
                    {
                        ComputeHardStrategy(playerHand, dealersHand);
                    }
                    else
                    {
                        //  When the hand is less than 11, we must compute the Soft Total first, before computing the hard...
                        ComputeSoftStrategy(playerHand, dealersHand);
                        ComputeHardStrategy(playerHand, dealersHand);

                    }
                }
            }
            return BSInfoArray_Hard;

        }
        private void ComputeSoftStrategy(List<Card> playersHand, List<Card> dealersCards)
        {
            //  Computing the soft strategy only requires we add an Ace to the hand.  Assuming the player had the hard value, and an Ace, 
            //  We will determine the best move.
            Card ace = new Card(CardValue.Ace, CardSuit.Hearts);
            List<Card> playersHand_Clone = new List<Card>(playersHand);
            playersHand_Clone.Add(ace);

            //  Detremine Stand EV
            masterLoseProbSum = 0;
            masterPushProbSum = 0;
            masterWinProbSum = 0;
            ComputeStandEV(playersHand_Clone, dealersCards, 1);
            Double S_EV = masterWinProbSum - masterLoseProbSum;

            //  Determine Double EV
            masterLoseProbSum = 0;
            masterPushProbSum = 0;
            masterWinProbSum = 0;
            ComputeHitEV_Double_Soft(playersHand_Clone, dealersCards, 1);
            Double D_EV = (masterWinProbSum - masterLoseProbSum) * 2;

            //  Determine BS_Hit EV
            EV_Total = 0;
            ComputeHitEV_BS(playersHand_Clone, dealersCards, 1);
            Double BS_EV = EV_Total;

            //  Add to the Soft Hit BS List
            //  Add to the Hit Chart (Highest EV if Can't Double)
            if (BS_EV >= S_EV)
            {
                BSInfoArray_Soft_HitOnly[(int)dealersCards[0].value, BlackJack.getHandValue_Lower(playersHand_Clone)].ev = BS_EV;
                BSInfoArray_Soft_HitOnly[(int)dealersCards[0].value, BlackJack.getHandValue_Lower(playersHand_Clone)].blackJackMove = BlackJackMove.Hit;
            }
            else
            {
                BSInfoArray_Soft_HitOnly[(int)dealersCards[0].value, BlackJack.getHandValue_Lower(playersHand_Clone)].ev = S_EV;
                BSInfoArray_Soft_HitOnly[(int)dealersCards[0].value, BlackJack.getHandValue_Lower(playersHand_Clone)].blackJackMove = BlackJackMove.Stay;
            }
            //  Add to the final Soft BS List
            BSInfo bestMove = getBestMoveFromEV(S_EV, D_EV, BS_EV);
            BSInfoArray_Soft[(int)dealersCards[0].value, BlackJack.getHandValue_Lower(playersHand_Clone)].ev = bestMove.ev;
            BSInfoArray_Soft[(int)dealersCards[0].value, BlackJack.getHandValue_Lower(playersHand_Clone)].blackJackMove = bestMove.blackJackMove;

            
        }
        private void ComputeHardStrategy(List<Card> playerHand, List<Card> dealersHand)
        {
            //  Detremine Stand EV
            masterLoseProbSum = 0;
            masterPushProbSum = 0;
            masterWinProbSum = 0;
            ComputeStandEV(playerHand, dealersHand, 1);
            Double S_EV = masterWinProbSum - masterLoseProbSum;

            //  Determine Double EV
            masterLoseProbSum = 0;
            masterPushProbSum = 0;
            masterWinProbSum = 0;
            ComputeHitEV_Double(playerHand, dealersHand, 1);
            Double D_EV = (masterWinProbSum - masterLoseProbSum) * 2;

            //  Determine BS_Hit EV
            EV_Total = 0;
            ComputeHitEV_BS(playerHand, dealersHand, 1);
            Double BS_EV = EV_Total;


            //  Add to the Hit Chart
            if (BS_EV >= S_EV)
            {
                BSInfoArray_Hard_HitOnly[(int)dealersHand[0].value, BlackJack.getHandValue(playerHand)].ev = BS_EV;
                BSInfoArray_Hard_HitOnly[(int)dealersHand[0].value, BlackJack.getHandValue(playerHand)].blackJackMove = BlackJackMove.Hit;
            }
            else
            {
                BSInfoArray_Hard_HitOnly[(int)dealersHand[0].value, BlackJack.getHandValue(playerHand)].ev = S_EV;
                BSInfoArray_Hard_HitOnly[(int)dealersHand[0].value, BlackJack.getHandValue(playerHand)].blackJackMove = BlackJackMove.Stay;
            }

            //  Add to the final BS Chart
            BSInfo bestMove = getBestMoveFromEV(S_EV, D_EV, BS_EV);
            BSInfoArray_Hard[(int)dealersHand[0].value, BlackJack.getHandValue(playerHand)].ev = bestMove.ev;
            BSInfoArray_Hard[(int)dealersHand[0].value, BlackJack.getHandValue(playerHand)].blackJackMove = bestMove.blackJackMove;
        }
        private BSInfo getBestMoveFromEV(Double stay_EV, Double double_EV, Double hit_EV)
        {
            //  If it's best to Stay
            if ((stay_EV >= double_EV) & (stay_EV >= hit_EV))
            {
                return new BSInfo(stay_EV, BlackJackMove.Stay);
            }
            //  If it's best to Double
            else if ((double_EV >= stay_EV) & (double_EV >= hit_EV))
            {
                if (double_EV > 0)
                {
                    return new BSInfo(double_EV, BlackJackMove.Double);
                }
                else
                {
                    return new BSInfo(double_EV / 2, BlackJackMove.Hit);
                }
            }
            //  If it's best to Hit
            else
            {
                return new BSInfo(hit_EV, BlackJackMove.Hit);
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

            //  If the first dealer card is an Ace, and he only has that one card then...
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

    }

}
