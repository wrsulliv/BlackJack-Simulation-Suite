using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfApplication1
{
    class BlackjackProcessor
    {

        public ProcessRunner Start(Delegate prog_del)
        {
            Player_Statistics stay_stats = new Player_Statistics();
            testStayScenario(stay_stats, prog_del);

            Player_Statistics hit_stats = new Player_Statistics();
            testHitScenario(hit_stats, prog_del);

            return new ProcessRunner(hit_stats, stay_stats);
        }

        private void testStayScenario(Player_Statistics ps, Delegate prog_del)
        {
            //  Get the list of cards
            List<Card> card_list = new List<Card>();
            Deck d = new Deck(1, Deck.ShuffleType.Durstenfeld);
            card_list = d.DeckList;


            //  Test all "Stay" scenarios
            for (int i = 0; i < card_list.Count; i++)
            {
                Card player1 = card_list.ElementAt(i);
                for (int j = 0; j < card_list.Count; j++)
                {
                    if (!(isSkipIndex(j, i)))
                    {
                        Card player2 = card_list.ElementAt(j);
                        {
                            if (!((player1.value == CardValue.Ace) | (player2.value == CardValue.Ace)) & !(player1 == player2)) //  No soft hands
                            {
                                for (int k = 0; k < card_list.Count; k++)
                                {
                                    if (!(isSkipIndex(k, i, j)))
                                    {
                                        object[] obj = new object[4];
                                        obj[0] = i.ToString();
                                        obj[1] = j.ToString();
                                        obj[2] = k.ToString();
                                        obj[3] = "Stay Round";
                                        prog_del.DynamicInvoke((Object)obj);
                                        Card dealerUp = card_list.ElementAt(k);

                                        //  Make the skip list
                                        List<int> skipList = new List<int>();
                                        skipList.Add(i); skipList.Add(j); skipList.Add(k);

                                        //  Make the dealers list
                                        List<Card> dealersList = new List<Card>();
                                        dealersList.Add(dealerUp);

                                        //  Make the players list
                                        List<Card> playersList = new List<Card>();
                                        playersList.Add(player1); playersList.Add(player2);

                                        //  Loop through all possible dealer draws
                                        dealerDraw(skipList, card_list, dealersList, playersList, ps);
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }
        private void dealerDraw(List<int> skipList, List<Card> card_list, List<Card> dealersCards, List<Card> playersCards, Player_Statistics ps)
        {
            for (int i = 0; i < card_list.Count; i++)
            {
                if (!(isSkipIndex(i, skipList)))
                {
                    //  Make Clones so the lists can be passed "ByValue"
                    List<int> skipList_clone = new List<int>(skipList);
                    List<Card> dealersCards_clone = new List<Card>(dealersCards);

                    dealersCards_clone.Add(card_list.ElementAt(i));
                    if (BlackJack.getHandValue(dealersCards_clone) < 17)
                    {
                        skipList_clone.Add(i);

                        dealerDraw(skipList_clone, card_list, dealersCards_clone, playersCards, ps);
                    }
                    else
                    {
                        //  Dealer will stand on S17
                        BlackjackResult bjr = BlackJack.getGameResult(dealersCards_clone, playersCards);
                        ps.addStat(playersCards, dealersCards_clone.ElementAt(0), bjr); //  TODO: Check this if 0 is dealers first card

                    }
                }
            }
        }
        private bool isSkipIndex(int testIndex, List<int> skipList)
        {
            foreach (int i in skipList)
            {
                if (testIndex == i)
                {
                    return true;
                }
            }
            return false;
        }
        private bool isSkipIndex(int testIndex, int first, int second, int third, int fourth)
        {
            if ((testIndex == first) | (testIndex == second) | (testIndex == third) | (testIndex == fourth))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool isSkipIndex(int testIndex, int first, int second, int third)
        {
            if ((testIndex == first) | (testIndex == second) | (testIndex == third))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool isSkipIndex(int testIndex, int first, int second)
        {
            if ((testIndex == first) | (testIndex == second))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool isSkipIndex(int testIndex, int first)
        {
            if (testIndex == first)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void testHitScenario(Player_Statistics ps, Delegate prog_del)
        {
            //  Get the list of cards
            List<Card> card_list = new List<Card>();
            Deck d = new Deck(1, Deck.ShuffleType.Durstenfeld, new CardCounting(new BlackJackGameParams()));
            card_list = d.DeckList;


            //  Test all "Hit" scenarios
            for (int i = 0; i < card_list.Count; i++)
            {
                Card player1 = card_list.ElementAt(i);
                for (int j = 0; j < card_list.Count; j++)
                {
                    if (!(isSkipIndex(j, i)))
                    {
                        Card player2 = card_list.ElementAt(j);
                        {
                            if (!((player1.value == CardValue.Ace) | (player2.value == CardValue.Ace)) & !(player1 == player2)) //  No soft hands
                            {
                                for (int k = 0; k < card_list.Count; k++)
                                {
                                    if (!(isSkipIndex(k, i, j)))
                                    {
                                        Card dealerUp = card_list.ElementAt(k);
                                        for (int l = 0; l < card_list.Count; l++)
                                        {
    
                                            if (!(isSkipIndex(l, i, j, k)))
                                            {
                                                object[] obj = new object[4];
                                                obj[0] = i.ToString();
                                                obj[1] = j.ToString();
                                                obj[2] = k.ToString();
                                                obj[3] = l.ToString();
                                                prog_del.DynamicInvoke((Object)obj);

                                                Card player3 = card_list.ElementAt(l);
                                                //  Make the skip list
                                                List<int> skipList = new List<int>();
                                                skipList.Add(i); skipList.Add(j); skipList.Add(k); skipList.Add(l);

                                                //  Make the dealers list
                                                List<Card> dealersList = new List<Card>();
                                                dealersList.Add(dealerUp);

                                                //  Make the players list
                                                List<Card> playersList = new List<Card>();
                                                playersList.Add(player1); playersList.Add(player2); playersList.Add(player3);

                                                //  Loop through all possible dealer draws
                                                dealerDraw(skipList, card_list, dealersList, playersList, ps);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }
    }
}
