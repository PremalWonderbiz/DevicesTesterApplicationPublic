using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceTesterCore.Models
{
    public enum ToastLevel
    {
        Info,
        Success,
        Warning,
        Error
    }

    public class ToastMessage
    {
        public string Message { get; set; } = string.Empty;
        public ToastLevel Level { get; set; } = ToastLevel.Info; // Core-safe
        public string? ViewKey { get; set; } // Target view/context, null = global
    }

}
