using BO;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace PL.Call
{
    /// <summary>
    /// Logique d'interaction pour CallInListWindow.xaml
    /// </summary>
    public partial class CallInList : Window
    {
        private List<BO.CallInList> _calls;
        private List<BO.CallInList> _filteredCalls;
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public CallInList()
        {
            InitializeComponent();
            CallDataGrid.MouseDoubleClick += CallDataGrid_MouseDoubleClick;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Call.AddObserver(CallListObserver);
            LoadCalls();
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Call.RemoveObserver(CallListObserver);
        }
        private void LoadCalls(CallFields? filter = null, Object? obj = null, CallFields? sort = null)
        {
            try
            {
                _calls = s_bl.Call.ReadAllCalls(filter, obj, sort).ToList();
                _filteredCalls = _calls;
                CallDataGrid.ItemsSource = _filteredCalls;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des appels : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CallListObserver()
        {
            LoadCalls();
        }
        private void AddCall_Click(object sender, RoutedEventArgs e)
        {
            //{
            //    CallDetails window = new CallDetails();
            //    window.ShowDialog();
            //    LoadCalls();
            MessageBox.Show("Fonctionnalité non implémentée", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // Recharger les données
            LoadCalls();
        }
        private void CallDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //if (CallDataGrid.SelectedItem is BO.CallInList call)
            //{
            //    CallDetails window = new CallDetails(call.Id);
            //    window.ShowDialog();
            //}
            MessageBox.Show("Fonctionnalité non implémentée", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }
}