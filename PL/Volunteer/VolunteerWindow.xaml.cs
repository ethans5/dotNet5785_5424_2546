using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PL.Volunteer
{
    public partial class VolunteerWindow : Window
    {
        private BO.Volunteer CurrentVolunteer;
        private int LoggedInId;
        private List<BO.Call> CallsInRange;
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private bool IsEditing = false;

        public VolunteerWindow(BO.Volunteer volunteer, int loggedInId)
        {
            InitializeComponent();

            CurrentVolunteer = volunteer;
            LoggedInId = loggedInId;

            LoadVolunteerInfo();
            LoadAvailableCalls();

            CloseButton.Click += CloseButton_Click;
            EditButton.Click += EditButton_Click;
            AssignCallButton.Click += AssignCallButton_Click;
            ViewOldCallsButton.Click += ViewOldCallsButton_Click; // Ajout du gestionnaire d'événements
        }

        private void LoadVolunteerInfo()
        {
            VolunteerNameTextBox.Text = CurrentVolunteer.Name;
            VolunteerPhoneTextBox.Text = CurrentVolunteer.Phone;
            VolunteerPasswordTextBox.Password = CurrentVolunteer.Password;
            VolunteerAddressTextBox.Text = CurrentVolunteer.Address ?? "Non spécifié";
            VolunteerEmailTextBox.Text = CurrentVolunteer.Mail;

            MaxDistanceTextBox.Text = CurrentVolunteer.MaxDistance?.ToString() ?? "Non défini";
            DistanceTypeComboBox.Text = CurrentVolunteer.DistanceType.ToString();
            IsActiveCheckBox.IsChecked = CurrentVolunteer.IsActive;

            TotalTreatedText.Text = $"✅ Total Traités : {CurrentVolunteer.Totaltreated}";
            TotalCancelledText.Text = $"❌ Total Annulés : {CurrentVolunteer.TotalSelfCancellation}";
            TotalExpiredText.Text = $"⏳ Total Expirés : {CurrentVolunteer.TotalExpired}";

            if (CurrentVolunteer.CallInProgress != null)
            {
                CallInProgressText.Text = $"📞 {CurrentVolunteer.CallInProgress.Description}";
                CallInProgressText.Foreground = System.Windows.Media.Brushes.Green;
            }
            else
            {
                CallInProgressText.Text = "Aucun appel en cours";
                CallInProgressText.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void LoadAvailableCalls()
        {
            CallsInRange = GetAllCalls();
            CallsListBox.ItemsSource = CallsInRange;

            NoCallsMessage.Visibility = CallsInRange.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private List<BO.Call> GetAllCalls()
        {
            return s_bl.Call.ReadAllOpenCalls(CurrentVolunteer.Id, null, null)
                            .Select(call => new BO.Call
                            {
                                Id = call.Id,
                                Description = call.description,
                                Address = call.Address
                            }).ToList();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsEditing)
            {
                SetEditableState(true);
                EditButton.Content = "💾 Enregistrer";
                EditButton.Background = System.Windows.Media.Brushes.SkyBlue;
                IsEditing = true;
            }
            else
            {
                try
                {
                    CurrentVolunteer.Name = VolunteerNameTextBox.Text;
                    CurrentVolunteer.Phone = VolunteerPhoneTextBox.Text;
                    CurrentVolunteer.Address = VolunteerAddressTextBox.Text;
                    CurrentVolunteer.Mail = VolunteerEmailTextBox.Text;
                    CurrentVolunteer.Password = VolunteerPasswordTextBox.Password;
                    CurrentVolunteer.IsActive = IsActiveCheckBox.IsChecked ?? false;
                    CurrentVolunteer.DistanceType = (BO.distanceType)Enum.Parse(typeof(BO.distanceType), DistanceTypeComboBox.Text);
                    CurrentVolunteer.MaxDistance = int.TryParse(MaxDistanceTextBox.Text, out int maxDistance) ? maxDistance : (int?)null;

                    s_bl.Volunteer.UpdateVolunteer(LoggedInId, CurrentVolunteer);
                    MessageBox.Show("Informations mises à jour avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

                    SetEditableState(false);
                    EditButton.Content = "✏️ Modifier";
                    EditButton.Background = System.Windows.Media.Brushes.Gold;
                    IsEditing = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SetEditableState(bool isEditable)
        {
            VolunteerNameTextBox.IsReadOnly = !isEditable;
            VolunteerPhoneTextBox.IsReadOnly = !isEditable;
            VolunteerAddressTextBox.IsReadOnly = !isEditable;
            VolunteerEmailTextBox.IsReadOnly = !isEditable;
            VolunteerPasswordTextBox.IsEnabled = isEditable;
            IsActiveCheckBox.IsEnabled = isEditable;
            DistanceTypeComboBox.IsEnabled = isEditable;
            MaxDistanceTextBox.IsReadOnly = !isEditable;

            var backgroundColor = isEditable ? System.Windows.Media.Brushes.White : System.Windows.Media.Brushes.LightGray;
            VolunteerNameTextBox.Background = backgroundColor;
            VolunteerPhoneTextBox.Background = backgroundColor;
            VolunteerAddressTextBox.Background = backgroundColor;
            VolunteerEmailTextBox.Background = backgroundColor;
        }

        private void AssignCallButton_Click(object sender, RoutedEventArgs e)
        {
            if (CallsListBox.SelectedItem is BO.Call selectedCall)
            {
                try
                {
                    s_bl.Call.ChoiceCall(CurrentVolunteer.Id, selectedCall.Id);
                    MessageBox.Show($"📞 Appel '{selectedCall.Description}' assigné avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

                    LoadVolunteerInfo();
                    LoadAvailableCalls();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un appel à assigner.", "Avertissement", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            LoginPage loginPage = new LoginPage();
            loginPage.Show();
            this.Close();
        }

        private void ViewOldCallsButton_Click(object sender, RoutedEventArgs e)
        {
            OldCallsWindow oldCallsWindow = new OldCallsWindow(CurrentVolunteer.Id);
            oldCallsWindow.Show();
        }
    }
}
