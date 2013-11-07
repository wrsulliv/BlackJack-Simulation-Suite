using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApplication1
{
    class Player
    {

        //  Holds the players hands, the '0' index is for the initial hand, others are split hands
        private List<List<Card>> cardList;
        int HitCount = 0;
        public Player(int hitCount)
        {
            this.cardList = new List<List<Card>>();
            this.cardList.Add(new List<Card>());
            this.HitCount = hitCount;
        }

        //  Used by the dealer class to give the Player a card
        public void takeCard(Card card)
        {
            this.cardList[0].Add(card);
        }
        public void clearHand()
        {
            this.cardList = new List<List<Card>>();
            this.cardList.Add(new List<Card>());
        }

        public void playHand(Dealer dealer, List<Player> players, Player_Statistics ps)
        {
            //  Rules to play the hand
            //  In this first case, we're going to stand..

            //  If the dealer has blackjack, then the player cannot take any hits. THe player must
            //  have a natural blackjack to push.
            if (!dealer.hasBlackJack())
            {
                //  Take as a hit until the specific count it reached
                while ((this.cardList[0].Count - 2) < this.HitCount)
                {
                    this.cardList[0].Add(dealer.takeHit());
                }
            }
            //  We're always going to split Doubles..

        }
        public List<Card> getHand(int splitIndex)
        {
            return this.cardList[0];
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
        //  Should be extended later to check for splits. thats why we use 0 index.
        public Boolean hasDoublesInInitialHand()
        {
            if (BlackJack.convertCardToBlackJack(this.cardList[0][0]).value == BlackJack.convertCardToBlackJack(this.cardList[0][1]).value)
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
