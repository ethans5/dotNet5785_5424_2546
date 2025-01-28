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
        private List<BO.Call> CallsInRange; // List of unassigned calls within the volunteer's travel range
        private BO.Call CurrentCall; // Simulates the current assigned call
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public VolunteerWindow(BO.Volunteer volunteer)
        {
            InitializeComponent();

            // Assign the provided volunteer
            CurrentVolunteer = volunteer;

            // Get all open calls within range
            CallsInRange = GetAllCalls();

            // Check if the volunteer has an ongoing call
            CurrentCall = GetCurrentCall(volunteer.Id);

            // Load the appropriate content
            LoadDynamicContent();
        }


        private void LoadDynamicContent()
        {
            if (CurrentCall == null)
            {
                // Volunteer has no ongoing call
                DisplayUnassignedCalls();
            }
            else
            {
                // Volunteer has an ongoing call
                DisplayCurrentCall();
            }
        }

        private void DisplayUnassignedCalls()
        {
            var listBox = new ListBox
            {
                ItemsSource = CallsInRange,
                DisplayMemberPath = "Description",
                Margin = new Thickness(10)
            };

            listBox.SelectionChanged += (sender, args) =>
            {
                if (listBox.SelectedItem is BO.Call selectedCall)
                {
                    AssignCallToVolunteer(selectedCall);
                }
            };

            DynamicContent.Content = listBox;
        }

        private void DisplayCurrentCall()
        {
            var stackPanel = new StackPanel
            {
                Margin = new Thickness(10),
                Orientation = Orientation.Vertical
            };

            stackPanel.Children.Add(new TextBlock
            {
                Text = "Current Call Details:",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 10)
            });

            stackPanel.Children.Add(new TextBlock
            {
                Text = $"Call ID: {CurrentCall.Id}",
                FontSize = 16
            });

            stackPanel.Children.Add(new TextBlock
            {
                Text = $"Description: {CurrentCall.Description}",
                FontSize = 16
            });

            stackPanel.Children.Add(new TextBlock
            {
                Text = $"Address: {CurrentCall.Address}",
                FontSize = 16
            });

            var completeButton = new Button
            {
                Content = "Complete Call",
                Height = 40,
                Width = 200,
                Margin = new Thickness(0, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left
            };

            completeButton.Click += (sender, args) =>
            {
                CompleteCall();
            };

            stackPanel.Children.Add(completeButton);

            DynamicContent.Content = stackPanel;
        }

        private void AssignCallToVolunteer(BO.Call call)
        {
            try
            {
                // Call the business logic layer to assign the call
                s_bl.Call.ChoiceCall(CurrentVolunteer.Id, call.Id);

                // Simulate the assignment by setting the current call
                CurrentCall = call;

                MessageBox.Show($"Call assigned to {CurrentVolunteer.Name}.", "Call Assigned", MessageBoxButton.OK, MessageBoxImage.Information);

                // Reload the content dynamically
                LoadDynamicContent();
            }
            catch (BlNotFoundException ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Assignment Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BlUnauthorizedException ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Assignment Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Assignment Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CompleteCall()
        {
            if (CurrentCall != null)
            {
                // Simulate the completion of the call
                CurrentCall = null;

                MessageBox.Show($"Call completed successfully.", "Call Completed", MessageBoxButton.OK, MessageBoxImage.Information);

                // Reload dynamic content
                LoadDynamicContent();
            }
        }

        private List<BO.Call> GetAllCalls()
        {
            // Get all open calls for the volunteer's range
            var openCalls = s_bl.Call.ReadAllOpenCalls(
                CurrentVolunteer.Id,
                null, // No specific filter for call type
                null  // No specific sorting
            );

            return openCalls.Select(call => new BO.Call
            {
                Id = call.Id,
                Description = call.description,
                Address = call.Address
            }).ToList();
        }

        private BO.Call GetCurrentCall(int volunteerId)
        {
            // Obtenir tous les appels ouverts pour le volontaire
            var openCalls = s_bl.Call.ReadAllOpenCalls(volunteerId, null, null);

            // Rechercher un appel en cours pour le volontaire
            foreach (var openCall in openCalls)
            {
                // Récupérer les détails complets de l'appel
                var detailedCall = s_bl.Call.ReadCall(openCall.Id);

                // Vérifier si le statut de l'appel est "InProgress"
                if (detailedCall.Status == Status.InProgress)
                {
                    return new BO.Call
                    {
                        Id = detailedCall.Id,
                        Description = detailedCall.Description,
                        Address = detailedCall.Address
                    };
                }
            }

            // Aucun appel en cours trouvé
            return null;
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
