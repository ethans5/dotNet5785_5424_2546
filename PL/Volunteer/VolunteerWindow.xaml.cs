// VolunteerWindow.xaml.cs
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
        private BO.Call CurrentCall;
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public VolunteerWindow(BO.Volunteer volunteer, int loggedInId)
        {
            InitializeComponent();

            CurrentVolunteer = volunteer;
            LoggedInId = loggedInId;

            CallsInRange = GetAllCalls();
            CurrentCall = GetCurrentCall(volunteer.Id);

            LoadDynamicContent();
        }

        private void LoadDynamicContent()
        {
            if (CurrentCall == null)
            {
                DisplayUnassignedCalls();
            }
            else
            {
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

            completeButton.Click += (sender, args) => CompleteCall();

            stackPanel.Children.Add(completeButton);

            DynamicContent.Content = stackPanel;
        }

        private void AssignCallToVolunteer(BO.Call call)
        {
            try
            {
                s_bl.Call.ChoiceCall(CurrentVolunteer.Id, call.Id);
                CurrentCall = call;

                MessageBox.Show($"Call assigned to {CurrentVolunteer.Name}.", "Call Assigned", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadDynamicContent();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}", "Assignment Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CompleteCall()
        {
            if (CurrentCall != null)
            {
                CurrentCall = null;
                MessageBox.Show("Call completed successfully.", "Call Completed", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadDynamicContent();
            }
        }

        private List<BO.Call> GetAllCalls()
        {
            var openCalls = s_bl.Call.ReadAllOpenCalls(
                CurrentVolunteer.Id,
                null,
                null
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
            var openCalls = s_bl.Call.ReadAllOpenCalls(volunteerId, null, null);

            foreach (var openCall in openCalls)
            {
                var detailedCall = s_bl.Call.ReadCall(openCall.Id);

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

            return null;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}