﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ItViteaYahtzee
{
    public class RelayCommand : ICommand
    {
        private Action commandTask;

        public RelayCommand(Action doAction)
        {
            commandTask = doAction;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            commandTask();
        }
    }
}

