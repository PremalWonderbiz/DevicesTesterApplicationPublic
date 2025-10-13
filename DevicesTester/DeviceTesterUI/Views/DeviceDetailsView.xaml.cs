﻿using System;
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
using DeviceTesterCore.Models;
using DeviceTesterUI.ViewModels;
using DeviceTesterUI.Windows;
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

        public DeviceDetailsView()
        {
            InitializeComponent();
            DeviceJsonTextBox.Text = string.Empty;
            DataContextChanged += DeviceDetailsView_DataContextChanged;
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

    }
}
