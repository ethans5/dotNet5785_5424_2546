// LoginPage.xaml.cs
using System;
using System.Diagnostics;
using System.Windows;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using PL.Volunteer;

namespace PL
{
    public partial class LoginPage : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public LoginPage()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string inputId = IdTextBox.Text;
            string inputPassword = PasswordBox.Password;

            if (string.IsNullOrEmpty(inputId) || string.IsNullOrEmpty(inputPassword))
            {
                MessageBox.Show("Veuillez remplir tous les champs.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                int.TryParse(inputId, out int myId);
                var volunteer = s_bl.Volunteer.ReadVolunteer(myId);
                string myPassword = volunteer.Password!;

                if (myPassword == inputPassword)
                {
                    if (volunteer.Job == BO.jobType.Director)
                    {
                        var adminWindow = new HomePage(myId);
                        adminWindow.Show();
                    }
                    else
                    {
                        var volunteerWindow = new VolunteerWindow(volunteer, myId);
                        volunteerWindow.Show();
                    }
                    Close();
                }
                else
                {
                    MessageBox.Show("ID ou mot de passe incorrect.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la connexion : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Debug.WriteLine($"ID: {IdTextBox.Text}, Password: {PasswordBox.Password}");
        }

        private void ForgotPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            string id = IdTextBox.Text;

            if (string.IsNullOrEmpty(id))
            {
                MessageBox.Show("Veuillez entrer votre identifiant (ID) pour récupérer votre mot de passe.",
                                "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string password = GetPasswordForId(id);

            if (password == null)
            {
                MessageBox.Show("Identifiant introuvable.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var volunteer = s_bl.Volunteer.ReadVolunteer(int.Parse(id));
                string volunteerMail = volunteer.Mail;

                SendEmailWithMailKit(volunteerMail, password);
                MessageBox.Show("Un email de récupération a été envoyé avec succès.",
                                "Mot de passe oublié", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'envoi de l'email : {ex.Message}",
                                "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetPasswordForId(string id)
        {
            try
            {
                var volunteer = s_bl.Volunteer.ReadVolunteer(int.Parse(id));
                return volunteer.Password!;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void SendEmailWithMailKit(string recipientEmail, string password)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Application", "envoyeurdemails@gmail.com"));
            message.To.Add(new MailboxAddress("rubensbensimon@gmail.com", recipientEmail));
            message.Subject = "Récupération de mot de passe";

            message.Body = new TextPart("plain")
            {
                Text = $"Bonjour,\n\nVotre mot de passe est : {password}\n\nCordialement,\nL'équipe de support"
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 465, SecureSocketOptions.StartTls);
                client.Authenticate("envoyeurdemails@gmail.com", "bajl phja cxsf aftg");
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}