using System.Windows.Forms;
using Satochat.Client.WindowsInternals;

namespace Satochat.Client {
    public static class FormExtensions {
        public static void Flash(this Form form) {
            User32.Flash(form.Handle);
        }

        public static bool IsActive(this Form form) {
            return User32.GetForegroundWindow() == form.Handle;
        }
    }
}
