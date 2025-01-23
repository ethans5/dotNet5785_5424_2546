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

        public VolunteerList()
        {
            InitializeComponent();
            LoadVolunteers();
        }

        private void LoadVolunteers()
        {
            // Exemple de données fictives
            _volunteers = new List<VolunteerInList>
            {
                new VolunteerInList { Id = 1, Name = "Jean Dupont", IsActive = true, Totaltreated = 15, TotalSelfCancellation = 2, TotalExpired = 1, callType = callType.BuyingFood },
                new VolunteerInList { Id = 2, Name = "Marie Curie", IsActive = false, Totaltreated = 20, TotalSelfCancellation = 3, TotalExpired = 0, callType = callType.Deliveries },
                new VolunteerInList { Id = 3, Name = "Albert Einstein", IsActive = true, Totaltreated = 30, TotalSelfCancellation = 1, TotalExpired = 2, callType = callType.PackingClothes }
            };

            _filteredVolunteers = _volunteers;
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

        private void VolunteerDataGrid_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (VolunteerDataGrid.SelectedItem is VolunteerInList selectedVolunteer)
            {
                var detailsWindow = new VolunteerDetails(selectedVolunteer.Id);
                detailsWindow.ShowDialog();
                LoadVolunteers(); // Recharger la liste après modifications
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
