using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApplication1
{
    class BSDealer
    {
        private Boolean H17 = true;
        private List<Card> Cards;
        private Deck Deck;
        public BSDealer(Boolean H17, Deck deck)
        {
            this.H17 = H17;
            this.Cards = new List<Card>();
            this.Deck = deck;
        }

        //  For testing only
        public void setHand(Card upCard, Card hiddenCard)
        {
            this.Cards.Add(upCard);
            this.Cards.Add(hiddenCard);
        }

        //  For testing only
        public void setHand(List<Card> dealersHand)
        {
            this.Cards = dealersHand;
        }

        public void clearTable(List<BSPlayer> players)
        {
            for (int x = 0; x < players.Count; x++)
            {
                //  Clear the players
                players[x].clearHand();
                players[x].clearCurrentBets();
            }

            //  Clear the dealer
            this.Cards.Clear();
        }

        //  Used by players to get a card from the dealer who uses the deck object
        public Card takeHit()
        {
            return this.Deck.draw();
        }

        //  Retrieves the dealers face up card (the first dealer card)
        public Card getUpCard()
        {
            return this.Cards[0];
        }

        //  Retrieves the dealers face down card (the second dealer card)
        public Card getHiddenCard()
        {
            return this.Cards[1];
        }

        public void getResults(List<BSPlayer> players, Player_Statistics ps)
        {
            foreach (BSPlayer p in players)
            {
                //  We will test hard hands and soft hand separatly and split hands seperatly
                if (!p.hasAceInInitalHand() & !p.hasDoublesInHand(0) & !this.hasBlackJack())
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
        public void dealCards(List<BSPlayer> players)
        {
            for (int x = 0; x < players.Count; x++)
            {
                if (!players[x].hasQuit)
                {
                    players[x].takeCard(this.Deck.draw());
                }
            }
            Cards.Add(this.Deck.draw());
            for (int x = 0; x < players.Count; x++)
            {
                if (!players[x].hasQuit)
                {
                    players[x].takeCard(this.Deck.draw());
                }
            }

            //  Draw a secret card so card counters cannot see
            Cards.Add(this.Deck.draw_secret());
        }

        public void drawTo17(bool H17)
        {
            while (BlackJack.getHandValue(this.Cards) < 17)
            {
                this.Cards.Add(this.Deck.draw());
            }

            if (H17)
            {
                if ((BlackJack.getHandValue(this.Cards) == 17) & (BlackJack.getHandValue_Lower(this.Cards) != 17))
                {
                    this.Cards.Add(this.Deck.draw());
                    drawTo17(H17);
                }
            }
        }
    }
}
