using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApplication1
{
    class Dealer
    {
        private Boolean H17 = true;
        private List<Card> Cards;
        private Deck Deck;
        public Dealer(Boolean H17, Deck deck)
        {
            this.H17 = H17;
            this.Cards = new List<Card>();
            this.Deck = deck;
        }

        public void clearTable(List<Player> players)
        {
            for (int x = 0; x < players.Count; x++)
            {
                //  Clear the players
                players[x].clearHand();
            }

            //  Clear the dealer
            this.Cards.Clear();
        }
        public Card takeHit()
        {
            return this.Deck.draw();
        }
        public void getResults(List<Player> players, Player_Statistics ps)
        {
            foreach (Player p in players)
            {
                //  We will test hard hands and soft hand separatly and split hands seperatly
                if (!p.hasAceInInitalHand() & !p.hasDoublesInInitialHand() & !this.hasBlackJack())
                {
                    BlackjackResult result = BlackJack.getGameResult(this.Cards, p.getHand(0));
                    ps.addStat(p.getHand(0), this.Cards[0], result);
                }
            }
        }

        //  Returns a Boolean as to wether or not the dealer has a blackjack
        public bool hasBlackJack()
        {
            return BlackJack.doesHandHaveBlackjack(this.Cards);
        }
        public List<Card> getHand()
        {
            return this.Cards;
        }
        public void dealCards(List<Player> players)
        {
            for (int x = 0; x < players.Count; x++)
            {
                players[x].takeCard(this.Deck.draw());
            }
            Cards.Add(this.Deck.draw());
            for (int x = 0; x < players.Count; x++)
            {
                players[x].takeCard(this.Deck.draw());
            }
            Cards.Add(this.Deck.draw());
        }

        public void drawTo17()
        {
            while (BlackJack.getHandValue(this.Cards) < 17)
            {
                this.Cards.Add(this.Deck.draw());
            }
        }
    }
}
