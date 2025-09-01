using HotkeyHelper.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HotkeyHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<JsonFile> JsonFiles = new ObservableCollection<JsonFile>();
        private JsonModel CurrentJson = new JsonModel();
        private string CurrentJsonFileLocation;

        private ObservableCollection<Hotkey> ObservableHotkeys = new ObservableCollection<Hotkey>();

        public string BasePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HotkeyHelper");

        public MainWindow()
        {
            InitializeComponent();

            if (!Directory.Exists(BasePath))
            {
                Directory.CreateDirectory(BasePath);
            }

            RefreshJsonsShown();
            Processes.ItemsSource = JsonFiles;

            string appDataBasePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var hotkeyHelperPath = System.IO.Path.Combine(appDataBasePath, "HotkeyHelper");

            CurrentJsonFileLocation = System.IO.Path.Combine(hotkeyHelperPath, "chrome.json");

            RefreshHotkeysShown(CurrentJsonFileLocation);
            Hotkeys.ItemsSource = ObservableHotkeys;

            this.DataContext = this;
        }

        private void BtnAddNewHotkey_Click(object sender, RoutedEventArgs e)
        {
            ObservableHotkeys.Clear();
            CurrentJson.Hotkeys.Add(new Hotkey()
            {
                Action = HotkeyAction.Text,
                Description = HotkeyDescription.Text
            });

            File.WriteAllText(CurrentJsonFileLocation, JsonConvert.SerializeObject(CurrentJson));
            foreach (var hotkey in CurrentJson.Hotkeys)
            {
                ObservableHotkeys.Add(hotkey);
            }
        }

        private void RefreshHotkeysShown(string filePath)
        {
            ObservableHotkeys.Clear();
            CurrentJson = JsonConvert.DeserializeObject<JsonModel>(File.ReadAllText(filePath));
            foreach (var hotkey in CurrentJson.Hotkeys)
            {
                ObservableHotkeys.Add(hotkey);
            }
        }

        /// <summary>
        /// Refresh the list of jsons that a user can click on
        /// </summary>
        private void RefreshJsonsShown()
        {
            JsonFiles.Clear();
            var jsons = Directory.GetFiles(BasePath).Where(x => System.IO.Path.GetExtension(x) == ".json");
            foreach (var json in jsons)
            {
                JsonFiles.Add(new()
                {
                    Name = System.IO.Path.GetFileName(json),
                    FilePath = System.IO.Path.GetFullPath(json),
                });
            }
        }

        private void ClickMe_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;
            HotkeyHelper.JsonFile current = (JsonFile)clickedButton.DataContext;
            CurrentJsonFileLocation = current.FilePath;
            RefreshHotkeysShown(current.FilePath);
        }

        private void BtnAddNewJson_Click(object sender, RoutedEventArgs e)
        {
            var text = NewJson.Text;

            // TODO: add validation when creating a new file 
            // does the file name already exist?
            // sanitization of the name

            var newPath = System.IO.Path.Combine(BasePath, $"{text}.json");
            var newJson = JsonConvert.SerializeObject(new JsonModel());
            byte[] jsonBytes = Encoding.UTF8.GetBytes(newJson);

            using (var file = File.Create(newPath))
            {
                file.Write(jsonBytes, 0, jsonBytes.Length);
            };

            CurrentJsonFileLocation = newPath;
            RefreshHotkeysShown(newPath);
            NewJson.Text = "";
            RefreshJsonsShown();
        }
    }

    public class JsonFile : INotifyPropertyChanged
    {
        private string name;
        private string filePath;
        public string Name
        {
            get { return this.name; }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.NotifyPropertyChanged("Name");
                }
            }
        }

        public string FilePath
        {
            get { return this.filePath; }
            set
            {
                if (this.filePath != value)
                {
                    this.filePath = value;
                    this.NotifyPropertyChanged("FilePath");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}