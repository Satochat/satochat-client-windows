using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Satochat.Client.Properties;
using Satochat.Client.Util;

namespace Satochat.Client.Emoji {
    public class EmojiManager {
        private readonly string _emojiDir;
        private readonly Dictionary<string, EmojiInfo> _emojiMap = new Dictionary<string, EmojiInfo>();
        private readonly Dictionary<string, string> _emojiPathMap = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _shortcutMap = new Dictionary<string, string>();

        public EmojiManager(string emojiDir) {
            _emojiDir = emojiDir;
            SetShortcutMappingsFromText(Settings.Default.EmojiMap);
            loadEmojiMap();
        }

        public string GetEmojiPath(string code) {
            if (!_emojiPathMap.TryGetValue(code, out var emojiPath)) {
                return null;
            }

            if (!File.Exists(emojiPath)) {
                return null;
            }

            return emojiPath;
        }

        public string GetShortcutMappingsAsText() {
            if (_shortcutMap.Count == 0) {
                return string.Empty;
            }

            var pairList = _shortcutMap.Select(pair => $"{pair.Key}={pair.Value}");
            string text = string.Join(Environment.NewLine, pairList);
            return text;
        }

        public void SetShortcutMappingsFromText(string text) {
            _shortcutMap.Clear();
            using (var reader = new StringReader(text)) {
                string pair;
                while ((pair = reader.ReadLine()) != null) {
                    var match = Regex.Match(pair, "^(.+)=(.+)$");
                    if (!match.Success) {
                        // TODO: error
                        continue;
                    }

                    var key = match.Groups[1];
                    var value = match.Groups[2];

                    _shortcutMap[key.Value] = value.Value;
                }
            }

            var settings = Settings.Default;
            settings.EmojiMap = GetShortcutMappingsAsText();
            settings.Save();
        }

        private string replaceShortcut(KeyValuePair<string, string> mapPair, string input) {
            // Input is HTML-encoded
            string pattern = @"\[emoji:([0-9A-Za-z_\- ()&;]+)\]";
            var m = Regex.Match(input, pattern);

            if (m.Success) {
                return m.Groups[0].Value;
            }

            string key = HtmlUtil.HtmlEncode(mapPair.Key);
            return input.Replace(key, $"[emoji:{mapPair.Value}]");
        }

        public string ReplaceShortcuts(string input) {
            // Input is HTML-encoded
            string splitPattern = @"(\[emoji:[0-9A-Za-z_\- ()&;]+\])";

            foreach (var pair in _shortcutMap) {
                string[] parts = Regex.Split(input, splitPattern);
                parts = parts.Select(s => replaceShortcut(pair, s)).ToArray();
                input = string.Join("", parts);
            }

            return input;
        }

        private void loadEmojiMap() {
            _emojiMap.Clear();
            _emojiPathMap.Clear();

            string indexFilePath = Path.Combine(_emojiDir, "index.json");
            if (!File.Exists(indexFilePath)) {
                // TODO: log error
                return;
            }

            string json = File.ReadAllText(indexFilePath, Encoding.UTF8);
            var list = JsonConvert.DeserializeObject<List<EmojiInfo>>(json);
            if (list == null) {
                // TODO: log error
                return;
            }

            foreach (var emojiInfo in list) {
                string emojiPath = new Uri(Path.Combine(_emojiDir, emojiInfo.File)).LocalPath;
                if (!File.Exists(emojiPath)) {
                    continue;
                }

                if (_emojiMap.ContainsKey(emojiInfo.Code)) {
                    // TODO: log error
                    continue;
                }

                _emojiMap.Add(emojiInfo.Code, emojiInfo);
                _emojiPathMap.Add(emojiInfo.Code, emojiPath);
            }
        }
    }
}
