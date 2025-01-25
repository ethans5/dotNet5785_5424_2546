using System;
using System.Windows;
using BlApi;
using BO;

namespace PL
{
    public partial class HomePage : Window
    {
        private readonly IBl s_bl = BlApi.Factory.Get();

        // Constructeur avec injection de IAdmin
        public HomePage(IAdmin admin)
        {
            InitializeComponent();
            UpdateClockText();
        }
        public HomePage()
        {
            InitializeComponent();
            UpdateClockText();
        }

        // Met à jour l'affichage de l'horloge
        private void UpdateClockText()
        {
            ClockText.Text = $"Horloge : {s_bl.Admin.GetSystemeClock():F}";
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
            MessageBox.Show("Gestion des appels (non implémentée)", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void InitializeDB_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.InitializaData();
            UpdateClockText();
            MessageBox.Show("Base de données initialisée avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ResetDB_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ResetData();
            MessageBox.Show("Base de données réinitialisée avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExitApp_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
       

    }
}
