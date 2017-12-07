using System.Windows.Forms;

namespace Satochat.Client {
    public interface ILogWindow {
        void AddLine(string line);
        void ShowWindow(IWin32Window owner, bool show);
        bool IsShowing();
    }
}
