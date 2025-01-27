using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using BlApi;
using BO;


namespace PL
{
    public partial class HomePage : Window
    {
        private readonly IBl s_bl = BlApi.Factory.Get();
        public int ConfigValue { get; set; } = 0;

        // Constructeur avec injection de IAdmin
        public HomePage(IAdmin admin)
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // Mettre à jour l'horloge toutes les secondes
            //s_bl.Admin.AddClockObserver(UpdateClockText);
            //s_bl.Admin.UpdateClock(UnitTime.seconds);
        }
        public HomePage()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // Mettre à jour l'horloge toutes les secondes
            //s_bl.Admin.AddClockObserver(UpdateClockText);
            timer.Tick += (sender, e) => UpdateClockText();
        }




        // Met à jour l'affichage de l'horloge
        private void UpdateClockText()
        {
            
            ClockText.Text = $"Horloge :" + s_bl.Admin.GetSystemeClock().ToString("dd/MM/yyyy HH:mm:ss");
            

        }

        // Gestion des événements de l'horloge
        private void AddMinute_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.UpdateClock(UnitTime.Minutes);
            UpdateClockText();
        }

        private void AddHour_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.UpdateClock(UnitTime.Hours);
            UpdateClockText();
        }

        private void AddDay_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.UpdateClock(UnitTime.Days);
            UpdateClockText();
        }

        private void AddMonth_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.UpdateClock(UnitTime.Months);
            UpdateClockText();
        }

        private void AddYear_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.UpdateClock(UnitTime.Years);
            UpdateClockText();
        }

        // Gestion des autres boutons
        private void ManageVolunteers_Click(object sender, RoutedEventArgs e)
        {
            new Volunteer.VolunteerList().ShowDialog();
        }

        private void ManageCalls_Click(object sender, RoutedEventArgs e)
        {
            Call.CallInList callInList = new Call.CallInList();
            callInList.ShowDialog();
        }

        private void InitializeDB_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.InitializaData();
            UpdateClockText();
            riskTxtBox.Text = s_bl.Admin.GetRiskRange().TotalMinutes.ToString();
            MessageBox.Show("Base de données initialisée avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ResetDB_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ResetData();
            MessageBox.Show("Base de données réinitialisée avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExitApp_Click(object sender, RoutedEventArgs e)
        {
            // Ouvrir la fenêtre de connexion avant de fermer la fenêtre actuelle
            LoginPage loginPage = new LoginPage();
            loginPage.Show();

            // Fermer la fenêtre actuelle
            this.Close();
        }




        private void UpdateConfigValue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(riskTxtBox.Text, out int riskValue))
                {
                    MessageBox.Show("Veuillez entrer une valeur entière valide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                ConfigValue = riskValue;
                s_bl.Admin.UpdateRiskRange(TimeSpan.FromMinutes(ConfigValue));

                // Simuler une mise à jour en base de données
                MessageBox.Show($"Valeur mise à jour : {ConfigValue}", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Une erreur s'est produite : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NumericOnlyTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }


    }
}


