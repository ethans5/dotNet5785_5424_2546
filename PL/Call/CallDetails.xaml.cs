using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using BO;

namespace PL.Call
{
    public partial class CallDetails : Window, INotifyPropertyChanged
    {
        private int? _callId;
        private bool _isCreating;
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public List<CallAssignInList> CallAssignInLists { get; set; }

        public CallDetails(int? callId = null)
        {
            InitializeComponent();
            _isCreating = !callId.HasValue;
            _callId = callId;

            if (callId.HasValue)
            {
                LoadCallDetails(callId.Value);
            }
            SaveButton.Content = _isCreating ? "Add" : "Update";
        }

        private void LoadCallDetails(int callId)
        {
            var call = s_bl.Call.ReadCall(callId);
            IdTextBox.Text = call.Id.ToString();
            CallTypeComboBox.SelectedItem = call.CallType;
            DescriptionTextBox.Text = call.Description;
            AddressTextBox.Text = call.Address;
            CreatedDateTextBox.Text = call.Created.ToString("g");

            CallAssignInLists = call.callAssignInLists ?? new List<CallAssignInList>();
            CallAssignInListsListBox.ItemsSource = CallAssignInLists;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var call = new BO.Call
            {
                CallType = (BO.callType)CallTypeComboBox.SelectedItem,
                Description = DescriptionTextBox.Text,
                Address = AddressTextBox.Text,
                Created = s_bl.Admin.GetSystemeClock(),
                callAssignInLists = CallAssignInLists
            };

            if (_isCreating)
            {
                s_bl.Call.CreateCall(call);
                MessageBox.Show("Call created successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                s_bl.Call.UpdateCall(call);
                MessageBox.Show("Call updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
