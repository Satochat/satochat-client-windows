using System;
using Satochat.Client.WindowsInternals;

namespace Satochat.Client.Sound {
    public class SoundManager {
        public void PlayAlias(string alias) {
            int flags = (int)(Winmm.PlaySoundFlags.SND_ASYNC | Winmm.PlaySoundFlags.SND_ALIAS);
            Winmm.PlaySound(alias, IntPtr.Zero, flags);
        }
    }
}
