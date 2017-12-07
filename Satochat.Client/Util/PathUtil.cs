using System;
using System.IO;
using System.Reflection;

namespace Satochat.Client.Util {
    public static class PathUtil {
        public static string GetPathRelativeToApp(string path) {
            var uri = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase);
            string dir = Path.GetDirectoryName(uri.LocalPath);
            return Path.Combine(dir, path);
        }
    }
}
