﻿using System;
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
        private readonly QueueAdmin _queueAdmin;
        private ObservableCollection<QueueDescription> _queues;

        public MainWindow() {
            InitializeComponent();

            Config configForm = new Config();
            configForm.ShowDialog();

            _queueAdmin = new QueueAdmin(
                Settings.Default.AwsAccessKey,
                Settings.Default.AwsSecretKey
            );

            RefreshQueues();
        }

        private void RefreshQueues() {
            _queues = _queueAdmin.ListQueues();
            gridQueues.ItemsSource = _queues;

            foreach (QueueDescription desc in _queues) {
                _queueAdmin.PopulateQueueAttributes(desc);
            }
        }

        private void gridMessages_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) {
            
        }

        private void gridQueues_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) {
            if (gridQueues.SelectedItem is QueueDescription) {
                QueueDescription qd = (QueueDescription)gridQueues.SelectedItem;

                gridMessages.ItemsSource = _queueAdmin.ListTop10Messages(qd.Url);

                buttonDeleteQueue.IsEnabled = true;
                return;
            }

            buttonDeleteQueue.IsEnabled = false;
        }

        private void buttonDeleteQueue_Click(object sender, RoutedEventArgs e) {
            if (MessageBox.Show("Are you sure you want to permanently delete this queue and all messages within it?", "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel) == MessageBoxResult.OK) {
                MessageBox.Show("Deleted queues can take up to 60 seconds to be removed.", "Notice", MessageBoxButton.OK, MessageBoxImage.Information);

                QueueDescription qd = (QueueDescription)gridQueues.SelectedItem;
                _queueAdmin.DeleteQueue(qd.Url);
            }
        }

        private void buttonRefresh_Click(object sender, RoutedEventArgs e) {
            RefreshQueues();
        }
    }
}
