//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace WpfApplication1
//{

//    //  Holds the individual CardStats objects.  Can be used to add new stats.
//    class Player_Statistics_Backup
//    {
//        //  We need a list of CardStatsList, becuase we will use the dealers card value as a Hash function to to get the first list's index
//        List<List<CardStats>> CardStatsList;
//        Boolean bucketByBothCards = false;
//        public Player_Statistics_Backup(Boolean bucketByBothCards)
//        {
//            //  Initialize the list of CardStats lists with 10 blank buckets
//            this.CardStatsList = new List<List<CardStats>>();
//            for (int x = 0; x < 10; x++)
//            {
//                this.CardStatsList.Add(new List<CardStats>());
//            }
//            this.bucketByBothCards = bucketByBothCards;
//        }
//        //  Adds a new stat.  Stats are based on the first two cards in the hand.
//        public void addStat(List<Card> cards, Card dealersCard, BlackjackResult result)
//        {
//            //  Two options, place in buckets by the two cards different values,  or using the total of the two cards
//            if (this.bucketByBothCards)
//            {
//                //  Subtract two from the value, because the values spread from 2-11, but the buckets indices spread from 0-9
//                foreach (CardStats cs in this.CardStatsList[(int)BlackJack.convertCardToBlackJack(dealersCard).value - 2])
//                {
//                    if (cs.hasFirstTwoCards(cards))
//                    {
//                        cs.addStat(result);
//                        return;
//                    }
//                }

//                //  If the method hasn't returned yet, then the 'Bucket' doesn't yet exist..
//                //  Create a new list holding only the first two cards from the players card
//                List<Card> cl = new List<Card>();
//                cl.Add(cards[0]);
//                cl.Add(cards[1]);

//                //  Add the new list to a new CardStats object
//                CardStats cs2 = new CardStats(cl);

//                //  Put a new result into the CardStat Object
//                cs2.addStat(result);

//                //  Add the CardStsats object to the CardStatsList
//                this.CardStatsList[(int)BlackJack.convertCardToBlackJack(dealersCard).value - 2].Add(cs2);
//            }
//            else
//            {
//                //  Subtract two from the value, because the values spread from 2-11, but the buckets indices spread from 0-9
//                foreach (CardStats cs in this.CardStatsList[(int)BlackJack.convertCardToBlackJack(dealersCard).value - 2])
//                {
//                    if (cs.getCardValue() == (int)BlackJack.convertCardToBlackJack(cards[0]).value + (int)BlackJack.convertCardToBlackJack(cards[1]).value)
//                    {
//                        cs.addStat(result);
//                        return;
//                    }
//                }

//                //  If the method hasn't returned yet, then the 'Bucket' doesn't yet exist..

//                //  Make a new CardStats object with the CardValue
//                CardStats cs2 = new CardStats((int)BlackJack.convertCardToBlackJack(cards[0]).value + (int)BlackJack.convertCardToBlackJack(cards[1]).value);

//                //  Put a new result into the CardStat Object
//                cs2.addStat(result);

//                //  Add the CardStsats object to the CardStatsList
//                this.CardStatsList[(int)BlackJack.convertCardToBlackJack(dealersCard).value - 2].Add(cs2);
//            }
//        }
//        public void generateReport(String filePath)
//        {

//            if (this.bucketByBothCards)
//            {
//                generateReportWithBothCards(filePath);
//            }
//            else
//            {
//                generateReportWithInteger(filePath);
//            }
//        }
//        public void generateReportWithBothCards(String filePath)
//        {
//            List<string> fileData = new List<string>();

//            //  Use a standard for loop because we need to find which card the dealer has by the bucket index
//            for (int x = 0; x < this.CardStatsList.Count; x++)
//            {
//                //  Print out a line with the dealers card, add two to the index.  
//                //    Do this because we subtracted two in order to match the range of the bucket's indices
//                fileData.Add("Dealer Card - '" + ((BlackJack_CardValue)(x + 2)).ToString() + "'");
//                foreach (CardStats cs in this.CardStatsList[x])
//                {
//                    string s = "     ";
//                    foreach (Card c in cs.getCards())
//                    {
//                        //  Write out the names of the cards in the players hand
//                        s += "'" + BlackJack.convertCardToBlackJack(c).value.ToString() + "' | ";
//                    }

//                    //  Write the percentage of the time the player won, and the total number of games
//                    s += " ------- " + cs.getWinPercentage().ToString() + " || Total = " + cs.getTotalPlays().ToString() + " ||";
//                    fileData.Add(s);
//                }
//            }
//            System.IO.File.WriteAllLines(filePath, fileData);

