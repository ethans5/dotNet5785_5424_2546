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
        }

        /// <summary>
        /// Charge les informations du volontaire dans les champs correspondants.
        /// </summary>
        private void LoadVolunteerInfo()
        {
            VolunteerNameTextBox.Text = CurrentVolunteer.Name;
            VolunteerPhoneTextBox.Text = CurrentVolunteer.Phone;
            VolunteerAddressTextBox.Text = CurrentVolunteer.Address ?? "Non spécifié";
            VolunteerEmailTextBox.Text = CurrentVolunteer.Mail;

            MaxDistanceTextBox.Text = CurrentVolunteer.MaxDistance?.ToString() ?? "Non défini";
            DistanceTypeTextBox.Text = CurrentVolunteer.DistanceType.ToString();
            IsActiveTextBox.Text = CurrentVolunteer.IsActive ? "✅ Actif" : "❌ Inactif";

            // 🎯 Mise à jour des statistiques
            TotalTreatedText.Text = $"✅ Total Traités : {CurrentVolunteer.Totaltreated}";
            TotalCancelledText.Text = $"❌ Total Annulés : {CurrentVolunteer.TotalSelfCancellation}";
            TotalExpiredText.Text = $"⏳ Total Expirés : {CurrentVolunteer.TotalExpired}";

            // 🟢 Vérification de l'appel en cours
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


        /// <summary>
        /// Charge les appels disponibles et les affiche dans la liste.
        /// </summary>
        private void LoadAvailableCalls()
        {
            CallsInRange = GetAllCalls();
            CallsListBox.ItemsSource = CallsInRange;

            // Afficher le message si aucun appel n'est disponible
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

        /// <summary>
        /// Active/Désactive la modification des informations du volontaire.
        /// </summary>
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsEditing)
            {
                // Activation des champs pour la modification
                SetEditableState(true);
                EditButton.Content = "💾 Enregistrer";
                EditButton.Background = System.Windows.Media.Brushes.SkyBlue;
                IsEditing = true;
            }
            else
            {
                try
                {
                    // Sauvegarde des nouvelles informations
                    CurrentVolunteer.Name = VolunteerNameTextBox.Text;
                    CurrentVolunteer.Phone = VolunteerPhoneTextBox.Text;
                    CurrentVolunteer.Address = VolunteerAddressTextBox.Text;
                    CurrentVolunteer.Mail = VolunteerEmailTextBox.Text;

                    // Mise à jour dans le système
                    s_bl.Volunteer.UpdateVolunteer(LoggedInId,CurrentVolunteer);
                    MessageBox.Show("Informations mises à jour avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Désactivation des champs après modification
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

        /// <summary>
        /// Permet de modifier l'état des champs (lecture seule ou éditable).
        /// </summary>
        private void SetEditableState(bool isEditable)
        {
            VolunteerNameTextBox.IsReadOnly = !isEditable;
            VolunteerPhoneTextBox.IsReadOnly = !isEditable;
            VolunteerAddressTextBox.IsReadOnly = !isEditable;
            VolunteerEmailTextBox.IsReadOnly = !isEditable;

            var backgroundColor = isEditable ? System.Windows.Media.Brushes.White : System.Windows.Media.Brushes.LightGray;
            VolunteerNameTextBox.Background = backgroundColor;
            VolunteerPhoneTextBox.Background = backgroundColor;
            VolunteerAddressTextBox.Background = backgroundColor;
            VolunteerEmailTextBox.Background = backgroundColor;
        }

        /// <summary>
        /// Assigne un appel sélectionné au volontaire.
        /// </summary>
        private void AssignCallButton_Click(object sender, RoutedEventArgs e)
        {
            if (CallsListBox.SelectedItem is BO.Call selectedCall)
            {
                try
                {
                    s_bl.Call.ChoiceCall(CurrentVolunteer.Id, selectedCall.Id);
                    MessageBox.Show($"📞 Appel '{selectedCall.Description}' assigné avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Mettre à jour les informations et la liste
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

        /// <summary>
        /// Ferme la fenêtre.
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
