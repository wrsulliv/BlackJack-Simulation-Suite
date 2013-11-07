using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Threading;
using System.Globalization;
using D3PaletteControl;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Runner runner;
        ProcessRunner proc_runner;
        delegate void ProgressBarCodeDelegate(object[] parameters);
        delegate void ProgressBar_ProcessCodeDelegate(object[] parameters);
        delegate void DevationIterationDelegate(object[] parameters);
        int DeckCount;
        int DevIter;
        int DeckIter;
        Deck.ShuffleType shuffleType;

        public MainWindow()
        {
            InitializeComponent();
        }
        private void ProgressBarCode(object[] parameters)
        {
            int totalTicks = (int)parameters[0];
            int ticksUsed = (int)parameters[1];
            double minsRemaining = (double)parameters[2];
            string minsRemainingString = "Unknown";
            if (!Double.IsInfinity(minsRemaining) & !Double.IsNaN(minsRemaining))
            {
                if (!(minsRemaining == 0))
                {
                    minsRemainingString = minsRemaining.ToString().Substring(0, minsRemaining.ToString().IndexOf(".")) + ":";
                    minsRemainingString += minsRemaining.ToString().Substring(minsRemaining.ToString().IndexOf(".") + 1, 2);
                }
                else
                {
                    minsRemainingString = "0";
                }
            }
            lbl_MinsRemaining.Dispatcher.Invoke(new Action<string>(delegate(string mins) { lbl_MinsRemaining.Content = mins; lbl_MinsRemaining.InvalidateVisual(); }), System.Windows.Threading.DispatcherPriority.Render, minsRemainingString);
            progressBar1.Dispatcher.Invoke(new Action<double>(delegate(double incValue) { progressBar1.Value = incValue; progressBar1.InvalidateVisual(); }), System.Windows.Threading.DispatcherPriority.Render, ((double)ticksUsed / (double)totalTicks) * 100);
        }

        private void ProgressBarCode_Process(object[] parameters)
        {
            lbl_first.Dispatcher.Invoke(new Action<string>(delegate(string first) { lbl_first.Content = first; lbl_first.InvalidateVisual(); }), System.Windows.Threading.DispatcherPriority.Render, (string)parameters[0]);
            lbl_second.Dispatcher.Invoke(new Action<string>(delegate(string second) { lbl_second.Content = second; lbl_second.InvalidateVisual(); }), System.Windows.Threading.DispatcherPriority.Render, (string)parameters[1]);
            lbl_third.Dispatcher.Invoke(new Action<string>(delegate(string third) { lbl_third.Content = third; lbl_third.InvalidateVisual(); }), System.Windows.Threading.DispatcherPriority.Render, (string)parameters[2]);
            lbl_fourth.Dispatcher.Invoke(new Action<string>(delegate(string fourth) { lbl_fourth.Content = fourth; lbl_fourth.InvalidateVisual(); }), System.Windows.Threading.DispatcherPriority.Render, (string)parameters[3]);

        }

        private void DeviationIterationCode(object[] parameters)
        {
            int devIterationsRemaining = (int)parameters[0];
            lbl_DevIterations.Dispatcher.Invoke(new Action<int>(delegate(int devIterRemaining) { lbl_DevIterations.Content = devIterRemaining.ToString(); lbl_DevIterations.InvalidateVisual(); }), System.Windows.Threading.DispatcherPriority.Render, devIterationsRemaining);
        }
        private void runRunner()
        {
            ProgressBarCodeDelegate progressBarCodeDel = ProgressBarCode;
            DevationIterationDelegate deviationIterationDel = DeviationIterationCode;
            this.runner = new Runner(progressBarCodeDel,deviationIterationDel,this.DevIter , this.DeckIter, this.DeckCount, this.shuffleType);
            this.finishRunner();
        }
        private void finishRunner()
        {

            this.Dispatcher.Invoke(new Action(finishRunnerCode), System.Windows.Threading.DispatcherPriority.Render, null);
        }
        private void finishProcessRunner()
        {
            this.Dispatcher.Invoke(new Action(finishProcessRunnerCode), System.Windows.Threading.DispatcherPriority.Render, null);
        }
        private void finishProcessRunnerCode()
        {
             Grid gridBasicStrategy = new Grid();
             gridBasicStrategy = this.proc_runner.generateResultGrid_HardHands();
             grid_BSHard.Children.Add(gridBasicStrategy);
        }
        private void finishRunnerCode()
        {
            Grid gridBasicStrategy = new Grid();
            gridBasicStrategy = runner.generateResultGrid_HardHands();
            grid_BSHard.Children.Add(gridBasicStrategy);
            this.Show();
            this.InvalidateVisual();
            this.lbl_Decks.Content = "8";
            this.lbl_H17.Content = "No";
            this.lbl_TotGames.Content = runner.getTotalGameCount().ToString();
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
                this.DeckCount = int.Parse(txt_Decks.Text);
                this.DeckIter = int.Parse(txt_DeckIter.Text);
                this.DevIter = int.Parse(txt_DevIter.Text);
                if (radio_Durstenfeld.IsChecked.Value) { this.shuffleType = Deck.ShuffleType.Durstenfeld; }
                else if (radio_Sullivan.IsChecked.Value) { this.shuffleType = Deck.ShuffleType.Sullivan; }
                else if (radio_Other.IsChecked.Value) { this.shuffleType = Deck.ShuffleType.Other; }

                ThreadStart threadStart = new ThreadStart(runRunner);
                Thread thread = new Thread(threadStart);
                thread.Start();
                button1.Visibility = System.Windows.Visibility.Hidden;

        }

        private TabItem findTab(string name)
        {
            foreach (TabItem ti in tabControl1.Items)
            {
                if (ti.Name == name)
                {
                    return ti;
                }
            }
            return null;
        }

        private void runProcessRunners()
        {
             ProgressBar_ProcessCodeDelegate prog_proc_del = ProgressBarCode_Process;
             BlackjackProcessor bp = new BlackjackProcessor();
             this.proc_runner = bp.Start(prog_proc_del);
             this.finishProcessRunner();
           
        }
        private void button2_Click(object sender, RoutedEventArgs e)
        {

            ThreadStart threadStart = new ThreadStart(runProcessRunners);
            Thread thread = new Thread(threadStart);
            thread.Start();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            Combinatorial_Analyzer sg = new Combinatorial_Analyzer();
            BSInfo[,] resultArray = sg.ComputeStrategy();
            string results = "";
            
            //  Reset the BSInfoArray Object
            for (int y = 5; y < 22; y++)
            {
                for (int x = 1; x < 11; x++)
                {

                    //results += " | " + decimal.Round((decimal)(resultArray[x, y].ev), 6).ToString() + " | ";

                    switch (resultArray[x, y].blackJackMove.ToString())
                    {
                        case "Stay":
                            results += " | Sty | ";
                            break;
                        case "Hit":
                            results += " | Hit | ";
                            break;
                        case "Double":
                            results += " | Dbl | ";
                            break;


                    }
                }
                results += "\n";
            }
            System.IO.File.WriteAllText("D:/bjs.txt", results);

            BSInfo[,] soft_resultArray = sg.BSInfoArray_Soft;
            results = "";
            //  Reset the BSInfoArray Object
            for (int y = 5; y < 22; y++)
            {
                for (int x = 1; x < 11; x++)
                {

                    switch (soft_resultArray[x, y].blackJackMove.ToString())
                    { 
                        case "Stay":
                            results += " | Sty | ";
                            break;
                        case "Hit":
                            results += " | Hit | ";
                            break;
                        case "Double":
                            results += " | Dbl | ";
                            break;


                    }
                    //results += " | " + decimal.Round((decimal)(soft_resultArray[x, y].ev), 6).ToString() + " | ";
                    
                }
                results += "\n";
            }
            System.IO.File.WriteAllText("D:/bjs_soft.txt", results);
     
        }
        private Card convertStringToCard(string cardString)
        { 
            switch(cardString)
            {
                case "Ace":
                    return new Card(CardValue.Ace, CardSuit.Hearts);
                case "Two":
                    return new Card(CardValue.Two, CardSuit.Hearts);
                case "Three":
                    return new Card(CardValue.Three, CardSuit.Hearts);
                case "Four":
                    return new Card(CardValue.Four, CardSuit.Hearts);
                case "Five":
                    return new Card(CardValue.Five, CardSuit.Hearts);
                case "Six":
                    return new Card(CardValue.Six, CardSuit.Hearts);
                case "Seven":
                    return new Card(CardValue.Seven, CardSuit.Hearts);
                case "Eight":
                    return new Card(CardValue.Eight, CardSuit.Hearts);
                case "Nine":
                    return new Card(CardValue.Nine, CardSuit.Hearts);
                case "Ten":
                    return new Card(CardValue.Ten, CardSuit.Hearts);
                default:
                    return new Card(CardValue.Ten, CardSuit.Hearts);

            }


        }

        private void button2_Click_1(object sender, RoutedEventArgs e)
        {
            BlackJackGameParams blackJackGameParams = new BlackJackGameParams();
            blackJackGameParams.bankroll = int.Parse(txt_BankRoll.Text); //  TODO
            blackJackGameParams.betSpread = int.Parse(txt_BetSpread.Text); // Only if not using a betting ramp
            if ((bool)radio_KO.IsChecked)
            {
                blackJackGameParams.countingMethod = CountingMethod.KO; //  TODO: Add Hi-Lo
            }
            else
            {
                blackJackGameParams.countingMethod = CountingMethod.NONE; //  TODO: Add Hi-Lo
            }
            
            blackJackGameParams.DAS = (bool)check_DAS.IsChecked; // TODO
            blackJackGameParams.deckPenetration = Double.Parse(txt_Penetration.Text);
            blackJackGameParams.H17 = (bool)check_H17.IsChecked;
            blackJackGameParams.hoursOfPlay = int.Parse(txt_HoursOfPlay.Text);
            blackJackGameParams.maxBet = int.Parse(txt_MaxBet.Text);
            blackJackGameParams.minBet = int.Parse(txt_MinBet.Text);
            blackJackGameParams.numDecks = int.Parse(txt_DeckCount.Text);
            blackJackGameParams.numPlayers = int.Parse(txt_NumPlayers.Text);
            blackJackGameParams.RSA = (bool)check_RSA.IsChecked; //  TODO
            blackJackGameParams.Surrender = (bool)check_Surrender.IsChecked; // TODO
            blackJackGameParams.tablePosition = int.Parse(txt_TablePosition.Text);
            blackJackGameParams.useBetRamp = (bool)check_BetRamp.IsChecked;

            BasicStrategySimulator bss = new BasicStrategySimulator(blackJackGameParams);

            //TestSuite ts = new TestSuite();
            //ts.test_BSPlayer_playHand(blackJackGameParams, "d:/unittest.test");
            //ts.test_CardCounter(blackJackGameParams);
           
            bss.BeginSimulation(int.Parse(txt_NumberOfTrials.Text));

            List<Double> bankRollVsGames = bss.getBankRoll_vs_GamesPlayed();
            List<Point> points = new List<Point>();
            for (int i = 0; i < bankRollVsGames.Count; i++)
            {
                points.Add(new Point(i, bankRollVsGames[i]));
            }
            EnumerableDataSource<Point> bankrollData = new EnumerableDataSource<Point>(points);

            Func<Point, Point> func = s => new Point(s.X, s.Y);
            myChartPlotter.RemoveUserElements();
            bankrollData.SetXYMapping(func);
            myChartPlotter.MaxWidth = points.Count;
            myChartPlotter.MaxHeight = bankRollVsGames.Max();
            myChartPlotter.AddLineGraph(bankrollData, Colors.LightBlue, 2, "BankRoll vs Games Played");

            txt_Results.Text = "You made: $" + (bss.getFinalBankrollAverage() - (Double)blackJackGameParams.bankroll).ToString() + "\nAverage Ending Bankroll: $" + bss.getFinalBankrollAverage().ToString() + "\nStd Devation: " + bss.getFinalBankrollStandardDevation().ToString() + "\nBelow Percentage: " + bss.getPercentageBelowAverage().ToString() + "\nAbove Average: " + bss.getPercentageAbove_EqualAverage().ToString() + "\nAverage Below Mean: $" + bss.getAverageOfThoseBelowTheMean().ToString() + "\nAverage Above Mean: $" + bss.getAverageOfThoseAbove_EqualTheMean().ToString();

        }

    }
}
