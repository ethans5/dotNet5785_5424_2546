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

namespace PL.Volunteer
{
    /// <summary>
    /// Logique d'interaction pour volunteerWindow.xaml
    /// </summary>


    public partial class VolunteerDetails : Window
    {
        private int? _volunteerId;

        public VolunteerDetails(int? volunteerId)
        {
            InitializeComponent();
            _volunteerId = volunteerId;
            LoadVolunteer();
        }

        private void LoadVolunteer()
        {
            if (_volunteerId.HasValue)
            {
                BO.Volunteer volunteer = GetVolunteerById(_volunteerId.Value);
                NameTextBox.Text = volunteer.Name;
                PhoneTextBox.Text = volunteer.Phone;
                MailTextBox.Text = volunteer.Mail;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            var volunteer = new BO.Volunteer
            {
                Id = _volunteerId ?? 0,
                Name = NameTextBox.Text,
                Phone = PhoneTextBox.Text,
                Mail = MailTextBox.Text
            };

            SaveVolunteer(volunteer);
            MessageBox.Show("Les détails ont été sauvegardés avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private BO.Volunteer GetVolunteerById(int id)
        {
            return new BO.Volunteer
            {
                Id = id,
                Name = "Jean Dupont",
                Phone = "0102030405",
                Mail = "jean.dupont@example.com"
            };
        }

        private void SaveVolunteer(BO.Volunteer volunteer)
        {
            // Logique de sauvegarde
        }
    }
}

