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
        private ObservableCollection<ProcessItem> pitems = new ObservableCollection<ProcessItem>();
        private JsonModel CurrentJson = new JsonModel();
        private string CurrentJsonFileLocation;

        private ObservableCollection<Hotkey> ObservableHotkeys = new ObservableCollection<Hotkey>();

        public MainWindow()
        {
            InitializeComponent();

            List<ProcessItem> localAll = Process.GetProcesses()
                .Where(p => p.MainWindowHandle != IntPtr.Zero)
                .Select(x => new ProcessItem()
                {
                    Name = x.ProcessName
                })
                .ToList();
            foreach (ProcessItem item in localAll)
            {
                pitems.Add(item);
            }

            Processes.ItemsSource = pitems;

            string appDataBasePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var hotkeyHelperPath = System.IO.Path.Combine(appDataBasePath, "HotkeyHelper");

            CurrentJsonFileLocation = System.IO.Path.Combine(hotkeyHelperPath, "chrome.json");
            CurrentJson = JsonConvert.DeserializeObject<JsonModel>(File.ReadAllText(CurrentJsonFileLocation));
            foreach (var hotkey in CurrentJson.Hotkeys)
            {
                ObservableHotkeys.Add(hotkey);
            }
            //Hotkeys.ItemsSource = CurrentJson.Hotkeys;
            Hotkeys.ItemsSource = ObservableHotkeys;

            if (!Directory.Exists(hotkeyHelperPath))
            {
                Directory.CreateDirectory(hotkeyHelperPath);
            }
            // Or for AppData\Local:

            // when we click on an application, load the json, if it doesnt exist create a json

            this.DataContext = this;

        }

        private void btnRefreshProcesses_Click(object sender, RoutedEventArgs e)
        {
            pitems.Clear();
            List<ProcessItem> localAll = Process.GetProcesses()
                .Where(p => p.MainWindowHandle != IntPtr.Zero)
                .Select(x => new ProcessItem()
                {
                    Name = x.ProcessName
                })
                .ToList();
            foreach (ProcessItem item in localAll)
            {
                pitems.Add(item);
            }
        }

        private void BtnAddNewHotkey_Click(object sender, RoutedEventArgs e)
        {
            ObservableHotkeys.Clear();
            CurrentJson.Hotkeys.Add(new Hotkey()
            {
                Action = HotkeyAction.Text,
                Description = HotkeyDescription.Text
            });
            //CurrentJson.Hotkeys.Add(new Hotkey()
            //{
            //    Action = "testing",
            //    Description = "testing description"
            //});
            File.WriteAllText(CurrentJsonFileLocation, JsonConvert.SerializeObject(CurrentJson));
            //Hotkeys.ItemsSource = CurrentJson.Hotkeys;
            foreach (var hotkey in CurrentJson.Hotkeys)
            {
                ObservableHotkeys.Add(hotkey);
            }
        }
    }

    public class ProcessItem : INotifyPropertyChanged
    {
        private string name;
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

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}