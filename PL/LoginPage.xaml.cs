using System;
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
            string id = IdTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Veuillez remplir tous les champs.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var volunteer = s_bl.Volunteer.ReadVolunteer(int.Parse(id));
            string myPassword = volunteer.Password!;
            if (myPassword == password)
            {
                if (volunteer.Job == BO.jobType.Director)
                {
                    var adminWindow = new HomePage();
                    adminWindow.Show();
                }
                else
                {
                    var volunteerWindow = new VolunteerWindow(volunteer);
                    volunteerWindow.Show();
                }
                Close();
            }
            else
            {
                MessageBox.Show("ID ou mot de passe incorrect.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }


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

            // Récupération du mot de passe depuis une base de données fictive
            string password = GetPasswordForId(id);

            if (password == null)
            {
                MessageBox.Show("Identifiant introuvable.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Récupérer l'email du volontaire
                var volunteer = s_bl.Volunteer.ReadVolunteer(int.Parse(id));
                string volunteerMail = volunteer.Mail;

                // Envoyer l'email
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
                return volunteer.Password;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void SendEmailWithMailKit(string recipientEmail, string password)
        {
            // Créer le message
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Application", "envoyeurdemails@gmail.com")); // Remplacez par votre email
            message.To.Add(new MailboxAddress("rubensbensimon@gmail.com", recipientEmail));
            message.Subject = "Récupération de mot de passe";

            message.Body = new TextPart("plain")
            {
                Text = $"Bonjour,\n\nVotre mot de passe est : {password}\n\nCordialement,\nL'équipe de support"
            };

            // Configurer le client SMTP
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect("smtp.gmail.com", 465, SecureSocketOptions.StartTls); // Connexion au serveur
                    client.Authenticate("envoyeurdemails@gmail.com", "bajl phja cxsf aftg"); // Remplacez par vos identifiants
                    client.Send(message); // Envoi du message
                    client.Disconnect(true); // Déconnexion propre
                }
                catch (Exception ex)
                {
                    throw new Exception($"Erreur SMTP : {ex.Message}");
                }
            }
        }
    }
}
