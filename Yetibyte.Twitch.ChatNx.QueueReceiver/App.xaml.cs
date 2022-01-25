using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Yetibyte.Twitch.ChatNx.QueueReceiver.Models;
using Yetibyte.Twitch.ChatNx.QueueReceiver.Mvvm.ViewModels;
using Yetibyte.Twitch.ChatNx.QueueReceiver.Services;

namespace Yetibyte.Twitch.ChatNx.QueueReceiver
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainViewModel _mainViewModel;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ICommandQueueWebSocketServer commandQueueWebSocketServer = new CommandQueueWebSocketServer(QueueStatusCallBack);
            //ICommandQueueWebSocketServer commandQueueWebSocketServer = new MockCommandQueueWebSocketServer();

            OptionService optionService = new OptionService();

            MainWindow = new MainWindow(commandQueueWebSocketServer, optionService);
            MainWindow.Show();

            _mainViewModel = ((MainWindow)MainWindow).ViewModel;
        }

        private QueueStatus QueueStatusCallBack()
        {

            int itemCount = _mainViewModel.QueueItemCount;
            int historyCount = _mainViewModel.HistoryItemCount;

            string[] itemIds = _mainViewModel.CommandQueueItems
                .Concat(new[] { _mainViewModel.CurrentItem })
                .Where(i => i is not null).Select(i => i.Data?.Id ?? string.Empty).ToArray();

            return new QueueStatus
            {
                HistoryItemCount = historyCount,
                QueueItemCount = itemCount,
                QueueItemIds = itemIds
            };
        }
    }
}
