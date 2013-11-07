using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Animation;

namespace WpfApplication1
{
    class Runner
    {
        List<Player_Statistics> PS_NoHit;
        List<Player_Statistics> PS_Hit;
        public Runner(Delegate progressBarCodeDelegate, Delegate updateDeviationIterationDelegate, int iterationsForDeviationAdjutment, int iterationsOfEntireDeck, int deckCount, Deck.ShuffleType shuffleType)
        {
            //  Add code to loop through and create multiple Player_Statistics objects, and then
            //  Look at the Deviations after averaging the results.
            this.PS_NoHit = new List<Player_Statistics>();
            this.PS_Hit = new List<Player_Statistics>();
            for (int x = 0; x < iterationsForDeviationAdjutment; x++)
            {
                //  Update the Deviation Iteration Label
                object[] obj = new object[2];
                obj[0] = iterationsForDeviationAdjutment - x;
                updateDeviationIterationDelegate.DynamicInvoke((Object)obj);
                //  Get the required player statistics...
                BlackJack BJ = new BlackJack(deckCount, true, 1, 0, "C:/BlackJack/blackJack_iter" + x.ToString() + "_0Hit.txt", false, progressBarCodeDelegate, iterationsOfEntireDeck, shuffleType);
                Player_Statistics PS_NoHit = BJ.start();
                BJ = new BlackJack(deckCount, true, 1, 1, "C:/BlackJack/blackJack_iter" + x.ToString() + "_1Hit.txt", false, progressBarCodeDelegate, iterationsOfEntireDeck, shuffleType);
                Player_Statistics PS_Hit = BJ.start();
                this.PS_NoHit.Add(PS_NoHit);
                this.PS_Hit.Add(PS_Hit);
            }
        }
        public int getTotalGameCount()
        {
            int totalGames = 0;
            foreach (Player_Statistics ps in PS_NoHit)
            {
                totalGames += ps.getTotalGames();
            }
            foreach (Player_Statistics ps in PS_Hit)
            {
                totalGames += ps.getTotalGames();
            }
            return totalGames;
        }
       
        public Grid generateResultGrid_HardHands()
        {


            //  Generate the actual grid
            Grid g = new Grid();
            g.Width = 536;
            g.Height = 500;
            g.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            g.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            g.ColumnDefinitions.Add(new ColumnDefinition());
            g.RowDefinitions.Add(new RowDefinition());

            for (int x = 2; x < 12; x++)
            {
                g.ColumnDefinitions.Add(new ColumnDefinition());
                Label label = new Label();
                Grid.SetColumn(label, x - 1);
                Grid.SetRow(label, 0);
                label.Content = x.ToString();
                g.Children.Add(label);
                for (int y = 5; y < 21; y++)
                {
                    //  Create the row definitions only once.
                    if (x == 2)
                    {
                        Label label2 = new Label();
                        Grid.SetColumn(label2, 0);
                        Grid.SetRow(label2, y-4);

                        // Set the row header with the proper text
                        if (y == 5)
                        {
                            label2.Content = "5 / Less";
                        }
                        else
                        {
                            label2.Content = y.ToString();
                        }
                        g.Children.Add(label2);
                        g.RowDefinitions.Add(new RowDefinition());
                    }
                    //  

                    Label l = new Label();
                    //  Add the column definition, use -1, because we want to start these at index of 1.
                    Grid.SetColumn(l, x-1);
                    //  Add the row definition, use -4, because we want to start these at index of 1.
                    Grid.SetRow(l, y-4);
                    //  Always hit on 5 or less.  Never double, it's a 33% chance dealer will bust (off internet)
                    if (y == 5)
                    {
                        l.Content = "Hit";
                    }
                    else
                    {
                        l.Content = getStatus(x, y).ToString();
                    }
                    g.Children.Add(l);
                }
            }

            g.ShowGridLines = true;
            return g;
        }
  

