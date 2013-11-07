using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace WpfApplication1
{
    class Deck
    {
        private Random mainRandom = new Random(System.DateTime.Now.Millisecond);
        public List<Card> DeckList;
        private List<Card> DeckList_Backup;
        private ShuffleType shuffleType;
        private int totalRndNumbers = 0;
        CardCounting cardCounter;
        private int previousSeed = 0;
        private System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
        public Deck(int size, ShuffleType shuffleType, CardCounting cardCounter)
        {
            //  Setup the cardCOunter object
            this.cardCounter = cardCounter;

            this.DeckList = new List<Card>();
            for (int x = 0; x < size; x++)
            {
                fillWith52Cards();
            }

            this.DeckList_Backup = new List<Card>();
            this.shuffleType = shuffleType;
        }

        //  Added to make old code compile
        public Deck(int size, ShuffleType shuffleType)
        {
            //  Setup the cardCOunter object
            this.cardCounter = new CardCounting(new BlackJackGameParams());

            this.DeckList = new List<Card>();
            for (int x = 0; x < size; x++)
            {
                fillWith52Cards();
            }

            this.DeckList_Backup = new List<Card>();
            this.shuffleType = shuffleType;
        }

        private void addCurrentToBackup()
        {
            //  Fill the backup with the cards
            foreach (Card c in DeckList)
            {
                this.DeckList_Backup.Add(c);
            }
        }
        private void fillWith52Cards()
        {
            //  Loop between 0 and 12, Ace - King
            for (int x = 1; x < 14; x++)
            {
                //  Loop through the suits
                for (int y = 1; y < 5; y++)
                {
                    DeckList.Add(new Card((CardValue)x, (CardSuit)y));
                }
            }
        }

        public void shuffle()
        {
            addCurrentToBackup();
            
            //  Clear and refill the deck from the backup
            this.DeckList.Clear();
            this.DeckList = new List<Card>();
            foreach (Card c in DeckList_Backup)
            {
                this.DeckList.Add(c);
            }
            this.DeckList_Backup.Clear();
            //  After we generate 100,000 numbers, restart the generator
            if (this.totalRndNumbers > 100000)
            {
                while (this.previousSeed != DateTime.Now.Millisecond)
                {
                    this.mainRandom = new Random(DateTime.Now.Millisecond);
                    this.totalRndNumbers = 0;
                    //System.Diagnostics.Debug.WriteLine("RND Seed: " + DateTime.Now.Millisecond.ToString());
                    this.previousSeed = DateTime.Now.Millisecond;
                }

            }

            switch (this.shuffleType)
            { 
                case ShuffleType.Durstenfeld:
                    //  Shuffle the deck 2 - 20 times
                    //for (int i = 0; i < 7; i++)
                   // {
                        DurstenfeldShuffle();
                   // }
                    return;
                case ShuffleType.Sullivan:
                    SullivanShuffle();
                    return;
                case ShuffleType.Other:
                    OtherShuffle();
                    return;
                default:
                    DurstenfeldShuffle();
                    return;
            }
        }
        private void DurstenfeldShuffle()
        {

            //  Durstenfeld Shuffeling Algorithem O(n)
            //  We only have to go down to swapping 1, because it will either swap with 0 or stay the same
            //  Going down to zero is a waste of time because zero will only swap with 0.
            for (int i = this.DeckList.Count - 1; i > 0; i--)
            {
                //  Get an index that hasn't yet been swapped
                //int r = (int)(i * mainRandom.NextDouble());
                int r = rndNumber(i);
                this.totalRndNumbers += 1;
                swapCards(i, r);
            }
        }

        //  Shuffles and Prints the values of the cards to the console
        public void printDeck()
        {
            this.shuffle();
            foreach (Card c in this.DeckList)
            {
                System.Diagnostics.Debug.WriteLine(c.value.ToString());
            }
        }
        private int rndNumber(int mod)
        {
            //byte[] randomNum = new byte[1];
            //rng.GetBytes(randomNum);
            //return (Convert.ToInt32(randomNum[0]) % mod);
            return mainRandom.Next(0, mod);

        }
        private void SullivanShuffle()
        {
            this.totalRndNumbers += 1;
            for (int x = 0; x < mainRandom.NextDouble() * 30000; x++)
            { 
                int firstIndex = (int)(mainRandom.NextDouble() * this.DeckList.Count - 1);
                int secondIndex = (int)(mainRandom.NextDouble() * this.DeckList.Count - 1);
                this.totalRndNumbers += 2;
                swapCards(firstIndex, secondIndex);
            }
        }
        private void OtherShuffle()
        { 
        
        }
        private void swapCards(int index1, int index2)
        {
            Card one_Value = this.DeckList[index1];
            this.DeckList[index1] = this.DeckList[index2];
            this.DeckList[index2] = one_Value;
        }
        public virtual Card draw()
        {
            Card c = DeckList[0];
            DeckList.RemoveAt(0);
            this.DeckList_Backup.Add(c);
            cardCounter.countCard(c);
            return c;
        }
        public Card draw_secret()
        {
            Card c = DeckList[0];
            DeckList.RemoveAt(0);
            this.DeckList_Backup.Add(c);
            return c;
        }

        //  Percentage of the deck already used
        public Double penetrationPercent()
        {
            int totalCards = this.DeckList_Backup.Count + this.DeckList.Count;
            return (1 - (Double)this.DeckList.Count / (Double)totalCards);
        }
        public enum ShuffleType
        {
            Durstenfeld=0, Sullivan=1, Other=2
        }
    }
}
