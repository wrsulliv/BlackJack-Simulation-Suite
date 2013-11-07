using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WpfApplication1
{
    class TestSuite
    {
        public TestSuite()
        {

        }

        public Card getCardFromText(string text)
        {
            switch (text)
            {
                case "A":
                    return new Card(CardValue.Ace, CardSuit.Hearts);
                case "2":
                    return new Card(CardValue.Two, CardSuit.Hearts);
                case "3":
                    return new Card(CardValue.Three, CardSuit.Hearts);
                case "4":
                    return new Card(CardValue.Four, CardSuit.Hearts);
                case "5":
                    return new Card(CardValue.Five, CardSuit.Hearts);
                case "6":
                    return new Card(CardValue.Six, CardSuit.Hearts);
                case "7":
                    return new Card(CardValue.Seven, CardSuit.Hearts);
                case "8":
                    return new Card(CardValue.Eight, CardSuit.Hearts);
                case "9":
                    return new Card(CardValue.Nine, CardSuit.Hearts);
                case "T":
                    return new Card(CardValue.Ten, CardSuit.Hearts);
                default:
                    throw new Exception("Invalid Card Symbol Used");
            }
        }

        public void test_CardCounter(BlackJackGameParams blackJackGameParams)
        {
            CardCounting cc = new CardCounting(blackJackGameParams);
            Deck d = new Deck(1, Deck.ShuffleType.Durstenfeld, cc);
            for(int i = 0; i < 52; i++)
            {
                d.draw();
            }

            if (cc.getCount(blackJackGameParams) == CardCounting.getIRC(blackJackGameParams) + 4)
            {
                System.Diagnostics.Debug.WriteLine("Counting - PASS");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Counting - FAIL");
            }
        }
        public void test_BSPlayer_playHand(BlackJackGameParams blackJackGameParams, string filePath)
        {
            var lines = File.ReadAllLines(filePath).Select(a => a.Split(',', ':'));
            IEnumerator<string[]> iter = lines.GetEnumerator();
            while (1 == 1)
            {
                int itemCount = 0;
                List<Card> deckList = new List<Card>();
                List<Card> playersHand = new List<Card>();
                List<Card> dealersHand = new List<Card>();
                Double initialBet = 0;
                Double winnings = 0;
                string name = "";
                while (iter.MoveNext() & (itemCount < 5))
                {
                    var l = iter.Current;
                    switch (l[0])
                    {
                        case "Name":
                            name = l[1];
                            break;
                        case "Player":
                            for (int i = 1; i < l.Length; i++)
                            {
                                l[i].Replace(" ", "");
                                playersHand.Add(getCardFromText(l[i]));
                            }
                            itemCount += 1;
                            break;
                        case "Dealer":
                            for (int i = 1; i < l.Length; i++)
                            {
                                l[i].Replace(" ", "");
                                dealersHand.Add(getCardFromText(l[i]));
                            }
                            itemCount += 1;
                            break;
                        case "Deck":
                            for (int i = 1; i < l.Length; i++)
                            {
                                l[i].Replace(" ", "");
                                deckList.Add(getCardFromText(l[i]));
                            }
                            itemCount += 1;
                            break;
                        case "Initial":
                            l[1].Replace(" ", "");
                            initialBet = Double.Parse(l[1]);
                            itemCount += 1;
                            break;
                        case "Win":
                            l[1].Replace(" ", "");
                            winnings = Double.Parse(l[1]);
                            itemCount += 1;
                            break;
                        default:
                            break;

                    }
                }

                //  When the iterator has run out of new items then return
                if (!(itemCount == 5)) return;
                BSPlayer player = new BSPlayer(blackJackGameParams);
                DeckDummy deck = new DeckDummy();
                BSDealer dealer = new BSDealer(false, deck);

                deck.setCardList(deckList);

                dealer.setHand(dealersHand);

                player.clearCurrentBets();
                player.clearHand();
                player.setBankRoll(blackJackGameParams.bankroll);
                player.setInitialBet(initialBet);
                player.setFirstHand(playersHand);

                player.playHand(dealer, new CardCounting(blackJackGameParams));
                BasicStrategySimulator.processPlayerWinnings(player, dealer);
                if (player.getTotalHandWinnings() == winnings)
                {
                    System.Diagnostics.Debug.WriteLine(name + ": PASS");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(name + ": FAIL");
                }

            }
        }
    }
}
