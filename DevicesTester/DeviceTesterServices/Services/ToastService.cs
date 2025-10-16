using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeviceTesterCore.Interfaces;
using DeviceTesterCore.Models;

namespace DeviceTesterServices.Services
{
    public class ToastService : IToastService
    {
        public event Action<ToastMessage>? ToastRequested;

        public void ShowToast(ToastMessage toast)
        {
            // Core layer only raises the event
            ToastRequested?.Invoke(toast);
        }
    }
}
