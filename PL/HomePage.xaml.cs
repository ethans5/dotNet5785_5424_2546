// HomePage.xaml.cs
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using BlApi;
using BO;

namespace PL
{
    public partial class HomePage : Window
    {
        private readonly IBl s_bl = BlApi.Factory.Get();
        private int LoggedInId;
        public int ConfigValue { get; set; } = 0;
        public HomePage(int loggedInId)
        {
            InitializeComponent();
            LoggedInId = loggedInId;

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
        private void ManageVolunteers_Click(object sender, RoutedEventArgs e)
        {
            new Volunteer.VolunteerList(LoggedInId).ShowDialog();
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
            LoginPage loginPage = new LoginPage();
            loginPage.Show();
            Close();
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