using BO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PL.Call
{
    public partial class CallInList : Window, INotifyPropertyChanged
    {
        private readonly BlApi.IBl _bl = BlApi.Factory.Get();
        private List<BO.CallInList> _calls;
        private List<BO.CallInList> _filteredCalls;

        public event PropertyChangedEventHandler PropertyChanged;

        private CallFields? _selectedFilter;
        public CallFields? SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                _selectedFilter = value;
                OnPropertyChanged();
            }
        }

        private object _selectedFilterValue;
        public object SelectedFilterValue
        {
            get => _selectedFilterValue;
            set
            {
                _selectedFilterValue = value;
                OnPropertyChanged();
            }
        }

        public CallInList()
        {
            InitializeComponent();
            DataContext = this;
            CallDataGrid.MouseDoubleClick += CallDataGrid_MouseDoubleClick;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _bl.Call.AddObserver(RefreshCalls);
            
            LoadCalls();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            _bl.Call.RemoveObserver(RefreshCalls);
        }

        private void LoadCalls(CallFields? filter = null, object? filterValue = null, CallFields? sort = null)
        {
            try
            {
                _calls = _bl.Call.ReadAllCalls(filter, filterValue, sort).ToList();
                _filteredCalls = _calls;
                CallDataGrid.ItemsSource = _filteredCalls;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des appels : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshCalls()
        {
            LoadCalls(SelectedFilter, SelectedFilterValue);
        }

        private void ApplyFilterAndSort_Click(object sender, RoutedEventArgs e)
        {
            SelectedFilter = GetSelectedFilter(FilterComboBox); 
            SelectedFilterValue = ConvertFilterValue(SelectedFilter, FilterTextBox.Text);
            var sortField = GetSelectedFilter(SortComboBox); 

            LoadCalls(SelectedFilter, SelectedFilterValue, sortField);
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            SelectedFilter = null;
            SelectedFilterValue = null;
            FilterComboBox.SelectedIndex = -1;
            FilterTextBox.Text = string.Empty;
            SortComboBox.SelectedIndex = -1;
            LoadCalls();
        }

        private void AddCallButton_Click(object sender, RoutedEventArgs e)
        {
            new CallDetails().ShowDialog();
            LoadCalls();
        }

        private void RefreshCallList_Click(object sender, RoutedEventArgs e)
        {
            LoadCalls();
        }

        private void CallDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CallDataGrid.SelectedItem is BO.CallInList selectedCall)
            {
                new CallDetails(selectedCall.callId).ShowDialog();
                LoadCalls();
            }
        }

        private CallFields? GetSelectedFilter(ComboBox comboBox)
        {
            return comboBox.SelectedItem is ComboBoxItem item && item.Tag != null
                ? Enum.Parse<CallFields>(item.Tag.ToString()!)
                : (CallFields?)null;
        }

        private object ConvertFilterValue(CallFields? filterField, string filterText)
        {
            if (filterField == null || string.IsNullOrEmpty(filterText)) return null;
            try
            {
                return filterField.Value switch
                {
                    CallFields.Id or CallFields.CallId or CallFields.totalAssignmentAllocation => int.Parse(filterText),
                    CallFields.startingTime => DateTime.Parse(filterText),
                    CallFields.remainingTime or CallFields.duration => TimeSpan.Parse(filterText),
                    CallFields.callType or CallFields.Status => Enum.Parse(typeof(CallFields), filterText),
                    _ => filterText
                };
            }
            catch (Exception)
            {
                MessageBox.Show($"Format invalide pour le filtre {filterField}", "Erreur de format", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void DeleteCallButton_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer l'ID du volontaire à supprimer
            if (sender is Button button && button.CommandParameter is int id)
            {
                // Demander confirmation à l'utilisateur
                var result = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer le call avec l'ID {id} ?",
                                             "Confirmation de suppression",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Supprimer le volontaire via la couche BL
                        _bl.Call.DeleteCall(id);

                        // Recharger la liste des volontaires
                        LoadCalls();

                        MessageBox.Show("Volontaire supprimé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur lors de la suppression : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer != null)
            {
                // Permet le scrolling fluide avec le touchpad
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta / 3);
                e.Handled = true; // Empêche la propagation de l'événement
            }
        }

    }
}
