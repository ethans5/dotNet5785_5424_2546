using System;
using System.Windows;
using BO;

namespace PL.Call
{
    public partial class CallDetails : Window
    {
        public BO.Call Call { get; private set; }

        public CallDetails(BO.Call? call = null)
        {
            InitializeComponent();

            Call = call ?? new BO.Call();
            DataContext = Call;

            // Populate combo boxes with enums or predefined values
            CallTypeComboBox.ItemsSource = Enum.GetValues(typeof(callType));
            StatusComboBox.ItemsSource = Enum.GetValues(typeof(Status));
        }
        public CallDetails()
        {
            InitializeComponent ();
            // Populate combo boxes with enums or predefined values
            CallTypeComboBox.ItemsSource = Enum.GetValues(typeof(callType));
            StatusComboBox.ItemsSource = Enum.GetValues(typeof(Status));
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Perform validation if necessary
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}