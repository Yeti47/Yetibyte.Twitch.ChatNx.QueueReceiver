using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Yetibyte.Twitch.ChatNx.QueueReceiver.Mvvm
{
    public class DelegateCommand<T, P> : ICommand
    {

        private const string ERROR_PARAM_TYPE_MISMATCH = "The command's parameter is not of the correct type. Please make sure it matches the type given as the DelegateCommand's type parameter.";

        private readonly Action<T> _executeAction;
        private readonly Predicate<P> _canExecutePredicate;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<T> executeAction, Predicate<P> canExecutePredicate)
        {
            _executeAction = executeAction;
            _canExecutePredicate = canExecutePredicate;
        }

        public bool CanExecute(object parameter) => _canExecutePredicate?.Invoke(parameter == null ? default(P) : (parameter is P typedParam ? typedParam : throw new ArgumentException(ERROR_PARAM_TYPE_MISMATCH))) ?? false;

        public void Execute(object parameter) => _executeAction?.Invoke(parameter == null ? default(T) : (parameter is T typedParam ? typedParam : throw new ArgumentException(ERROR_PARAM_TYPE_MISMATCH)));

        public void RaiseCanExecuteChangedEvent()
        {

            EventHandler handler = CanExecuteChanged;
            handler?.Invoke(this, EventArgs.Empty);

        }

    }

    public class DelegateCommand : DelegateCommand<object, object>
    {
        public static Predicate<object> AlwaysTruePredicate { get; } = new Predicate<object>(o => true);

        public DelegateCommand(Action<object> executeAction, Predicate<object> canExecutePredicate) : base(executeAction, canExecutePredicate)
        {

        }

    }
}