        private BlackJackMove getStatus(Card dealersCard, List<Card> playersHand)
        {
            //  Add to the first Player_Statistics double, we will find the average value.
            Double firstWinPercentageAverage = 0;
            Double firstWinPercentageDeviationAverage = 0;
            foreach (Player_Statistics ps in PS_NoHit)
            {
                firstWinPercentageAverage =+ ps.getWinPercentage(dealersCard, playersHand);
            }
            firstWinPercentageAverage = firstWinPercentageAverage / PS_NoHit.Count;
            foreach (Player_Statistics ps in PS_NoHit)
            {
                firstWinPercentageDeviationAverage =+ Math.Abs(ps.getWinPercentage(dealersCard, playersHand) - firstWinPercentageAverage);
            }
            firstWinPercentageDeviationAverage = firstWinPercentageDeviationAverage / PS_NoHit.Count;

            //  Add to the first Player_Statistics double, we will find the average value.
            Double secondWinPercentageAverage = 0;
            Double secondWinPercentageDeviationAverage = 0;
            foreach (Player_Statistics ps in PS_Hit)
            {
                secondWinPercentageAverage += ps.getWinPercentage(dealersCard, playersHand);
            }
            secondWinPercentageAverage = secondWinPercentageAverage / PS_Hit.Count;
            foreach (Player_Statistics ps in PS_Hit)
            {
                secondWinPercentageDeviationAverage += Math.Abs(ps.getWinPercentage(dealersCard, playersHand) - secondWinPercentageAverage);
            }
            secondWinPercentageDeviationAverage = secondWinPercentageDeviationAverage / PS_Hit.Count;

            //  If the second win percentage average is within the bounds of the first win percentages deviations, then they are so close, that it
            //  should be considered a Hit.  Otherwise, it may be considered a Stand, and the only place things get so close is when
            //  the odds are actually very very close.  That happens with small cards whose total after a hit is less than 17.
            if ((secondWinPercentageAverage < (firstWinPercentageAverage + firstWinPercentageDeviationAverage)) & (secondWinPercentageAverage > (firstWinPercentageAverage - firstWinPercentageDeviationAverage)))
            {
                return BlackJackMove.Hit;
            }

            if (secondWinPercentageAverage > firstWinPercentageAverage)
            {
                if (((secondWinPercentageAverage + secondWinPercentageDeviationAverage) > 0.50) & ((secondWinPercentageAverage - secondWinPercentageDeviationAverage) > 0.50))
                {
                    return BlackJackMove.Double;
                }
                else
                {
                    return BlackJackMove.Hit;
                }
            }
            else
            {
                return BlackJackMove.Stand;
            }

        }

        private BlackJackMove getStatus(int dealersValue, int playersInitialHandValue)
        {
            //  Add to the first Player_Statistics double, we will find the average value.
            Double firstWinPercentageAverage = 0;
            Double firstWinPercentageDeviationAverage = 0;
            foreach (Player_Statistics ps in PS_NoHit)
            {
                firstWinPercentageAverage += ps.getWinPercentage(dealersValue, playersInitialHandValue);
            }
            firstWinPercentageAverage = firstWinPercentageAverage / PS_NoHit.Count;
            foreach (Player_Statistics ps in PS_NoHit)
            {
                firstWinPercentageDeviationAverage += Math.Abs(ps.getWinPercentage(dealersValue, playersInitialHandValue) - firstWinPercentageAverage);
            }
            firstWinPercentageDeviationAverage = firstWinPercentageDeviationAverage / PS_NoHit.Count;

            //  Add to the second Player_Statistics double, we will find the average value.
            Double secondWinPercentageAverage = 0;
            Double secondWinPercentageDeviationAverage = 0;
            foreach (Player_Statistics ps in PS_Hit)
            {
                secondWinPercentageAverage += ps.getWinPercentage(dealersValue, playersInitialHandValue);
            }
            secondWinPercentageAverage = secondWinPercentageAverage / PS_Hit.Count;
            foreach (Player_Statistics ps in PS_Hit)
            {
                secondWinPercentageDeviationAverage += Math.Abs(ps.getWinPercentage(dealersValue, playersInitialHandValue) - secondWinPercentageAverage);
            }
            secondWinPercentageDeviationAverage = secondWinPercentageDeviationAverage / PS_Hit.Count;

            //  If the second win percentage average is within the bounds of the first win percentages deviations, then they are so close, that it
            //  should be considered a Hit.  Otherwise, it may be considered a Stand, and the only place things get so close is when
            //  the odds are actually very very close.  That happens with small cards whose total after a hit is less than 17.
            if ((secondWinPercentageAverage < (firstWinPercentageAverage + firstWinPercentageDeviationAverage)) & (secondWinPercentageAverage > (firstWinPercentageAverage - firstWinPercentageDeviationAverage)))
            {
                return BlackJackMove.Hit;
            }

            if (secondWinPercentageAverage > firstWinPercentageAverage)
            {
                if (((secondWinPercentageAverage + secondWinPercentageDeviationAverage) > 0.50) & ((secondWinPercentageAverage - secondWinPercentageDeviationAverage) > 0.50))
                {
                    return BlackJackMove.Double;
                }
                else
                {
                    return BlackJackMove.Hit;
                }
            }
            else
            {
                return BlackJackMove.Stand;
            }

        }
        public enum BlackJackMove
        { 
        Stand=0, Double=1, Hit=2
        }
    }
}
