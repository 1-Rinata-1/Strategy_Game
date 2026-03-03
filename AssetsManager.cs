using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace StrategyGame
{
    // паттерн одиночка
    public sealed class AssetsManager // sealed запрещает наследованиие
    {
        // экземпляр создаётся при первом обращении
        private static readonly Lazy<AssetsManager> _instance = new Lazy<AssetsManager>(() => new AssetsManager());
        public static AssetsManager Instance => _instance.Value;

        private readonly Dictionary<string, Image> _cache = new Dictionary<string, Image>(StringComparer.OrdinalIgnoreCase);
        private readonly string _assetsDirectory;

        private AssetsManager()
        {
            _assetsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets");
        }

        public Image GetImage(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return null;
            if (_cache.TryGetValue(fileName, out var img)) return img;
            try
            {
                var path = Path.Combine(_assetsDirectory, fileName);
                if (!File.Exists(path)) return null;
                img = Image.FromFile(path);
                _cache[fileName] = img;
                return img;
            }
            catch
            {
                return null;
            }
        }
    }
}

