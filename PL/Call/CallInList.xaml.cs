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
        private List<BO.CallInList> _calls;
        private List<BO.CallInList> _filteredCalls;
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        private CallFields? _selectedFilter;
        private object _selectedFilterValue;

        // Implémentation de INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public CallFields? SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                if (_selectedFilter != value)
                {
                    _selectedFilter = value;
                    OnPropertyChanged();
                }
            }
        }

        public object SelectedFilterValue
        {
            get => _selectedFilterValue;
            set
            {
                if (_selectedFilterValue != value)
                {
                    _selectedFilterValue = value;
                    OnPropertyChanged();
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public CallInList()
        {
            InitializeComponent();
            DataContext = this;
            CallDataGrid.MouseDoubleClick += CallDataGrid_MouseDoubleClick;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Inscrire l'observateur et charger les données
            s_bl.Call.AddObserver(CallListObserver);
            LoadCalls();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // Supprimer l'observateur
            s_bl.Call.RemoveObserver(CallListObserver);
        }

        private void LoadCalls(CallFields? filter = null, object filterValue = null, CallFields? sort = null)
        {
            try
            {
                // Charger les appels depuis la couche BL avec les filtres actifs
                _calls = s_bl.Call.ReadAllCalls(filter, filterValue, sort).ToList();

                // Appliquer les filtres
                _filteredCalls = _calls;

                // Mettre à jour la source de données du DataGrid
                CallDataGrid.ItemsSource = _filteredCalls;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des appels : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CallListObserver()
        {
            // Mettre à jour la liste des appels
            LoadCalls(sort: SelectedFilter);
        }

        private object ConvertFilterValue(CallFields filterField, string filterText)
        {
            if (string.IsNullOrEmpty(filterText)) return null;

            try
            {
                return filterField switch
                {
                    CallFields.Id or CallFields.CallId or CallFields.totalAssignmentAllocation
                        => int.Parse(filterText),
                    CallFields.startingTime
                        => DateTime.Parse(filterText),
                    CallFields.remainingTime or CallFields.duration
                        => TimeSpan.Parse(filterText),
                    CallFields.callType or CallFields.Status
                        => Enum.Parse(typeof(CallFields), filterText),
                    _ => filterText
                };
            }
            catch (Exception)
            {
                MessageBox.Show($"Format invalide pour le filtre {filterField}", "Erreur de format", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
        }

        private void ApplyFilterAndSort_Click(object sender, RoutedEventArgs e)
        {
            CallFields? filterField = null;
            object filterValue = null;
            CallFields? sortField = null;

            // Récupération du champ de filtrage
            if (FilterComboBox.SelectedItem is ComboBoxItem filterItem &&
                filterItem.Tag != null)
            {
                filterField = (CallFields)Enum.Parse(typeof(CallFields), filterItem.Tag.ToString());
                filterValue = ConvertFilterValue(filterField.Value, FilterTextBox.Text);
            }

            // Récupération du champ de tri
            if (SortComboBox.SelectedItem is ComboBoxItem sortItem &&
                sortItem.Tag != null)
            {
                sortField = (CallFields)Enum.Parse(typeof(CallFields), sortItem.Tag.ToString());
            }

            LoadCalls(filterField, filterValue, sortField);
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            // Réinitialiser les filtres
            SelectedFilter = null;
            SelectedFilterValue = null;

            // Réinitialiser les contrôles
            FilterComboBox.SelectedIndex = -1;
            FilterTextBox.Text = string.Empty;
            SortComboBox.SelectedIndex = -1;

            // Recharger les appels sans filtre
            LoadCalls();
        }

        private void AddCall_Click(object sender, RoutedEventArgs e)
        {
            // Ouvrir la fenêtre pour ajouter un appel
            var detailsWindow = new CallDetails();
            detailsWindow.ShowDialog();

            // Recharger la liste après ajout
            LoadCalls();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // Recharger les données
            LoadCalls();
        }

        private void CallDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CallDataGrid.SelectedItem is BO.CallInList selectedCall)
            {
                // Ouvrir la fenêtre avec les détails de l'appel
                var details = new CallDetails(s_bl.Call.ReadCall(selectedCall.callId));
                details.ShowDialog();

                // Recharger la liste après modification
                LoadCalls();
            }
        }
    }
}