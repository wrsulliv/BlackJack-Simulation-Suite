using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApplication1
{

    //  Holds the individual CardStats objects.  Can be used to add new stats.
    class Player_Statistics
    {
        int totalGames = 0;
        //  We need a list of CardStatsList.  We will use a hash function on the dealers card to to get the first list's index, and 
        //  a hash function on the first two cards of the players hand to get the index of the inner list.
        List<List<CardStats>> CardStatsList;
        public Player_Statistics()
        {
            //  Initialize the list of CardStats lists with 10 buckets, one for each dealer card
            this.CardStatsList = new List<List<CardStats>>();
            for (int x = 0; x < 10; x++)
            {
                this.CardStatsList.Add(new List<CardStats>());
                //  Add 18 CardStats entries into each list of CardStats
                for (int y = 0; y < 18; y++)
                {
                    this.CardStatsList[x].Add(new CardStats(y + 4));
                }
            }
        }
        //  Adds a new stat.  Stats are based on the first two cards in the hand.
        public void addStat(List<Card> cards, Card dealersCard, BlackjackResult result)
        {
            this.totalGames += 1;
            int index_Dealer = getIndexFromDealerCard(dealersCard);
            int index_Player = getIndexFromPlayersFirstTwoCards(cards);
            this.CardStatsList[index_Dealer][index_Player].addStat(result);
        }
        public void addStat(Card player1, Card player2, Card dealersCard, BlackjackResult result)
        {
            List<Card> cards = new List<Card>();
            cards.Add(player1);
            cards.Add(player2);
            this.totalGames += 1;
            int index_Dealer = getIndexFromDealerCard(dealersCard);
            int index_Player = getIndexFromPlayersFirstTwoCards(cards);
            this.CardStatsList[index_Dealer][index_Player].addStat(result);
        }
        public int getTotalGames()
        {
            return this.totalGames;
        }

        //  Used to Get the index in 'this.CardStatsList'  This is actually a Hash function
        private int getIndexFromDealerCard(Card dealersCard)
        {
            //  Subtract two because the values range between 2 and 11, but the index range is 0 to 9.
            return (int)BlackJack.convertCardToBlackJack(dealersCard).value - 2;
        }
        private int getIndexFromDealerCardTotal(int dealersValue)
        {
            //  Subtract two because the values range between 2 and 11, but the index range is 0 to 9.
            return (int)(dealersValue - 2);
        }
        private int getIndexFromPlayersFirstTwoCards(List<Card> playersCards)
        {
            int cardTotal = (int)BlackJack.convertCardToBlackJack(playersCards[0]).value + (int)BlackJack.convertCardToBlackJack(playersCards[1]).value;
            //  The values range from 4 to 22
            return cardTotal - 4;
        }
        private int getIndexFromPlayersCardTotal(int cardTotal)
        {
            //  The values range from 4 to 22
            return cardTotal - 4;
        }
        public void generateReport(String filePath)
        {
            List<string> fileData = new List<string>();

            //  Use a standard for loop because we need to find which card the dealer has by the bucket index
            for (int x = 0; x < this.CardStatsList.Count; x++)
            {
                //  Print out a line with the dealers card, add two to the index.  
                //    Do this because we subtracted two in order to match the range of the bucket's indices
                fileData.Add("Dealer Card - '" + ((BlackJack_CardValue)(x + 2)).ToString() + "'");
                foreach (CardStats cs in this.CardStatsList[x])
                {
                    string s = "     ";
                    int cardTotal = cs.getCardValue();
                    s += "| '" + cardTotal.ToString() + "' |";
                    //  Write the percentage of the time the player won, and the total number of games
                    s += " ------- " + cs.getWinPercentage().ToString() + " || Total = " + cs.getTotalPlays().ToString() + " ||";
                    fileData.Add(s);
                }
            }
            System.IO.File.WriteAllLines(filePath, fileData);
        }
        public Double getWinPercentage(Card dealersCard, List<Card> playersHand)
        {
            return this.CardStatsList[getIndexFromDealerCard(dealersCard)][getIndexFromPlayersFirstTwoCards(playersHand)].getWinPercentage();
        }
        public Double getWinPercentage(int dealersValue, int playersInitialHandValue)
        {
            return this.CardStatsList[getIndexFromDealerCardTotal(dealersValue)][getIndexFromPlayersCardTotal(playersInitialHandValue)].getWinPercentage();
        }
        
    }

    class CardStats
    { 
        int CardValue;
        int total;
        double wins;
        public CardStats(int cardValue)
        { 
            //  Do a copy of the first two cards in the parameter 'cards' list into this.Cards
            this.CardValue = cardValue;
            this.total = 0;
            this.wins = 0;
        }
        public int getCardValue()
        {
            return this.CardValue;
        }
        public int getTotalPlays()
        {
            return this.total;
        }
        public Double getWinPercentage()
        {
            return (Double)this.wins / (Double)this.total;
        }
        public void addStat(BlackjackResult result)
        {
            if (result == BlackjackResult.PlayerWins)
            {
                wins += 1;
                total += 1;
            }
            else if (result == BlackjackResult.Push)
            {
               // wins += 1;
                //total += 1;
            }
            else if (result == BlackjackResult.DealerWins)
            {
                total += 1;
            }
        }
    }
}
