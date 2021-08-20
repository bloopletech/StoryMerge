using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace StoryMerge
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<string> _items = new ObservableCollection<string>();

        public MainWindow()
        {
            InitializeComponent();

            ItemsList.ItemsSource = _items;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Text documents (.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string path in openFileDialog.FileNames)
                {
                    _items.Add(path);
                }
            }
        }

        public void BtnMoveUp_Click(object sender, RoutedEventArgs e)
        {
            var currentIndex = ItemsList.SelectedIndex;
            if (currentIndex == -1) return;

            if (currentIndex > 0)
            {
                int upIndex = currentIndex - 1;
                _items.Move(upIndex, currentIndex);
            }
        }

        public void BtnMoveDown_Click(object sender, RoutedEventArgs e)
        {
            var currentIndex = ItemsList.SelectedIndex;
            if (currentIndex == -1) return;

            if (currentIndex < _items.Count - 1)
            {
                int upIndex = currentIndex + 1;
                _items.Move(upIndex, currentIndex);
            }
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            var currentIndex = ItemsList.SelectedIndex;
            if (currentIndex == -1) return;

            _items.RemoveAt(currentIndex);

            if (currentIndex == _items.Count) currentIndex--;
            ItemsList.SelectedIndex = currentIndex;
        }

        private void BtnMerge_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<string> lines = new();
                foreach (string path in _items)
                {
                    lines.AddRange(File.ReadAllLines(path));
                    lines.Add("");
                }

                if (lines.Any()) lines.RemoveAt(lines.Count - 1);

                for (int index = 0; index < lines.Count; index++)
                {
                    var line = lines[index];
                    if (line.StartsWith("#")) lines[index] = $"#{line}";
                }

                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.FileName = "output";
                saveFileDialog.DefaultExt = ".txt";
                saveFileDialog.Filter = "Text documents (.txt)|*.txt";

                bool? result = saveFileDialog.ShowDialog();

                if (result == true)
                {
                    File.WriteAllLines(saveFileDialog.FileName, lines);
                    MessageBox.Show($"Stories successfully merged into {saveFileDialog.FileName}");
                }
            }
            catch(IOException exception)
            {
                MessageBox.Show($"An error occurred while merging stories: {exception.Message}");
            }
            
        }
    }
}
