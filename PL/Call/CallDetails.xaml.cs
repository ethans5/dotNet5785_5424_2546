using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BO;

namespace PL.Call
{
    public partial class CallDetails : Window, INotifyPropertyChanged
    {
        private int? _callId;
        private bool _isCreating;
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private DalApi.IDal _dal = Factory.Get;
        private int LoggedInId;


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public List<CallAssignInList> CallAssignInLists { get; set; }

        public CallDetails(int loggedIn, int? callId = null)
        {
            InitializeComponent();
            _isCreating = !callId.HasValue;
            _callId = callId;
            LoggedInId = loggedIn;

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
            LatitudeTextBox.Text = call.Latitude.ToString();
            LongitudeTextBox.Text = call.Longitude.ToString();
            CreatedDateTextBox.Text = call.Created.ToString("g");
            MaxEndTreatmentTextBox.SelectedDate = call.MaxEndTreatment;  // Correct usage of Calendar
            StatusComboBox.Text = call.Status.ToString();
            CallAssignInLists = call.callAssignInLists ?? new List<CallAssignInList>();
            CallAssignInListsListBox.ItemsSource = CallAssignInLists;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var call = new BO.Call
            {
                Id = _callId ?? 0,
                CallType = (BO.callType)CallTypeComboBox.SelectedItem,
                Description = DescriptionTextBox.Text,
                Address = AddressTextBox.Text,
                Created = s_bl.Admin.GetSystemeClock(),
                MaxEndTreatment = MaxEndTreatmentTextBox.SelectedDate,  // Correct assignment for Calendar
                Status = (BO.Status)StatusComboBox.SelectedItem,
                callAssignInLists = CallAssignInLists,



            };

            if (_isCreating)
            {
                s_bl.Call.CreateCall(call);
                MessageBox.Show("Call created successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if (_isComboBoxModified)
                {

                    s_bl.Call.UpdateCall(call);
                    var selectedStatus = (BO.typeOfEndTreatment)StatusComboBox.SelectedItem;
                    if (selectedStatus == BO.typeOfEndTreatment.treated)
                    {
                        var assign = _dal.Assignment.ReadAll().Where(c => c.CallId == call.Id).FirstOrDefault();
                        s_bl.Call.UpdateCallCancel(LoggedInId, assign.CallId);

                    }
                    else if (selectedStatus == BO.typeOfEndTreatment.selfCancellation || selectedStatus == BO.typeOfEndTreatment.directorCancellation)

                    {
                        var assign = _dal.Assignment.ReadAll().Where(c => c.CallId == call.Id).FirstOrDefault();
                        s_bl.Call.UpdateCallEnd(LoggedInId, assign.CallId);



                    }
                    MessageBox.Show("Call updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }

            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta / 3);
                e.Handled = true;
            }
        }
        private bool _isComboBoxModified = false;
        private object _selectedComboBoxValue = null;

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                _isComboBoxModified = true;
                _selectedComboBoxValue = e.AddedItems[0]; // Stocke la nouvelle valeur sélectionnée
            }
        }


    }
}