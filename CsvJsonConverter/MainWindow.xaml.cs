using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace CsvJsonConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string NO_FILE_CHOSEN = "No file chosen";
        private readonly CsvConfiguration _csvConfiguration = new(CultureInfo.InvariantCulture)
        {
            IgnoreBlankLines = true,
            HasHeaderRecord = true,
            HeaderValidated = null,
            MissingFieldFound = null,
            DetectDelimiter = true,
            ShouldSkipRecord = record => string.IsNullOrEmpty(record.ToString()),
        };

        public MainWindow()
        {
            InitializeComponent();
            SetVersionOnTitle();
        }

        private void SetVersionOnTitle()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            if (version is not null)
                this.Title += $" v{version.Major}.{version.Minor}.{version.Build}";
        }

        private void bttChooseFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                CheckPathExists = true,
                CheckFileExists = true,
                DefaultExt = "csv",
                Filter = "CSV files (*.csv)|*.csv|TXT files (*.txt)|*.txt|All files (*.*)|*.*",
                InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory
            };
            txtChooseFile.Text = openFileDialog.ShowDialog().HasValue && File.Exists(openFileDialog.FileName) ? openFileDialog.FileName : NO_FILE_CHOSEN;
            txtChooseFile.Focus();
            txtChooseFile.CaretIndex = txtChooseFile.Text.Length;
        }

        private void bttConvert_Click(object sender, RoutedEventArgs e)
        {
            txtJSON.Text = ShouldConvertFromFile()
                ? ConvertCsvFromFile(txtChooseFile.Text)
                : ConvertCsvFromText(txtCSV.Text);
        }

        private bool ShouldConvertFromFile()
        {
            return !string.IsNullOrEmpty(txtChooseFile.Text)
                && !txtChooseFile.Text.Equals(NO_FILE_CHOSEN)
                && string.IsNullOrEmpty(txtCSV.Text);
        }
        private string ConvertCsvFromFile(string file)
        {
            using var reader = new StreamReader(file);
            return ParseCsv(reader);
        }

        private string ConvertCsvFromText(string text)
        {
            using var reader = new StringReader(text);
            return ParseCsv(reader);
        }

        private string ParseCsv(TextReader reader)
        {
            using var csvReader = new CsvReader(reader, _csvConfiguration);

            csvReader.Read();
            csvReader.ReadHeader();

            var list = csvReader.GetRecords<dynamic>().ToList();

             var jsonSerializerSettings = new JsonSerializerSettings()
             {
                 Formatting = Formatting.Indented
             };

            if (chkParseNumbers.IsChecked == true)
                jsonSerializerSettings.Converters.Add(new ParseNumbersConverter());

            return JsonConvert.SerializeObject(list, jsonSerializerSettings);
        }

        private void bttClear_Click(object sender, RoutedEventArgs e)
        {
            txtCSV.Clear();
            txtJSON.Clear();
        }

        private void bttSave_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                CheckPathExists = true,
                DefaultExt = "json",
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                InitialDirectory = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, System.IO.Path.GetFileNameWithoutExtension(txtChooseFile.Text))
            };

            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, txtJSON.Text);
        }

        private void bttCopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txtJSON.Text);
        }

        private void cmbSeparator_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            switch (cmbSeparator.SelectedIndex)
            {
                case 0:
                    {
                        _csvConfiguration.DetectDelimiter = true;
                        break;
                    }
                case 1:
                    {
                        _csvConfiguration.DetectDelimiter = false;
                        _csvConfiguration.Delimiter = ",";
                        break;
                    }
                case 2:
                    {
                        _csvConfiguration.DetectDelimiter = false;
                        _csvConfiguration.Delimiter = ";";
                        break;
                    }
                case 3:
                    {
                        _csvConfiguration.DetectDelimiter = false;
                        _csvConfiguration.Delimiter = "\t";
                        break;
                    }
                default:
                    {
                        _csvConfiguration.DetectDelimiter = true;
                        break;
                    }
            }

        }

        private void bttFormatJson_Click(object sender, RoutedEventArgs e)
        {
            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            };

            txtJSON.Text = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(txtJSON.Text), jsonSerializerSettings);
        }

        private void hyperlinkGithub_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = e.Uri.AbsoluteUri, UseShellExecute = true });
            e.Handled = true;
        }
    }
}
