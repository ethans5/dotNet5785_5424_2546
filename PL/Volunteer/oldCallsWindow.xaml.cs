using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using BO;

namespace PL.Volunteer
{
    public partial class OldCallsWindow : Window
    {
        private List<ClosedCallInList> OldCalls;
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private volatile bool _observerWorking = false; // Flag pour éviter les mises à jour multiples

        public OldCallsWindow(int volunteerId)
        {
            InitializeComponent();
            LoadOldCalls(volunteerId);
        }

        private void LoadOldCalls(int volunteerId)
        {
            if (!_observerWorking)
            {
                _observerWorking = true;

                _ = Dispatcher.BeginInvoke(() =>
                {
                    OldCalls = s_bl.Call.ReadAllEndedCalls(volunteerId, null, null)?.ToList();
                    OldCallsListBox.ItemsSource = OldCalls;
                    _observerWorking = false;
                });
            }
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
