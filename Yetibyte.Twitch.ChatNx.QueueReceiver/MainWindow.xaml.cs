using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Yetibyte.Twitch.ChatNx.QueueReceiver.Mvvm.ViewModels;
using Yetibyte.Twitch.ChatNx.QueueReceiver.Services;

namespace Yetibyte.Twitch.ChatNx.QueueReceiver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly OptionService _optionService;

        private bool _isQueueAutoScrollEnabled;
        private bool _isHistoryAutoScrollEnabled;

        public MainViewModel ViewModel { get; private set; }

        public MainWindow(ICommandQueueWebSocketServer commandQueueWebSocketServer, OptionService optionService)
        {
            if (commandQueueWebSocketServer is null)
            {
                throw new ArgumentNullException(nameof(commandQueueWebSocketServer));
            }

            _optionService = optionService ?? throw new ArgumentNullException(nameof(optionService));

            DataContext = ViewModel = new MainViewModel(commandQueueWebSocketServer, optionService);
            InitializeComponent();
        }

        private void scrollQueue_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer scrollViewer = sender as ScrollViewer;

            if (sender is null)
                return;

            HandleAutoScroll(e, scrollViewer, ref _isQueueAutoScrollEnabled);
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer scrollViewer = sender as ScrollViewer;

            if (sender is null)
                return;

            HandleAutoScroll(e, scrollViewer, ref _isHistoryAutoScrollEnabled);
        }

        private void HandleAutoScroll(ScrollChangedEventArgs e, ScrollViewer scrollViewer, ref bool isAutoScrollEnabled)
        {
            if (Math.Abs(e.ExtentHeightChange) <= double.Epsilon)
            {
                isAutoScrollEnabled = scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight;
            }

            if (isAutoScrollEnabled && Math.Abs(e.ExtentHeightChange) > double.Epsilon)
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.ExtentHeight);
            }
        }

        private void ScrollViewer_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is not ScrollViewer scrollViewer)
                return;

            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        }

        private void ScrollViewer_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is not ScrollViewer scrollViewer)
                return;

            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }

        private void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            OptionsViewModel optionsViewModel = new OptionsViewModel(ViewModel.Options);
            OptionsWindow optionsWindow = new OptionsWindow(optionsViewModel);

            bool? dialogResult =optionsWindow.ShowDialog();

            if (dialogResult.GetValueOrDefault())
            {
                ViewModel.Options.Copy(optionsViewModel.Options);

                _optionService.SaveOptions(ViewModel.Options);
            }
        }

        private void scrollQueue_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is not ScrollViewer scrollViewer)
                return;

            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        }

        private void scrollQueue_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is not ScrollViewer scrollViewer)
                return;

            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }
    }
}
