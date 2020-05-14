using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace ItViteaYahtzee
{
    public class Dice : INotifyPropertyChanged
    {
        //Private fields for holding the values of public properties.
        int _Number, _CheatNumber;
        bool _IsHeld;
        int[] _NumberArray;

        public Dice()
        {
            _NumberArray = new int[]
            {
                1, 2, 3, 4, 5, 6
            };
        }

        //Public properties.
        //One to track the die number, one to track if the die is being held to check if it should be re-rolled or not.
        public int Number
        {
            get { return _Number; }
            set
            {
                _Number = value;
                OnPropertyChanged();
            }
        }

        public bool IsHeld
        {
            get { return _IsHeld; }
            set
            {
                _IsHeld = value;
                OnPropertyChanged();
            }
        }

        public int CheatNumber
        {
            get { return _CheatNumber; }
            set
            {
                _CheatNumber = value;
                OnPropertyChanged();
            }
        }

        public int[] NumberArray
        {
            get { return _NumberArray; }
            set
            {
                _NumberArray = value;
                OnPropertyChanged();
            }
        }

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