//        }
//        public void generateReportWithInteger(String filePath)
//        {
//            List<string> fileData = new List<string>();

//            //  Use a standard for loop because we need to find which card the dealer has by the bucket index
//            for (int x = 0; x < this.CardStatsList.Count; x++)
//            {
//                //  Print out a line with the dealers card, add two to the index.  
//                //    Do this because we subtracted two in order to match the range of the bucket's indices
//                fileData.Add("Dealer Card - '" + ((BlackJack_CardValue)(x + 2)).ToString() + "'");
//                foreach (CardStats cs in this.CardStatsList[x])
//                {
//                    string s = "     ";
//                    int cardTotal = cs.getCardValue();
//                    s += "| '" + cardTotal.ToString() + "' |";
//                    //  Write the percentage of the time the player won, and the total number of games
//                    s += " ------- " + cs.getWinPercentage().ToString() + " || Total = " + cs.getTotalPlays().ToString() + " ||";
//                    fileData.Add(s);
//                }
//            }
//            System.IO.File.WriteAllLines(filePath, fileData);
//        }
        
//    }

//    class CardStats_Backup
//    { 
//        List<Card> Cards;
//        int CardValue;
//        int total;
//        int wins;
//        public CardStats_Backup(List<Card> cards)
//        { 
//            //  Do a copy of the first two cards in the parameter 'cards' list into this.Cards
//            this.Cards = new List<Card>();
//            for(int x=0; x < 2; x++)
//            {
//                this.Cards.Add(cards[x]);
//            }
//            this.total = 0;
//            this.wins = 0;
//        }
//        public CardStats_Backup(int cardValue)
//        {
//            this.CardValue = cardValue;
//            this.total = 0;
//            this.wins = 0;
//        }
//        public int getCardValue()
//        {
//            return this.CardValue;
//        }
//        public int getTotalPlays()
//        {
//            return this.total;
//        }
//        public List<Card> getCards()
//        {
//            return this.Cards;
//        }
//        public Double getWinPercentage()
//        {
//            return (Double)this.wins / (Double)this.total;
//        }
//        public void addStat(BlackjackResult result)
//        {
//            if (result == BlackjackResult.PlayerWins)
//            {
//                wins += 1;
//                total += 1;
//            }
//            else if (result == BlackjackResult.Push)
//            {

//            }
//            else if (result == BlackjackResult.DealerWins)
//            {
//                total += 1;
//            }
//        }

//        //  Used to test wether the bucket has the first two cards in the passed list
//        public Boolean hasFirstTwoCards(List<Card> cards)
//        {
//            //  Make a copy of this objects Card aray so we can remove cards as they're checked (There will always be only two cards in the 'this.Cards' List)
//            //  Also, we don't care about the suit, so convert to a 'BlackJack_Card' object
//            List<BlackJack_Card> cardCopy = new List<BlackJack_Card>();
//            foreach(Card c in this.Cards)
//            {
//                cardCopy.Add(BlackJack.convertCardToBlackJack(c));
//            }

//            for (int x = 0; x < 2; x++)
//            {
//                BlackJack_Card bc = BlackJack.convertCardToBlackJack(cards[x]);
//                if (!cardCopy.Contains(bc))
//                {
//                    //  Return false because a value is missing
//                    return false;
//                }
//                else
//                {
//                    cardCopy.Remove(BlackJack.convertCardToBlackJack(cards[x]));
//                }
//            }
//            //  Return true only if we get through the entire list successfully
//            return true;

//        }

//        //  Used to test wether the bucket has all the cards in the passed list
//        public Boolean hasAllCards(List<Card> cards)
//        {
//            //  Make a copy of the Card aray so we can remove cards as they're checked
//            //  Also, we don't care about the suit, so convert to a 'BlackJack_Card' object
//            List<BlackJack_Card> cardCopy = new List<BlackJack_Card>();
//            foreach (Card c in this.Cards)
//            {
//                cardCopy.Add(BlackJack.convertCardToBlackJack(c));
//            }

//            for (int x = 0; x < cards.Count; x++)
//            {
//                BlackJack_Card bc = BlackJack.convertCardToBlackJack(cards[x]);
//                if (!cardCopy.Contains(bc))
//                {
//                    //  Return false because a value is missing
//                    return false;
//                }
//                else
//                {
//                    cardCopy.Remove(BlackJack.convertCardToBlackJack(cards[x]));
//                }
//            }

//            //  Return true only if we get through the entire list successfully
//            return true;

//        }
//    }
//}
