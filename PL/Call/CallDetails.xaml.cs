using System;
using System.Windows;
using BO;
using DO;

namespace PL.Call
{
    public partial class CallDetails : Window
    {
        public BO.Call Call { get; private set; }
        private bool _isCreating;
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public CallDetails(BO.Call? call = null, bool isCreating = true)
        {
            InitializeComponent();

            _isCreating = isCreating;
            Call = call ?? new BO.Call();
            DataContext = Call;

            // Remplir les ComboBox avec des valeurs d'énumérations
            CallTypeComboBox.ItemsSource = Enum.GetValues(typeof(BO.callType));
            StatusComboBox.ItemsSource = Enum.GetValues(typeof(Status));

            // Si on crée un nouvel appel
            if (_isCreating)
            {
                Call.Created = s_bl.Admin.GetSystemeClock();
                CreatedDateTextBox.Text = Call.Created.ToString("g"); // Format lisible
            }
            else
            {
                CreatedDateTextBox.Text = Call.Created.ToString("g");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ||
                    string.IsNullOrWhiteSpace(AddressTextBox.Text) ||
                    StatusComboBox.SelectedItem == null ||
                    CallTypeComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Veuillez remplir tous les champs obligatoires.", "Erreur de validation", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Call.Description = DescriptionTextBox.Text;
                Call.Address = AddressTextBox.Text;
                Call.MaxEndTreatment = MaxEndTreatmentDatePicker.SelectedDate;
                Call.Status = (Status)StatusComboBox.SelectedItem;
                Call.CallType = (BO.callType)CallTypeComboBox.SelectedItem;

                if (_isCreating)
                {
                    s_bl.Call.CreateCall(Call);
                    MessageBox.Show("L'appel a été créé avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    s_bl.Call.UpdateCall(Call);
                    MessageBox.Show("Les détails de l'appel ont été mis à jour avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur est survenue : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
