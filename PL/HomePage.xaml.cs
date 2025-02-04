using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using BlApi;
using BO;

namespace PL
{
    public partial class HomePage : Window, INotifyPropertyChanged
    {
        private readonly IBl s_bl = BlApi.Factory.Get();
        private int LoggedInId;

        private bool _isSimulatorRunning = false;
        private int _interval = 1; // Valeur par défaut

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Indique si le simulateur est en cours d'exécution
        /// </summary>
        public bool IsSimulatorRunning
        {
            get => _isSimulatorRunning;
            set
            {
                if (_isSimulatorRunning != value)
                {
                    _isSimulatorRunning = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(SimulatorButtonText));
                    OnPropertyChanged(nameof(IsIntervalEditable));
                    OnPropertyChanged(nameof(IsControlsEnabled));
                }
            }
        }

        /// <summary>
        /// Intervalle de progression (en minutes)
        /// </summary>
        public int Interval
        {
            get => _interval;
            set
            {
                if (_interval != value)
                {
                    _interval = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Texte dynamique du bouton de simulation
        /// </summary>
        public string SimulatorButtonText => IsSimulatorRunning ? "Arrêter le simulateur" : "Démarrer le simulateur";

        /// <summary>
        /// Définit si la TextBox d'intervalle est modifiable
        /// </summary>
        public bool IsIntervalEditable => !IsSimulatorRunning;

        /// <summary>
        /// Désactive les boutons de gestion du temps et la configuration lorsqu'il tourne
        /// </summary>
        public bool IsControlsEnabled => IsSimulatorRunning = false;

        public HomePage(int loggedInId)
        {
            InitializeComponent();
            DataContext = this;
            LoggedInId = loggedInId;

            // Mise à jour automatique de l'horloge toutes les secondes
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += (sender, e) => UpdateClockText();
            timer.Start();
        }

        private void UpdateClockText()
        {
            ClockText.Text = $"Horloge : {s_bl.Admin.GetSystemeClock():dd/MM/yyyy HH:mm:ss}";
        }

        /// <summary>
        /// Gestion du simulateur (Démarrage / Arrêt)
        /// </summary>
        private void ToggleSimulator_Click(object sender, RoutedEventArgs e)
        {
            if (IsSimulatorRunning)
            {
                s_bl.Admin.StopSimulator();
                IsSimulatorRunning = false;
            }
            else
            {
                s_bl.Admin.StopSimulator();
                IsSimulatorRunning = true;
                s_bl.Admin.StartSimulator(Interval);
               
            }
        }

        // 🔹 Mise à jour de l'horloge système (désactivé si le simulateur tourne)
        private void AddMinute_Click(object sender, RoutedEventArgs e)
        {
            if (!IsSimulatorRunning) { s_bl.Admin.UpdateClock(UnitTime.Minutes); UpdateClockText(); }
        }

        private void AddHour_Click(object sender, RoutedEventArgs e)
        {
            if (!IsSimulatorRunning) { s_bl.Admin.UpdateClock(UnitTime.Hours); UpdateClockText(); }
        }

        private void AddDay_Click(object sender, RoutedEventArgs e)
        {
            if (!IsSimulatorRunning) { s_bl.Admin.UpdateClock(UnitTime.Days); UpdateClockText(); }
        }

        private void AddMonth_Click(object sender, RoutedEventArgs e)
        {
            if (!IsSimulatorRunning) { s_bl.Admin.UpdateClock(UnitTime.Months); UpdateClockText(); }
        }

        private void AddYear_Click(object sender, RoutedEventArgs e)
        {
            if (!IsSimulatorRunning) { s_bl.Admin.UpdateClock(UnitTime.Years); UpdateClockText(); }
        }

        private void ManageVolunteers_Click(object sender, RoutedEventArgs e)
        {
            new Volunteer.VolunteerList(LoggedInId).ShowDialog();
        }

        private void ManageCalls_Click(object sender, RoutedEventArgs e)
        {

            Call.CallInList callInList = new Call.CallInList(LoggedInId);
            callInList.ShowDialog();
        }

        private void InitializeDB_Click(object sender, RoutedEventArgs e)
        {
            if (!IsSimulatorRunning)
            {
                s_bl.Admin.InitializaData();
                UpdateClockText();
                riskTxtBox.Text = s_bl.Admin.GetRiskRange().TotalMinutes.ToString();
                MessageBox.Show("Base de données initialisée avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ResetDB_Click(object sender, RoutedEventArgs e)
        {
            if (!IsSimulatorRunning)
            {
                s_bl.Admin.ResetData();
                MessageBox.Show("Base de données réinitialisée avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExitApp_Click(object sender, RoutedEventArgs e)
        {
            if (IsSimulatorRunning)
            {
                s_bl.Admin.StopSimulator();
                IsSimulatorRunning = false;
            }
            new LoginPage().Show();
            Close();
        }

        private void UpdateConfigValue_Click(object sender, RoutedEventArgs e)
        {
            if (!IsSimulatorRunning)
            {
                try
                {
                    if (!int.TryParse(riskTxtBox.Text, out int riskValue))
                    {
                        MessageBox.Show("Veuillez entrer une valeur entière valide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    s_bl.Admin.UpdateRiskRange(TimeSpan.FromMinutes(riskValue));

                    MessageBox.Show($"Valeur mise à jour : {riskValue}", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Une erreur s'est produite : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void NumericOnlyTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }
    }
}
