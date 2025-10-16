﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeviceTesterCore.Interfaces;
using Moq;
using NUnit.Framework.Legacy;
using NUnit.Framework;
using DeviceTesterCore.Models;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using Google.Protobuf.WellKnownTypes;
using Moq.Protected;
using DeviceTesterUI.ViewModels;
using System.Reflection;

namespace DeviceTesterUITests.ViewModelTests
{
    [TestFixture]
    public class DeviceViewModelTests
    {
        private DeviceViewModel? _vm;
        private Mock<IDeviceRepository>? _repoMock;
        private Mock<IDeviceDataProvider>? _dataProviderMock;
        private Mock<IToastService>? _toastServiceMock;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IDeviceRepository>();
            _dataProviderMock = new Mock<IDeviceDataProvider>();
            _toastServiceMock = new Mock<IToastService>();

            _repoMock.Setup(r => r.LoadDevicesAsync())
                     .ReturnsAsync(new List<Device>());

            _vm = new DeviceViewModel(_toastServiceMock.Object,_repoMock.Object, _dataProviderMock.Object);
        }

        #region Selection & Editing Tests

        [Test]
        public void SelectedDevice_Set_ShouldCopyDeviceToEditingDevice()
        {
            var device = new Device { DeviceId = "123", Agent = "Redfish" };
            _vm!.List.SelectedDevice = device;

            ClassicAssert.AreNotSame(device, _vm.Form.EditingDevice);
            ClassicAssert.AreEqual(device.DeviceId, _vm.Form.EditingDevice!.DeviceId);
            ClassicAssert.AreEqual(device.Agent, _vm.Form.EditingDevice.Agent);
        }

        [Test]
        public void SelectedDevice_SetToNull_ShouldCreateDefaultEditingDevice()
        {
            _vm!.List.SelectedDevice = null;
            ClassicAssert.IsNotNull(_vm.Form.EditingDevice);
            ClassicAssert.IsEmpty(_vm.Form.EditingDevice!.DeviceId);
        }

        [Test]
        public void EditingDevice_AgentChange_ShouldReloadPorts()
        {
            _vm!.Form.EditingDevice!.Agent = "EcoRT";
            ClassicAssert.Contains("51443", _vm.Form.AvailablePorts);
            ClassicAssert.Contains("51499", _vm.Form.AvailablePorts);
        }

        #endregion

        #region Device List CRUD Tests

        [Test]
        public async Task LoadDevicesAsync_ShouldPopulateDevices()
        {
            var list = new List<Device> { new () { DeviceId = "1" } };
            _repoMock!.Setup(r => r.LoadDevicesAsync()).ReturnsAsync(list);

            await _vm!.LoadDevicesAsync();
            ClassicAssert.AreEqual(1, _vm.List.Devices.Count);
            ClassicAssert.AreEqual("1", _vm.List.Devices[0].DeviceId);
        }

        [Test]
        public void SaveCommand_ShouldInsertNewDevice()
        {
            _vm!.Form.EditingDevice = new Device { IpAddress = "127.0.0.1", Port = "9000", Agent = "Redfish" };

            // Execute SaveCommand which internally calls SaveDeviceAsync
            _vm!.SaveCommand.Execute(null);

            ClassicAssert.AreEqual(1, _vm.List.Devices.Count);
            _repoMock!.Verify(r => r.SaveDevicesAsync(It.IsAny<IEnumerable<Device>>()), Times.AtLeastOnce);
        }

        [Test]
        public void SaveDeviceAsync_UpdateExistingDevice_ShouldReplaceDevice()
        {
            var device = new Device { DeviceId = "1", IpAddress = "127.0.0.1", Port = "9000" };
            _vm!.List.Devices.Add(new Device(device));
            _vm!.Form.EditingDevice = new Device(device)
            {
                IpAddress = "192.168.0.1"   // change IP
            };

            //_vm.Form.EditingDevice.IpAddress = "192.168.0.1"; // change IP
            _vm.SaveCommand.Execute(null);

            ClassicAssert.AreEqual(1, _vm.List.Devices.Count);
            ClassicAssert.AreEqual("192.168.0.1", _vm.List.Devices[0].IpAddress);
            _repoMock!.Verify(r => r.SaveDevicesAsync(It.IsAny<IEnumerable<Device>>()), Times.AtLeastOnce);
        }

