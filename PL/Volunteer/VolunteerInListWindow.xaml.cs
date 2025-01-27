using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BO;

namespace PL.Volunteer
{
    public partial class VolunteerList : Window
    {
        private List<VolunteerInList> _volunteers;
        private List<VolunteerInList> _filteredVolunteers;
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        // Propriété pour le critère de tri sélectionné
        public VolunteerSortField? SelectedFilter { get; set; }

        public VolunteerList()
        {
            InitializeComponent();
            VolunteerDataGrid.MouseDoubleClick += VolunteerDataGrid_MouseDoubleClick;
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

        private void LoadVolunteers(bool? isActive = null, VolunteerSortField? sortBy = null)
        {
            try
            {
                // Charger les volontaires depuis la couche BL
                _volunteers = s_bl.Volunteer.ReadAllVolunteers(isActive, sortBy).ToList();

                // Appliquer les filtres (le cas échéant)
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
            // Appliquer le filtre sélectionné
            LoadVolunteers(sortBy: SelectedFilter);
        }

        private void AddVolunteer_Click(object sender, RoutedEventArgs e)
        {
            // Ouvrir la fenêtre pour ajouter un volontaire
            var detailsWindow = new VolunteerDetails(null);
            detailsWindow.ShowDialog();

            // Recharger la liste après ajout
            LoadVolunteers();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // Recharger les données
            LoadVolunteers();
        }

        private void VolunteerDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (VolunteerDataGrid.SelectedItem is VolunteerInList selectedVolunteer)
            {
                // Ouvrir la fenêtre de détails pour modifier le volontaire
                var detailsWindow = new VolunteerDetails(selectedVolunteer.Id);
                detailsWindow.ShowDialog();

                // Recharger la liste après modification
                LoadVolunteers();
            }
        }
    }
}
