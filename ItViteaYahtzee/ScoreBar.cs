    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace ItViteaYahtzee
{
    public class ScoreBar : INotifyPropertyChanged
    {
        //private fields for holding the values of public properties.
        private string _Name;
        private int _Points;
        private bool _IsValid, _IsUsed, _AllowClick = true;

        //public properties
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged();
            }
        }
        public int Points
        {
            get { return _Points; }
            set
            {
                _Points = value;
                OnPropertyChanged();
            }
        }
        public bool IsValid
        {
            get { return _IsValid; }
            set
            {
                _IsValid = value;
                OnPropertyChanged();
            }
        }
        public bool IsUsed
        {
            get { return _IsUsed; }
            set
            {
                _IsUsed = value;
                OnPropertyChanged();
                OnIsUsedChanged(EventArgs.Empty);
            }
        }
        public bool AllowClick
        {
            get { return _AllowClick; }
            set
            {
                _AllowClick = value;
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

        public event EventHandler IsUsedChanged;
        private void OnIsUsedChanged(EventArgs e)
        {
            IsUsedChanged?.Invoke(this, e);
        }
        #endregion
    }
}
