using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClientWpfAsyncSocket
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private const string _fileName = "setting.txt";
        public SettingsWindow()
        {
            InitializeComponent();
            using (FileStream fs = new FileStream(_fileName, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    txtIP.Text = sr.ReadLine();
                    txtPort.Text = sr.ReadLine();
                }
            }
        }

        private void Port_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtPort.Text))
            {
                checkPort.Foreground = new SolidColorBrush(Colors.Green);
                checkPort.Text = "+";
                if (checkIP.Text == "+") SaveSettings.IsEnabled = true;
            }
            else
            {
                checkPort.Foreground = new SolidColorBrush(Colors.Red);
                checkPort.Text = "-";
            }
        }

        private void IP_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtIP.Text))
            {
                checkIP.Foreground = new SolidColorBrush(Colors.Green);
                checkIP.Text = "+";
                if (checkPort.Text == "+") SaveSettings.IsEnabled = true;
            }
            else
            {
                checkIP.Foreground = new SolidColorBrush(Colors.Red);
                checkIP.Text = "-";
            }
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var tryIP = IPAddress.Parse(txtIP.Text);
                using (FileStream fs = new FileStream(_fileName, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(txtIP.Text);
                        sw.WriteLine(txtPort.Text);
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Проблема збереження налаштувань");
            }
        }
    }
}
