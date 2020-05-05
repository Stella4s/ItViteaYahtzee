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
            int Sum;
            for (int i = 0; i < 6; i++)
            {
                Sum = GetSumSameDice(i + 1);
                ScoreGrid[i].Points = Sum;
                if (Sum != 0)
                    ScoreGrid[i].IsValid = true;
                else
                    ScoreGrid[i].IsValid = false;
            }
               
        }

        public int GetSumSameDice( int dieNum)
        {
            int Sum = 0;
            for (int i = 0; i < 5; i++)
            {
                if (DiceArr[i].Number == dieNum)
                {
                    Sum += dieNum;
                }
            }
            return Sum;
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
