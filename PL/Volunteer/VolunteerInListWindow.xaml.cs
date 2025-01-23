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
        private BO.Volunteer _volunteer;

        public VolunteerList()
        {
            InitializeComponent();
            VolunteerDataGrid.MouseDoubleClick += VolunteerDataGrid_MouseDoubleClick;
            LoadVolunteers();
        }

        private void LoadVolunteers(bool? isActive = null, VolunteerSortField? sortBy = null)
        {
            // Appel de la méthode ReadAllVolunteers pour charger les données
            var _volunteers = s_bl.Volunteer.ReadAllVolunteers(isActive, sortBy);
            var _filteredVolunteers = _volunteers;
            VolunteerDataGrid.ItemsSource = _filteredVolunteers;
        }


        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var query = SearchTextBox.Text.ToLower();
            _filteredVolunteers = _volunteers
                .Where(v => v.Name.ToLower().Contains(query))
                .ToList();
            VolunteerDataGrid.ItemsSource = _filteredVolunteers;
        }

        private void VolunteerDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (VolunteerDataGrid.SelectedItem is BO.VolunteerInList selectedVolunteer)
            {
                // Ouvre VolunteerDetails avec l'ID du volontaire sélectionné
                var detailsWindow = new VolunteerDetails(selectedVolunteer.Id);
                detailsWindow.ShowDialog();

                // Recharge la liste après modification
                LoadVolunteers();
            }
        }

     


        private void AddVolunteer_Click(object sender, RoutedEventArgs e)
        {

            var detailsWindow = new VolunteerDetails(null); // Ajouter un nouveau volontaire
            detailsWindow.ShowDialog();
            LoadVolunteers(); // Recharger la liste après ajout
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadVolunteers(); // Rafraîchir les données
        }

       
    }
}
