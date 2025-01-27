using System;
using System.Windows;
using System.Windows.Controls;

namespace PL.Volunteer
{
    /// <summary>
    /// Logique d'interaction pour VolunteerDetails.xaml
    /// </summary>
    public partial class VolunteerDetails : Window
    {
        private int? _volunteerId;
        private bool _isCreating; // Indique si on est en mode création
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public VolunteerDetails(int? volunteerId = null)
        {
            InitializeComponent();
            _volunteerId = volunteerId;
            _isCreating = !_volunteerId.HasValue; // Mode création si pas d'ID fourni

            // Si en mode création, le champ ID est activé ; sinon, il est désactivé
            IdTextBox.IsEnabled = _isCreating;


            LoadVolunteer();
        }

        private void LoadVolunteer()
        {
            if (_volunteerId.HasValue)
            {
                BO.Volunteer volunteer = GetVolunteerById(_volunteerId.Value);
                IdTextBox.Text = volunteer.Id.ToString();
                IdTextBox.IsEnabled = false; // Grise le champ ID en mode édition
                PasswordBox.Password = volunteer.Password;
                NameTextBox.Text = volunteer.Name;
                PhoneTextBox.Text = volunteer.Phone;
                MailTextBox.Text = volunteer.Mail;
                AddressTextBox.Text = volunteer.Address;
                LongitudeTextBox.Text = volunteer.Longitude.ToString();
                LatitudeTextBox.Text = volunteer.Latitude.ToString();
                IsActiveCheckBox.IsChecked = volunteer.IsActive;
                MaxDistanceTextBox.Text = volunteer.MaxDistance.ToString();
                DistanceTypeComboBox.SelectedIndex = (int)volunteer.DistanceType;
                JobTypeComboBox.SelectedIndex = (int)volunteer.Job;
            }
            else
            {
                // Mode création : nettoyer les champs
                ClearForm();
            }
        }

        private void ClearForm()
        {
            IdTextBox.Text = string.Empty;
            PasswordBox.Password = string.Empty;
            NameTextBox.Text = string.Empty;
            PhoneTextBox.Text = string.Empty;
            MailTextBox.Text = string.Empty;
            AddressTextBox.Text = string.Empty;
            IsActiveCheckBox.IsChecked = false;
            MaxDistanceTextBox.Text = string.Empty;
            DistanceTypeComboBox.SelectedIndex = -1;
            JobTypeComboBox.SelectedIndex = -1;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(IdTextBox.Text) && !_isCreating)
            {
                MessageBox.Show("L'ID est requis en mode création.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var volunteer = new BO.Volunteer
                {
                    Id = int.Parse(IdTextBox.Text),
                    Name = NameTextBox.Text,
                    Password = PasswordBox.Password,
                    Phone = PhoneTextBox.Text,
                    Mail = MailTextBox.Text,
                    Address = AddressTextBox.Text,
                    IsActive = IsActiveCheckBox.IsChecked ?? false,
                    MaxDistance = double.Parse(MaxDistanceTextBox.Text),
                    DistanceType = (BO.distanceType)DistanceTypeComboBox.SelectedIndex,
                    Job = (BO.jobType)JobTypeComboBox.SelectedIndex
                };

                if (_isCreating)
                {
                    s_bl.Volunteer.CreateVolunteer(volunteer);
                    MessageBox.Show("Le volontaire a été créé avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    
                    MessageBox.Show("Les détails ont été sauvegardés avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'enregistrement : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private BO.Volunteer GetVolunteerById(int id)
        {
            var volunteer = s_bl.Volunteer.ReadVolunteer(id);

            return new BO.Volunteer
            {
                Id = volunteer.Id,
                Name = volunteer.Name,
                Password = volunteer.Password,
                Phone = volunteer.Phone,
                Mail = volunteer.Mail,
                Address = volunteer.Address,
                Latitude = volunteer.Latitude,
                Longitude = volunteer.Longitude,
                IsActive = volunteer.IsActive,
                MaxDistance = volunteer.MaxDistance,
                DistanceType = volunteer.DistanceType,
                Job = volunteer.Job
            };
        }

        private void TogglePasswordVisibility_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Visibility == Visibility.Visible)
            {
                PasswordTextBox.Text = PasswordBox.Password;
                PasswordTextBox.Visibility = Visibility.Visible;
                PasswordBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                PasswordBox.Password = PasswordTextBox.Text;
                PasswordBox.Visibility = Visibility.Visible;
                PasswordTextBox.Visibility = Visibility.Collapsed;
            }
        }

    }
}
