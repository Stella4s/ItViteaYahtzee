using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ItViteaYahtzee
{
    public class VM_RollsRules : INotifyPropertyChanged
    {
        //Private fields for properties.
        private Dice[] _DiceArr;
        private Random rnd = new Random();
        private ScoreBar[] _ScoreGrid;
        private int _DiceRoll;
        private bool _NoNotedPoints;
        private int _EndGameOpacity;
        private int _TotalScore;

        public VM_RollsRules()
        {
            _DiceArr = new Dice[]
            {
                new Dice {Number = 1, IsHeld = false },
                new Dice {Number = 1, IsHeld = false },
                new Dice {Number = 1, IsHeld = false },
                new Dice {Number = 1, IsHeld = false },
                new Dice {Number = 1, IsHeld = false }
            };

            _ScoreGrid = new ScoreBar[]
            {
                new ScoreBar{Name = "Ones"},
                new ScoreBar{Name = "Twos"},
                new ScoreBar{Name = "Threes"},
                new ScoreBar{Name = "Fours"},
                new ScoreBar{Name = "Fives"},
                new ScoreBar{Name = "Sixes"},
                new ScoreBar{Name = "Sum", AllowClick = false},
                new ScoreBar{Name = "Bonus", AllowClick = false},
                new ScoreBar{Name = "Three of a kind"},
                new ScoreBar{Name = "Four of a kind"},
                new ScoreBar{Name = "Full House"},
                new ScoreBar{Name = "Small straight"},
                new ScoreBar{Name = "Large straight"},
                new ScoreBar{Name = "Chance"},
                new ScoreBar{Name = "Yahtzee"},
                new ScoreBar{Name = "Total Score", AllowClick = false}
            };
            InitializeScoreGrid();
            StartGame();
        }

        #region Public properties.
        public Dice[] DiceArr
        {
            get { return _DiceArr; }
            set
            {
                _DiceArr = value;
                OnPropertyChanged();
            }
        }
        public ScoreBar[] ScoreGrid
        {
            get { return _ScoreGrid; }
            set
            {
                _ScoreGrid = value;
                OnPropertyChanged();
            }
        }
        public int DiceRoll
        {
            get { return _DiceRoll; }
            set
            {
                _DiceRoll = value;
                OnPropertyChanged();
            }
        }
        public bool NoNotedPoints
        {
            get { return _NoNotedPoints; }
            set
            {
                _NoNotedPoints = value;
                OnPropertyChanged();
            }
        }
        public int EndGameOpacity
        {
            get { return _EndGameOpacity; }
            set
            {
                _EndGameOpacity = value;
                OnPropertyChanged();
            }
        }
        public int TotalScore
        {
            get { return _TotalScore; }
            set
            {
                _TotalScore = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region General Game Methods
        /// <summary>
        /// Hides endgame area, resets Total score. Invokes other support methods for resetting.
        /// </summary>
        public void StartGame()
        {
            EndGameOpacity = 0;
            TotalScore = 0;
            NextTurn();
            ResetScoreGrid();
        }

        public void NextTurn()
        {
            //Reset notedPoints to false.
            NoNotedPoints = true;
            //Reset DiceHolds.
            ResetDice();
            yahtzeeUpdateSwitch = true;
        }

        ///<summary>Updates the points being used to true to keep track of if the player has noted their points for this round.
        ///This does not immediately go to the next turn as to prevent the user submitting the same points a second time.
        ///</summary>
        public void UpdateIsUsed(object sender, EventArgs e)
        {
            NoNotedPoints = false;
            //Reset diceroll to 0.
            DiceRoll = 0;
            //Reset DiceHolds.
            ResetDice();
            //Updates ScoreGrid to reflect changes.
            yahtzeeUpdateSwitch = false;
            UpdateScoreGrid();
        }


        public void GameFinish()
        {
            //Add all points from ScoreGrid together for TotalScore. And make EndGame area visible.
            TotalScore = ScoreGrid[15].Points;
            EndGameOpacity = 1;
        }
        #endregion 

        #region Methdods relating to DiceArray
        /// <summary>
        /// Method for Initialising/Resetting Dice Array.
        /// </summary>
        public void ResetDice()
        {
            for (int i = 0; i < 5; i++)
            {
                DiceArr[i].IsHeld = false;
            }
        }

        /// <summary>
        /// Method to roll the dice in the dice array. 
        /// Or not roll dice depending on properties.
        /// </summary>
        public void RollDice()
        {
            if (!NoNotedPoints)
                NextTurn();

            //Roll the dice if they have been rolled less than 3 times.
            //And if the EndGameOpacity = 0.
            if (DiceRoll < 3 && EndGameOpacity == 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    //If the die is NOT held, then roll.
                    if (!DiceArr[i].IsHeld)
                        DiceArr[i].Number = rnd.Next(1, 7);
                }
                UpdateScoreGrid();
                DiceRoll++;
            }
        }
        #endregion

        #region Methods relating to ScoreGrid

        /// <summary>
        /// Subscribes the UpdateIsUsed event to all the bars.
        /// And sets the proper index for each bar.
        /// </summary>
        public void InitializeScoreGrid()
        {
            int i = 0;
            //Connected UpdateIsUsed to IsUsedChanged Event.
            foreach (ScoreBar bar in ScoreGrid)
            {
                bar.Index = i;
                bar.IsUsedChanged += UpdateIsUsed;
                i++;
            }
        }

        /// <summary>
        /// Resets Points, IsUsed and IsValid back to starting values for game.
        /// </summary>
        public void ResetScoreGrid()
        {
            foreach (ScoreBar bar in ScoreGrid)
            {
                bar.IsUsed = false;
                bar.IsValid = false;
                bar.Points = 0;
            }
        }

        /// <summary>
        /// Updates every row in the ScoreGrid using the other supporting scoregrid methods. Is called after each DiceRoll.
        /// </summary>
        public void UpdateScoreGrid()
        {
            int points;
            //ScoreRows 1 to 6.
            for (int i = 0; i < 6; i++)
            {
                //For each number points (ones, twos, etc.) Calculates the sum and updates the points.
                points = GetSumSameDice(i + 1);
                UpdateScoreBar(i, points);
            }
            //Update ScoreRows Sum and Bonus.
            GetSumTop();

            //Set points to Sum of All Dice for ThreeOfAKind, FourOfAKind and Chance.
            points = GetSumAllDice();

            //Set Chance.
            UpdateScoreBar(13, points);

            //Switch case for ThreeOfaKind, FourOfAKind and Yahtzee.
            switch (NumberOfAKind())
            {
                case 0:
                    UpdateScoreBar(8, 0);
                    UpdateScoreBar(9, 0);
                    UpdateYahtzee(14, false);
                    break;
                case 3:
                    UpdateScoreBar(8, points);
                    UpdateScoreBar(9, 0);
                    UpdateYahtzee(14, false);
                    break;
                case 4:
                    UpdateScoreBar(8, points);
                    UpdateScoreBar(9, points);
                    UpdateYahtzee(14, false);
                    break;
                case 5:
                    UpdateScoreBar(8, points);
                    UpdateScoreBar(9, points);
                    UpdateYahtzee(14, true);
                    break;
                default:
                    break;
            }
            
            //Check for fullhouse.
            if (CheckFullHouse())
                points = 25;
            else
                points = 0;
            UpdateScoreBar(10, points);

            //Reset points to 0. 
            points = 0;
            //Checkstraight returns 4 for largestraight, 3 for smallstraight.
            //Any other number returned means no straight, leaving them both at 0 points.
            switch (CheckStraight())
            {
                case 3:
                    UpdateScoreBar(11, 30);
                    UpdateScoreBar(12, points);
                    break;
                case 4:
                    UpdateScoreBar(11, 30);
                    UpdateScoreBar(12, 40);
                    break;
                default:
                    UpdateScoreBar(11, points);
                    UpdateScoreBar(12, points);
                    break;
            }

            //Update TotalScore & CheckEntire grid for either no valid options or all options being used. 
            UpdateScore();
            CheckEntireScoreGrid();
        }

        /// <summary>
        /// Checking for if all Clickable bars are used. As well as if all remaining bars are unused and invalid. 
        /// To make sure the user can select the invalid bars for 0 points.
        /// </summary>
        public void CheckEntireScoreGrid()
        {
            //If all bars are used, the game is over.
            if (ScoreGrid.Select(group => new { group.AllowClick, group.IsUsed })
                .Where(c => c.AllowClick == true)
                .All(x => x.IsUsed))
            {
                GameFinish();
            }
  
            //If all unused, clickable options are not valid. Make them valid. (Using index to ONLY make those Valid.)
            var tempGrid = ScoreGrid.Select(group => new { group.IsValid, Used = group.IsUsed, group.AllowClick, group.Index })
                .Where(c => c.Used == false)
                .Where(c => c.AllowClick == true);

            if (tempGrid.All(x => x.IsValid.Equals(false)))
            {
                var tempArray = tempGrid.Select(x => x.Index).ToArray();
                foreach (int index in tempArray)
                {
                    ScoreGrid[index].IsValid = true;
                }
            }
          
        }

        /// <summary>
        /// Updates the scoregrid bar based on the index with the points given.
        /// If the points are 0 it will set the bar IsValid as false.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="points"></param>
        public void UpdateScoreBar(int index, int points)
        {
            //Do not update points if the row was used already.
            if (!ScoreGrid[index].IsUsed)
            {
                ScoreGrid[index].Points = points;

                //If the points are 0 there are no dice of that number. Making the row invalid.
                //As the user should not be able to select to use that bar to add to their score.
                if (points != 0)
                    ScoreGrid[index].IsValid = true;
                else
                    ScoreGrid[index].IsValid = false;
            }
        }

        /// <summary>
        /// Gets the sum of the top section of points depending on which ones are used.
        /// If all are used it will determine if the player gets bonus points and will set both rows as IsUsed.
        /// And will no longer go through all the calculations when the method is called upon.
        /// </summary>
        public void GetSumTop ()
        {
            //Check the sumrow IsUsed. If IsUsed == true. Do not go through all the calculations anymore as the sum has already been determined.
            if (!ScoreGrid[6].IsUsed)
            {
                int sum = 0, tracker = 0;
                for (int i = 0; i < 6; i++)
                {
                    //If the bar has been used for adding points. Add those points to the sum.
                    //Also for each used row add 1 to the tracker.
                    if (ScoreGrid[i].IsUsed)
                    {
                        sum += ScoreGrid[i].Points;
                        tracker++;
                    }
                }
                //If the tracker reaches 6 it means all number rows have been used.
                //Which means there is no need to re-calculate the sum anymore as it will not change.
                //And if the player gets the bonus points can be determined now.
                if (tracker == 6)
                {
                    //If the sum is 63 or more points get 35 bonus points. Otherwise 0.
                    ScoreGrid[6].IsUsed = true;
                    if (sum >= 63)
                        ScoreGrid[7].Points = 35;
                    else
                        ScoreGrid[7].Points = 0;
                    ScoreGrid[7].IsUsed = true;
                }
                ScoreGrid[6].Points = sum;
            }
        }

        ///<summary> 
        ///Calculates the sum of the dice that have the same number. 
        ///Being 0 if no dice are the selected dieNum.
        ///</summary>
        public int GetSumSameDice( int dieNum)
        {
            int sum = 0;
            for (int i = 0; i < 5; i++)
            {
                if (DiceArr[i].Number == dieNum)
                {
                    sum += dieNum;
                }
            }
            return sum;
        }
        /// <summary>
        /// Returns the sum of all the dice.
        /// </summary>
        /// <returns></returns>
        public int GetSumAllDice()
        {
            int sum = 0;
            for (int i = 0; i < 5; i++)
            {
                sum += DiceArr[i].Number;
            }
            return sum;
        }

        /// <summary>
        /// Returns how many dice are of the same kind. Returns 0 if there are less than 3 of the same kind.
        /// </summary>
        /// <param name="numSame"></param>
        /// <returns></returns>
        public int NumberOfAKind()
        {
            int sameKind = 0;
            for (int i = 1; i <= 6; i++)
            {
                int counter = 0;
                for (int j = 0; j < 5; j++)
                {
                    if (DiceArr[j].Number == i)
                        counter++;
                    if (counter > 2)
                        if (counter > sameKind)
                            sameKind = counter;
                }
            }
            return sameKind;
        }

        ///<summary>Checks if fullhouse is true or false.</summary>
        public bool CheckFullHouse()
        {
            bool isFullHouse = false;
            var houselist = DiceArr.Select(x => x.Number)
                .GroupBy(c => c)
                .Select(group => new { Num = group.Key, Count = group.Count() })
                .OrderBy(x => x.Num).ToList();

            if (houselist.Count == 2)
                foreach (var item in houselist)
                {
                    if (item.Count == 3)
                        isFullHouse = true;
                }
            return isFullHouse;
        }

        /// <summary>
        /// Checks for both small and large straight Returning 0, 3 or 4. 
        /// To be used with a switch for no straight, small straight or large straight.
        /// </summary>
        /// <returns></returns>
        public int CheckStraight()
        {
            //Take the dice and group them. (To take care of duplicate numbers. e.g. 1 2 3 3 4. Still counts as a smallstraight.)
            //Then order them and put them in a list.
            var straightList = DiceArr.Select(x => x.Number)
                .GroupBy(c => c)
                .Select(group => new { Num = group.Key, Count = group.Count() })
                .OrderBy(x => x.Num).ToList();

            int itemA = straightList[0].Num, 
                itemB, 
                counter = 0,
                consecutive = 0;
            //ItemA starts at smallest number. (Either 1 or 2). 
            //For loop makes ItemB ItemA. Then ItemA the number i, the next index in the list.
            //If ItemA - ItemB = 1 then the numbers are sequential and the counter goes up.
            //If counter == 4 all 5 numbers are sequential. Largestraight.
            //If counter == 3 4 numbers are sequential. Smallstraight.

            //If there are less than 4 different groups there is no straight and no need to check.
            if (straightList.Count > 3)
                for (int i = 1; i < straightList.Count; i++)
                {
                    itemB = itemA;
                    itemA = straightList[i].Num;
                    //Counter goes up for each item that only has 1 difference with the last. 
                    //Separate consecutive counter. (To prevent for example 1 2 3 5 6 being counted as a small straight.)
                    if (itemA - itemB == 1)
                    {
                        counter++;
                        consecutive++;
                    }
                    else
                        consecutive = 0;
                }
            //If the row of consecutive numbers is smaller than 3. It's not a straight.
            if (consecutive < 3)
                counter = 0;
            return counter;
        }

        public void UpdateScore()
        {
            //Select all Points from ScoreGrid from used bars and add them together. Then UpdateScoreBar of totalscore.
            int score = ScoreGrid.Select(group => new { group.Points, Used = group.IsUsed }).Where(c => c.Used == true).Sum(c => c.Points);
            UpdateScoreBar(15, score);
        }


        #region yahtzee variables
        /// <summary>
        /// Counter to keep track of how many times yahtzee has been selected.
        /// </summary>
        private int yahtzeeCounter = 0;
        /// <summary>
        /// yahtzeePoints allow for yahtzeebar's points to be returned to previous amount if the yahtzeebar was not selected by the user.
        /// It only updates if the ClickYahtzee method is called. 
        /// </summary>
        private int yahtzeePoints = 0;
        /// <summary>
        /// bool to switch between displaying the possible points and current yahtzeePoints.
        /// switch is set to true each turn. And false when UpdateIsUSed event is called.
        /// Setting the yathzeebar's points to yahtzeePoints
        /// </summary>
        private bool yahtzeeUpdateSwitch = true;
        #endregion

        /// <summary>
        /// Method called instead of UpdateScoreBar method for the Yahtzee bar. 
        /// Updates the points accordingly, determining the amount of bonus points with the yahtzeecounter.
        /// And using the yahtzeeUpdateSwitch to allow the possible new points to be visible.
        /// Whilst returning to the previous points if the user does not select the yahtzeebar.
        /// </summary>
        /// <param name="index">Used to determine which bar to update.</param>
        /// <param name="valid">A bool to determine if the method is called with a valid yahtzee or invalid. 
        /// Setting the yahtzeebar.IsValid to true or false to regulate it being clickable or not.</param>
        public void UpdateYahtzee(int index, bool valid)
        {
            //If method is called with valid = true there is a yahtzee. 
            if (valid)
            {
                //If Yahtzee isn't used with zero points. (Aka. Hasn't been given up for zero points.)
                if (!(ScoreGrid[index].IsUsed && ScoreGrid[index].Points == 0))
                {

                    //If the switch is true, updates to possible points.
                    //If false sets back to yahtzeePoints. Which are either the prior points or the newly updated points.
                    //Based on if ClickYahtzee() was invoked.
                    if (yahtzeeUpdateSwitch)
                    {
                        //Update the Yahtzee points. 50 initial points + 100 bonus points for each Yahtzee after.
                        ScoreGrid[index].Points = 50 + yahtzeeCounter * 100;
                    }
                    else
                        ScoreGrid[index].Points = yahtzeePoints;

                    //Set bar to valid to allow user to click button.
                    ScoreGrid[index].IsValid = true;
                }
            }
            else
                ScoreGrid[index].IsValid = false;
        }

        /// <summary>
        /// Called with Command to handle the finalizing of the yahtzee bar being selected.
        /// </summary>
        public void ClickYahtzee()
        {
            //Update the yahtzeePoints.
            yahtzeePoints = 50 + yahtzeeCounter * 100;
            //Set the yahtzeeClick bool to true & update the counter for amount of selected yahtzee's.
            yahtzeeCounter++;

            //Set yahtzeebar to used.
            ScoreGrid[14].IsUsed = true;

            //Now yahtzee has been clicked return IsValid to false to prevent it being clicked again until next yahtzee. 
            ScoreGrid[14].IsValid = false; 
        }


        #endregion

        #region game testing methods.

        /// <summary>
        /// Set all the dice numbers to the numbers selected in the comboboxes.
        /// </summary>
        public void WeightedDice()
        {
            if (!NoNotedPoints)
                NextTurn();

            //Roll the dice if the EndGameOpacity == 0.
            if (EndGameOpacity == 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    //If the die is NOT held, then roll.
                    if (!DiceArr[i].IsHeld)
                        DiceArr[i].Number = DiceArr[i].CheatNumber;
                }
                UpdateScoreGrid();
                DiceRoll++;
            }
        }
        #endregion

        #region Commands
        public ICommand GetDiceCmd
        {
            get
            {
                return new RelayCommand(RollDice);
            }
        }
        public ICommand NewGameCmd
        {
            get
            {
                return new RelayCommand(StartGame);
            }
        }
        public ICommand WeightedDiceCmd
        {
            get
            {
                return new RelayCommand(WeightedDice);
            }
        }
        public ICommand ClickYahtzeeCmd
        {
            get
            {
                return new RelayCommand(ClickYahtzee);
            }
        }
        #endregion



        #region INotifyPropertyChanged Members  
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
