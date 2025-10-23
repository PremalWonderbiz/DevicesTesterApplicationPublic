using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml;
using DeviceTesterCore.Interfaces;
using DeviceTesterCore.Models;
using DeviceTesterUI.ViewModels;
using DeviceTesterUI.Windows;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeviceTesterUI.Views
{
    /// <summary>
    /// Interaction logic for DeviceDetailsView.xaml
    /// </summary>
    public partial class DeviceDetailsView : UserControl
    {
        private DeviceViewModel? _vm;
        private readonly IToastService _toastService;
        private const string ViewKey = "DeviceDetails";

        public DeviceDetailsView()
        {
            InitializeComponent();
            DeviceJsonTextBox.Text = string.Empty;
            DataContextChanged += DeviceDetailsView_DataContextChanged;
            _toastService = App.serviceProvider.GetService<IToastService>()!;
            _toastService.ToastRequested += OnToastRequested;
        }

        private void DeviceDetailsView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_vm != null)
                _vm.PropertyChanged -= Vm_PropertyChanged;

            _vm = DataContext as DeviceViewModel;

            if (_vm != null)
                _vm.PropertyChanged += Vm_PropertyChanged;
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DeviceViewModel.List.SelectedDevice))
            {
                _vm!.StopDynamicUpdates();

                DeviceJsonTextBox.Text = string.Empty;
            }
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
