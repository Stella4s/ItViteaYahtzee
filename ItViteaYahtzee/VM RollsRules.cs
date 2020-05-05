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
        }

        //Public properties.
        public Dice[] DiceArr
        {
            get { return _DiceArr; }
            set
            {
                _DiceArr = value;
                OnPropertyChanged();
            }
        }

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
