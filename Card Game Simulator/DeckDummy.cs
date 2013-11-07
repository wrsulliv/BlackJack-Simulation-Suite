using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApplication1
{
    class DeckDummy : Deck
    {
        public DeckDummy()
            : base(6, ShuffleType.Durstenfeld)
        {

        }
        List<Card> cardList = new List<Card>();
        public override Card draw()
        {
            Card c = cardList[0];
            cardList.RemoveAt(0);
            return c;
        }

        public void setCardList(List<Card> cardList)
        {
        this.cardList = cardList;
        }
    }
}
