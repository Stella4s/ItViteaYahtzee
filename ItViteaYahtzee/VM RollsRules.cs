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
            ResetDice();

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
            InitScoreGridEvents();
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
        #endregion

        #region General Game Methods
        //private fields for Game Methods.
        private bool notedPoints;

        public void NextTurn()
        {
            //Reset diceroll to 0.
            DiceRoll = 0;
            //Reset notedPoints to false.
            notedPoints = false;
            ResetDice();
        }

        ///<summary>Updates the points being used to true to keep track of if the player has noted their points for this round.
        ///This does not immediately go to the next turn as to prevent the user submitting the same points a second time.</summary>
        public void UpdateIsUsed(object sender, EventArgs e)
        {
            notedPoints = true;
            //Reset diceroll to 0.
            DiceRoll = 0;
        }
        #endregion 

        #region Methdods relating to DiceArray
        //Method for Initialising/Resetting Dice Array.
        public void ResetDice()
        {
            for (int i = 0; i < 5; i++)
            {
                //DiceArr[i].Number = i; Don't change numbers, probably not necessary.
                DiceArr[i].IsHeld = false;
            }
        }

        //Methods to roll the dice in the dice array.
        public void RollDice()
        {
            if (notedPoints)
                NextTurn();

            if (DiceRoll < 3)
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

        public void InitScoreGridEvents()
        {
            foreach (ScoreBar bar in ScoreGrid)
            {
                bar.IsUsedChanged += UpdateIsUsed;
            }
        }

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
                    UpdateScoreBar(14, 0);
                    break;
                case 3:
                    UpdateScoreBar(8, points);
                    UpdateScoreBar(9, 0);
                    UpdateScoreBar(14, 0);
                    break;
                case 4:
                    UpdateScoreBar(8, points);
                    UpdateScoreBar(9, points);
                    UpdateScoreBar(14, 0);
                    break;
                case 5:
                    UpdateScoreBar(8, points);
                    UpdateScoreBar(9, points);
                    UpdateScoreBar(14, 50);
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

            //Update TotalScore. Later moved elsewhere? 
            TotalScore();
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
                counter = 0;
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
                    if (itemA - itemB == 1)
                        counter++;
                }
            return counter;
        }

        public void TotalScore()
        {
            //Select all Points from ScoreGrid and add them together. Then UpdateScoreBar of totalscore.
            //int score = ScoreGrid.Select(x => x.Points).Sum();

            int score = ScoreGrid.Select(group => new { group.Points, Used = group.IsUsed }).Where(c => c.Used == true).Sum(c => c.Points);
            UpdateScoreBar(15, score);
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
