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
using System.Windows.Shapes;
using Yetibyte.Twitch.ChatNx.QueueReceiver.Mvvm.ViewModels;

namespace Yetibyte.Twitch.ChatNx.QueueReceiver
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        public OptionsViewModel ViewModel { get; private set; }

        public OptionsWindow(OptionsViewModel optionsViewModel)
        {
            if (optionsViewModel is null)
            {
                throw new ArgumentNullException(nameof(optionsViewModel));
            }

            DataContext = ViewModel = optionsViewModel;
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnOkay_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
