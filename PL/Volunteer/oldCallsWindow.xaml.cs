using System.Collections.Generic;
using System.Windows;
using BO;

namespace PL.Volunteer
{
    public partial class OldCallsWindow : Window
    {
        private List<ClosedCallInList> OldCalls;
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();


        public OldCallsWindow(int volunteerId)
        {
            InitializeComponent();
            LoadOldCalls(volunteerId);
        }

        private void LoadOldCalls(int volunteerId)
        {
            OldCalls = s_bl.Call.ReadAllEndedCalls(volunteerId, null, null ).ToList();  // Vous devrez ajuster cette méthode en fonction de votre implémentation
            OldCallsListBox.ItemsSource = OldCalls;
        }
    }
}
