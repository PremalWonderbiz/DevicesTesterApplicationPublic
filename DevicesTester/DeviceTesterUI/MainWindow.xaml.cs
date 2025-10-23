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
using DeviceTesterCore.Interfaces;
using DeviceTesterCore.Models;
using DeviceTesterUI.ViewModels;
using DeviceTesterUI.Views;
using Microsoft.Extensions.DependencyInjection;

namespace DeviceTesterUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IToastService _toastService;
        private const string ViewKey = "MainWindowLeft";
        private const string ViewKey2 = "MainWindowRight";
        public MainWindow(DeviceViewModel deviceViewModel)
        {
            InitializeComponent();
            this.DataContext = deviceViewModel;
            _toastService = App.serviceProvider.GetService<IToastService>()!;
            _toastService.ToastRequested += OnToastRequested;
        }

        private void OnToastRequested(ToastMessage toast)
        {
            if (toast.ViewKey == null || toast.ViewKey == ViewKey)
            {
                Dispatcher.Invoke(() =>
                {
                    Brush color = toast.Level switch
                    {
                        ToastLevel.Success => Brushes.Green,
                        ToastLevel.Info => Brushes.LightBlue,
                        ToastLevel.Warning => Brushes.Orange,
                        ToastLevel.Error => Brushes.Red,
                        _ => Brushes.Gray
                    };

                    Toast.Show(toast.Message, color);
                });
            }

            if (toast.ViewKey == null || toast.ViewKey == ViewKey2)
            {
                Dispatcher.Invoke(() =>
                {
                    Brush color = toast.Level switch
                    {
                        ToastLevel.Success => Brushes.Green,
                        ToastLevel.Info => Brushes.LightBlue,
                        ToastLevel.Warning => Brushes.Orange,
                        ToastLevel.Error => Brushes.Red,
                        _ => Brushes.Gray
                    };

                    RightToast.Show(toast.Message, color);
                });
            }
        }

        
    }
}