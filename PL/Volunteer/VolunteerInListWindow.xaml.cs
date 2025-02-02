using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BO;

namespace PL.Volunteer
{
    public partial class VolunteerList : Window, INotifyPropertyChanged
    {
        private List<VolunteerInList> _volunteers;
        private List<VolunteerInList> _filteredVolunteers;
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private int LoggedInId;
        private VolunteerSortField? _selectedFilter;

        // Implémentation de INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        public VolunteerSortField? SelectedFilter
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

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public VolunteerList(int loggedInId)
        {
            InitializeComponent();
            DataContext = this; // Définir le DataContext pour les liaisons
            VolunteerDataGrid.MouseDoubleClick += VolunteerDataGrid_MouseDoubleClick;
            LoggedInId = loggedInId;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Inscrire l'observateur et charger les données
            s_bl.Volunteer.AddObserver(VolunteerListObserver);
            LoadVolunteers();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // Supprimer l'observateur
            s_bl.Volunteer.RemoveObserver(VolunteerListObserver);
        }

        //private void LoadVolunteers(bool? isActive = null, VolunteerSortField? sortBy = null)
        //{
        //    try
        //    {
        //        // Charger les volontaires depuis la couche BL
        //        _volunteers = s_bl.Volunteer.ReadAllVolunteers(isActive, sortBy).ToList();

        //        // Appliquer les filtres (le cas échéant)
        //        _filteredVolunteers = _volunteers;

        //        // Mettre à jour la source de données du DataGrid
        //        VolunteerDataGrid.ItemsSource = _filteredVolunteers;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Erreur lors du chargement des volontaires : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        private void LoadVolunteers(bool? isActive = null, VolunteerSortField? sortBy = null)
        {
            try
            {
                // Charger les volontaires depuis la couche BL avec les filtres actifs
                _volunteers = s_bl.Volunteer.ReadAllVolunteers(isActive, sortBy).ToList();

                // Appliquer les filtres
                _filteredVolunteers = _volunteers;

                // Mettre à jour la source de données du DataGrid
                VolunteerDataGrid.ItemsSource = _filteredVolunteers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des volontaires : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void VolunteerListObserver()
        {
            // Mettre à jour la liste des volontaires
            LoadVolunteers(sortBy: SelectedFilter);
        }
        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            // Appliquer le filtre combiné
            LoadVolunteers(isActive: SelectedStatus, sortBy: SelectedFilter);
        }



        private void ResetFilterButton_Click(object sender, RoutedEventArgs e)
        {
            // Réinitialiser les filtres
            SelectedFilter = null;
            SelectedStatus = null;

            // Réinitialiser la sélection des ComboBox
            StatusComboBox.SelectedIndex = 0;

            // Recharger les volontaires sans filtre
            LoadVolunteers();
        }


        private void AddVolunteer_Click(object sender, RoutedEventArgs e)
        {
            // Ouvrir la fenêtre pour ajouter un volontaire
            var detailsWindow = new VolunteerDetails(LoggedInId);
            detailsWindow.ShowDialog();

            // Recharger la liste après ajout
            LoadVolunteers();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // Recharger les données
            LoadVolunteers();
        }

        //private void VolunteerDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    if (VolunteerDataGrid.SelectedItem is VolunteerInList selectedVolunteer)
        //    {
        //        // Ouvrir la fenêtre de détails pour modifier le volontaire
        //        var detailsWindow = new VolunteerDetails(selectedVolunteer.Id);
        //        detailsWindow.ShowDialog();

        //        // Recharger la liste après modification
        //        LoadVolunteers();
        //    }
        //}

        private void VolunteerDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (VolunteerDataGrid.SelectedItem is VolunteerInList selectedVolunteer)
            {
                // Ouvrir la fenêtre avec l'ID du volontaire
                var detailsWindow = new VolunteerDetails(LoggedInId, selectedVolunteer.Id);
                detailsWindow.ShowDialog();

                // Recharger la liste après modification
                LoadVolunteers();
            }
        }


        private bool? _selectedStatus;


        public bool? SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                if (_selectedStatus != value)
                {
                    _selectedStatus = value;
                    OnPropertyChanged();
                }
            }
        }


        private void StatusFilterButton_Click(object sender, RoutedEventArgs e)
        {
            // Vérifier la sélection dans le ComboBox
            if (StatusComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                switch (selectedItem.Content.ToString())
                {
                    case "Actif":
                        SelectedStatus = true;
                        break;
                    case "Inactif":
                        SelectedStatus = false;
                        break;
                    default:
                        SelectedStatus = null;
                        break;
                }
            }

            // Charger les données avec le statut sélectionné
            LoadVolunteers(isActive: SelectedStatus);
        }

        private void DeleteVolunteerButton_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer l'ID du volontaire à supprimer
            if (sender is Button button && button.CommandParameter is int volunteerId)
            {
                // Demander confirmation à l'utilisateur
                var result = MessageBox.Show($"Êtes-vous sûr de vouloir supprimer le volontaire avec l'ID {volunteerId} ?",
                                             "Confirmation de suppression",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Supprimer le volontaire via la couche BL
                        s_bl.Volunteer.DeleteVolunteer(volunteerId);

                        // Recharger la liste des volontaires
                        LoadVolunteers(isActive: SelectedStatus, sortBy: SelectedFilter);

                        MessageBox.Show("Volontaire supprimé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur lors de la suppression : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

    }
}