        [Test]
        public void SaveDeviceAsync_DuplicateIpPort_ShouldSetErrorMessage()
        {
            var device1 = new Device { DeviceId = "1", IpAddress = "127.0.0.1", Port = "9000" };
            _vm!.List.Devices.Add(device1);

            _vm.Form.EditingDevice = new Device { DeviceId = "2", IpAddress = "127.0.0.1", Port = "9000" };
            _vm.SaveCommand.Execute(null);

            ClassicAssert.AreEqual("A device with the same IP and Port already exists!", _vm.Form.ErrorMessage);
        }

        [Test]
        [Ignore("Tests includes interaction")]
        public void DeleteDeviceAsync_ShouldRemoveDevice()
        {
            var device = new Device { DeviceId = "1" };
            _vm!.List.Devices.Add(device);

            _vm.DeleteCommand.Execute(device);

            _vm.List.Devices.Remove(device); // simulate confirmation

            ClassicAssert.IsFalse(_vm.List.Devices.Contains(device));
            _repoMock!.Verify(r => r.SaveDevicesAsync(It.IsAny<IEnumerable<Device>>()), Times.AtLeastOnce);
        }

        #endregion

        #region Port Management Tests

        [Test]
        public void LoadPorts_ShouldPopulateCorrectPorts()
        {
            _vm!.Form.EditingDevice!.Agent = "SoftdPACManager";
            ClassicAssert.Contains("443", _vm.Form.AvailablePorts);
            ClassicAssert.Contains("Other", _vm.Form.AvailablePorts);
        }

        [Test]
        public void SortAvailablePorts_ShouldSortNumbersAndOtherLast()
        {
            _vm!.Form.AvailablePorts.Clear();
            _vm.Form.AvailablePorts.Add("51499");
            _vm.Form.AvailablePorts.Add("Other");
            _vm.Form.AvailablePorts.Add("443");

            // Use private method via reflection
            var method = typeof(DeviceViewModel).GetMethod("SortAvailablePorts", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method?.Invoke(_vm, null);

            ClassicAssert.AreEqual("443", _vm.Form.AvailablePorts[0]);
            ClassicAssert.AreEqual("51499", _vm.Form.AvailablePorts[1]);
            ClassicAssert.AreEqual("Other", _vm.Form.AvailablePorts[2]);
        }

        #endregion

        #region Dynamic Data Tests

        [Test]
        public async Task GetStaticDataAsync_ShouldCallDataProvider()
        {
            var device = new Device { DeviceId = "1" };
            _vm!.List.SelectedDevice = device;

            _dataProviderMock?.Setup(d => d.GetStaticAsync(device)).ReturnsAsync("StaticData");

            await _vm.GetStaticDataAsync();

            ClassicAssert.AreEqual("StaticData", _vm.Details.DeviceJson);
        }

        [Test]
        public void StartDynamicUpdates_ShouldCallDataProvider()
        {
            var device = new Device { DeviceId = "1" };
            _vm!.List.SelectedDevice = device;

            bool called = false;
            _dataProviderMock?.Setup(d => d.StartDynamicUpdates(device, It.IsAny<Action<string>>()))
                             .Callback<Device, Action<string>>((d, a) => called = true);

            _vm.StartDynamicUpdates(s => { });

            ClassicAssert.IsTrue(called);
        }

        [Test]
        public void StopDynamicUpdates_ShouldCallDataProviderAndClearJson()
        {
            var device = new Device { DeviceId = "1" };
            _vm!.List.SelectedDevice = device;
            _vm!.Details.DeviceJson = "SomeData";

            _vm!.StopDynamicUpdates();

            _dataProviderMock?.Verify(d => d.StopDynamicUpdates(device), Times.Exactly(2));
            ClassicAssert.IsEmpty(_vm.Details.DeviceJson);
        }

        #endregion

        #region PropertyChanged Tests

        [Test]
        public void ChangingDeviceJson_ShouldRaisePropertyChanged()
        {
            bool fired = false;
            _vm!.Details.PropertyChanged += (s, e) => { if (e.PropertyName == "DeviceJson") fired = true; };

            _vm.Details.DeviceJson = "Test";

            ClassicAssert.IsTrue(fired);
        }

        [Test]
        public void ChangingErrorMessage_ShouldRaisePropertyChanged()
        {
            bool fired = false;
            _vm!.Form.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(_vm.Form.ErrorMessage))
                    fired = true;
            };
            _vm.Form.ErrorMessage = "Error";

            ClassicAssert.IsTrue(fired);
        }

        #endregion

    }

}
