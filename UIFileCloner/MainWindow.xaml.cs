using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UIFileCloner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string filePath;
        public MainWindow()
        {
            InitializeComponent();
            filePath = "C:\\Users\\sarat\\source\\repos\\FileCloner\\UIFileCloner\\config.json"; //need to add relative path
            LoadJsonData();
        }
        private void LoadJsonData()
        {
            if (File.Exists(filePath))
            {
                string jsonInString = File.ReadAllText(filePath);
                if (jsonInString != null)
                {
                    ModalClass data = JsonSerializer.Deserialize<ModalClass>(jsonInString);
                    TitleTextBox.Text = data.Title;
                    DescriptionTextBox.Text = data.Description;
                }
                else
                {
                    MessageBox.Show("Json file is empty!");
                }
            }
            else
            {
                MessageBox.Show(filePath);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ModalClass newData = new ModalClass
            {
                Title = TitleTextBox.Text,
                Description = DescriptionTextBox.Text
            };

            //include try catch here
            string jsonString = JsonSerializer.Serialize(newData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonString);
            MessageBox.Show("Changes saved!");
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            TitleTextBox.Text = "";
            DescriptionTextBox.Text = "";
            ModalClass newData = new ModalClass
            {
                Title = "",
                Description = ""
            };
            string emptyJsonString = JsonSerializer.Serialize(newData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, emptyJsonString);
            MessageBox.Show("Succesfully reset the config file!");
        }
    }
}