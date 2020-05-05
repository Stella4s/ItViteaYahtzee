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
        //Private fields
        private Dice[] _DiceArr;
        private Random rnd = new Random();
        private ScoreBar[] _ScoreGrid;

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
            InitialiseDice();

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
        #endregion

        #region Methdods relating to DiceArray
        //Method for Initialising/Resetting Dice Array.
        public void InitialiseDice()
        {
            for (int i = 0; i < 5; i++)
            {
                DiceArr[i].Number = i;
                DiceArr[i].IsHeld = false;
            }
        }
        //Methods to roll the dice in the dice array.
        public void RollDice()
        {
            for (int i = 0; i < 5; i++)
            {
                //If the die is NOT held, then roll.
                if (!DiceArr[i].IsHeld)
                    DiceArr[i].Number = rnd.Next(1, 7);
            }
            UpdateScoreGrid();
        }
        #endregion

        #region Methods relating to ScoreGrid
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

            //Switch case for ThreeOfaKind, FourOfAKind and checking Yahtzee.
            bool isYahtzee = false;
            points = GetSumAllDice();
            switch (NumberOfAKind())
            {
                case 0:
                    UpdateScoreBar(8, 0);
                    UpdateScoreBar(9, 0);
                    break;
                case 3:
                    UpdateScoreBar(8, points);
                    UpdateScoreBar(9, 0);
                    break;
                case 4:
                    UpdateScoreBar(8, points);
                    UpdateScoreBar(9, points);
                    break;
                case 5:
                    UpdateScoreBar(8, points);
                    UpdateScoreBar(9, points);
                    isYahtzee = true;
                    break;
                default:
                    break;
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
