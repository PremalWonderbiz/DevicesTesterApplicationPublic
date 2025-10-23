using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeviceTesterCore.Models;
using DeviceTesterServices.Services;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace DeviceTesterTests.ServicesTests
{
    [TestFixture]
    public class ToastServiceTests
    {
        private ToastService _toastService;

        [SetUp]
        public void Setup()
        {
            _toastService = new ToastService();
        }

        [Test]
        public void ShowToast_ShouldRaiseToastRequestedEvent()
        {
            // Arrange
            ToastMessage? receivedMessage = null;
            _toastService.ToastRequested += msg => receivedMessage = msg;

            var toast = new ToastMessage
            {
                Message = "Hello World"
            };

            // Act
            _toastService.ShowToast(toast);

            // Assert
            ClassicAssert.NotNull(receivedMessage, "ToastRequested event was not raised.");
            ClassicAssert.AreEqual(toast.Message, receivedMessage.Message);
        }

        [Test]
        public void ShowToast_WhenNoSubscribers_ShouldNotThrow()
        {
            // Arrange
            var toast = new ToastMessage
            {
                Message = "This should not throw"
            };

            // Act & Assert
            Assert.DoesNotThrow(() => _toastService.ShowToast(toast));
        }
    }
}
