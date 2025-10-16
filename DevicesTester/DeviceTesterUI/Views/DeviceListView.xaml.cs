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
using DeviceTesterCore.Interfaces;
using DeviceTesterCore.Models;
using DeviceTesterServices.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DeviceTesterUI.Views
{
    /// <summary>
    /// Interaction logic for DeviceListView.xaml
    /// </summary>
    public partial class DeviceListView : UserControl
    {
        private readonly IToastService _toastService;
        private const string ViewKey = "DeviceList";

        public DeviceListView()
        {
            InitializeComponent();
            _toastService = App.serviceProvider.GetService<IToastService>()!;
            _toastService.ToastRequested += OnToastRequested;
        }

        private void OnToastRequested(ToastMessage toast)
        {
            if (toast.ViewKey == null || toast.ViewKey == ViewKey)
            {
                Dispatcher.Invoke(() =>
                {
                    // Map ToastLevel to WPF Brush
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
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _toastService.ToastRequested -= OnToastRequested;
        }
    }
}
