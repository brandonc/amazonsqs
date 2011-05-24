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
using System.Windows.Navigation;
using System.Windows.Shapes;
using AmazonSqs.Status.Components;
using AmazonSqs.Status.Properties;
using System.Collections.ObjectModel;
using System.Threading;

namespace AmazonSqs.Status {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private QueueAdmin queueAdmin;
        private ObservableCollection<QueueDescription> queues;

        public MainWindow() {
            InitializeComponent();

            Config configForm = new Config();
            configForm.ShowDialog();

            this.queueAdmin = new QueueAdmin(
                Settings.Default.AwsAccessKey,
                Settings.Default.AwsSecretKey
            );

            RefreshQueues();
        }

        private void RefreshQueues() {
            this.queues = this.queueAdmin.ListQueues();
            this.gridQueues.ItemsSource = this.queues;

            foreach (QueueDescription desc in queues) {
                this.queueAdmin.PopulateQueueAttributes(desc);
            }
        }

        private void gridMessages_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) {
            
        }

        private void gridQueues_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) {
            if (this.gridQueues.SelectedItem is QueueDescription) {
                QueueDescription qd = (QueueDescription)this.gridQueues.SelectedItem;

                this.gridMessages.ItemsSource = this.queueAdmin.ListTop10Messages(qd.Url);

                this.buttonDeleteQueue.IsEnabled = true;
                return;
            }

            this.buttonDeleteQueue.IsEnabled = false;
        }

        private void buttonDeleteQueue_Click(object sender, RoutedEventArgs e) {
            if (MessageBox.Show("Are you sure you want to permanently delete this queue and all messages within it?", "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel) == MessageBoxResult.OK) {
                MessageBox.Show("Deleted queues can take up to 60 seconds to be removed.", "Notice", MessageBoxButton.OK, MessageBoxImage.Information);

                QueueDescription qd = (QueueDescription)this.gridQueues.SelectedItem;
                this.queueAdmin.DeleteQueue(qd.Url);
            }
        }

        private void buttonRefresh_Click(object sender, RoutedEventArgs e) {
            RefreshQueues();
        }
    }
}
