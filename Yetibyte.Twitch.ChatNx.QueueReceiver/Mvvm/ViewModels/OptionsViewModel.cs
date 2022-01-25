using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Yetibyte.Twitch.ChatNx.QueueReceiver.Models;
using Yetibyte.Twitch.ChatNx.QueueReceiver.Services;

namespace Yetibyte.Twitch.ChatNx.QueueReceiver.Mvvm.ViewModels
{
    public class OptionsViewModel : INotifyPropertyChanged
    {
        private readonly DelegateCommand _restoreDefaultCommand;

        public int? Port => Options.Port;

        public ApplicationOptions Options { get; private set; }

        public ICommand RestoreDefaultCommand => _restoreDefaultCommand;

        public event PropertyChangedEventHandler PropertyChanged;

        public OptionsViewModel(ApplicationOptions options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            Options = ApplicationOptions.CreateDefault();
            Options.Copy(options);

            _restoreDefaultCommand = new DelegateCommand(ExecuteRestoreDefaultCommand, DelegateCommand.AlwaysTruePredicate);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ExecuteRestoreDefaultCommand(object parameter)
        {
            ApplicationOptions defaultOptions = ApplicationOptions.CreateDefault();

            Options.Copy(defaultOptions);
        }
    }
}
