using System;
using System.IO;
using Xamarin.Forms;
using XkcdViewer.Droid.Utilities;
using XkcdViewer.Models;

[assembly: Dependency(typeof(SaveAndLoad))]
namespace XkcdViewer.Droid.Utilities
{
    /// <summary>
    /// Android and iOS implementation of SaveAndLoad (text files)
    /// </summary>
    public class SaveAndLoad : ISaveAndLoad
    {
        public void SaveText(string filename, string text)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, filename);
            File.WriteAllText(filePath, text);
        }
        public string LoadText(string filename)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, filename);
            return File.ReadAllText(filePath);
        }
    }
}