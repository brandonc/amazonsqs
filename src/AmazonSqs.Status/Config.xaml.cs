using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AmazonSqs.Status {
    /// <summary>
    /// Interaction logic for Config.xaml
    /// </summary>
    public partial class Config : Window {
        public Config() {
            InitializeComponent();

            textAccessKey.Text = Properties.Settings.Default.AwsAccessKey;
            textSecretKey.Text = Properties.Settings.Default.AwsSecretKey;
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.AwsAccessKey = textAccessKey.Text;
            Properties.Settings.Default.AwsSecretKey = textSecretKey.Text;

            Properties.Settings.Default.Save();

            Close();
        }
    }
}
