using System;
using System.Windows.Input;

public class RelayCommand : ICommand
{
    #region Local Members

    readonly Action<object> _execute;
    readonly Predicate<object> _canExecute;

    #endregion

    #region Constructors

    public RelayCommand(ICommand command, Action<object> execute)
        : this(execute, null)
    {
    }


    public RelayCommand(Action<object> execute, Predicate<object> canExecute)
    {
        if (execute == null)
        {
            throw new ArgumentNullException("execute");
        }

        _execute = execute;
        _canExecute = canExecute;
    }

    #endregion

    #region ICommand Implementation

    public bool CanExecute(object parameter)
    {
        return _canExecute == null || _canExecute(parameter);
    }

    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public void Execute(object parameter)
    {
        _execute(parameter);
    }

    #endregion
}

