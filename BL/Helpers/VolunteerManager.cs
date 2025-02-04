using BO;
using DalApi;

namespace Helpers;

internal static class VolunteerManager
{
    private static readonly IDal _dal = Factory.Get; // Stage 4
    internal static ObserverManager Observers = new();
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    public static double? latitude;
    public static double? longitude;

   
    public static  BO.Volunteer ParseDoToBoVolunteer(DO.Volunteer volunteer)
    {
        if (volunteer == null)
            throw new ArgumentNullException(nameof(volunteer), "Volunteer cannot be null.");
        //var coordinates = Tools.GetCoordinatesAsync(volunteer.Address!);
        //latitude = coordinates.Result.Latitude;
        //longitude = coordinates.Result.Longitude;
        try
        {
            

            BO.Volunteer volunteer1 = new BO.Volunteer
            {
                Id = volunteer.Id,
                Name = volunteer.Name,
                Phone = volunteer.Phone,
                Mail = volunteer.Email,
                Password = volunteer.Password,
                Address = volunteer.Address,
                Latitude = volunteer.Latitude,
                Longitude = volunteer.Longitude,
                Job = (BO.jobType)volunteer.JobType,
                IsActive = volunteer.isActive,
                MaxDistance = volunteer.MaxDistance,
                DistanceType = (BO.distanceType)volunteer.distanceType,
                Totaltreated = Tools.totalTreated(volunteer.Id),
                TotalSelfCancellation = Tools.totalSelfCancelled(volunteer.Id),
                TotalExpired = Tools.totalExpired(volunteer.Id)
            };

            return volunteer1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred: {ex.Message}");
            throw;
        }
    }

    public static IEnumerable<BO.Volunteer> GetAvailableVolunteers()
    {
        lock (AdminManager.BlMutex)
        {
            var volunteers = _dal.Volunteer.ReadAll();
            return volunteers.Select(ParseDoToBoVolunteer).Where(v => v.CallInProgress == null);
        }
    }
    private static readonly Random s_rand = new(); // Générateur aléatoire
    private static int s_simulatorCounter = 0; // Compteur des simulations

    internal static void SimulateCallHandling()
    {
        Task.Run(() => // Exécuter de manière asynchrone
        {
            Thread.CurrentThread.Name = $"CallSimulator{++s_simulatorCounter}";

            List<int> callsToUpdate = new(); // Liste des appels mis à jour

            // Charger tous les volontaires actifs en dehors du `lock` avec `ToList()`
            List<DO.Volunteer> activeVolunteers;
            lock (AdminManager.BlMutex)
            {
                activeVolunteers = _dal.Volunteer.ReadAll().Where(v => v.isActive).ToList();
            }

            foreach (var volunteer in activeVolunteers)
            {
                
                BO.Call? boCall = null;

                // Vérifier si le volontaire a un appel en cours

                lock (AdminManager.BlMutex)
                {
                    var currentAssignment = _dal.Assignment.ReadAll()
                        .FirstOrDefault(a => a.VolunteerId == volunteer.Id && a.endTreatment == null);

                    if (currentAssignment != null)
                    {
                        var assignedCall = _dal.Call.Read(currentAssignment.CallId);
                        if (assignedCall != null)
                        {
                            boCall = CallManager.parseDoToBoCall(assignedCall);
                        }
                    }
                }

                // Si le volontaire n'a pas d'appel en cours
                if (boCall == null)
                {
                    // Lire toutes les appels ouverts avec `ToList()`
                    List<BO.Call> openCalls;
                    lock (AdminManager.BlMutex)
                    {
                        openCalls = _dal.Call.ReadAll()
                         .Select(c => CallManager.parseDoToBoCall(c)) // Conversion en BO.Call
                         .Where(c => (c.Status == Status.Open || c.Status == Status.OpenAlmostExpired)
                                     && c.Latitude != 0 && c.Longitude != 0)
                         .ToList();

                    }

                    // Sélectionner un appel aléatoire avec une probabilité de 20%
                    if (openCalls.Count > 0 && s_rand.NextDouble() < 0.2)
                    {
                        var selectedCall = openCalls[s_rand.Next(openCalls.Count)];

                        lock (AdminManager.BlMutex)
                        {
                            s_bl.Call.ChoiceCall(volunteer.Id, selectedCall.Id);
                        }

                        callsToUpdate.Add(selectedCall.Id);
                    }
                }
                else
                {
                    // Vérifier si l’appel en cours doit être clôturé
                    var elapsedMinutes = (AdminManager.Now - boCall.Created).TotalMinutes;
                    var distance = Tools.CalculateDistance(boCall.Latitude ?? 0, boCall.Longitude ?? 0,
                                                           volunteer.Latitude ?? 0, volunteer.Longitude ?? 0);
                    double requiredMinutes = distance / 5.0 + s_rand.Next(3, 10); // Facteur temps basé sur la distance

                    if (elapsedMinutes >= requiredMinutes)
                    {
                        lock (AdminManager.BlMutex)
                        {
                            s_bl.Call.UpdateCallEnd(volunteer.Id, boCall.Id);
                        }
                        callsToUpdate.Add(boCall.Id);
                    }
                    else if (s_rand.NextDouble() < 0.1) // Probabilité de 10% d'annuler l'appel
                    {
                        lock (AdminManager.BlMutex)
                        {
                            s_bl.Call.UpdateCallCancel(volunteer.Id, boCall.Id);
                        }
                        callsToUpdate.Add(boCall.Id);
                    }
                }
            }

            // Notifications en dehors du `lock`
            foreach (int id in callsToUpdate)
            {
                CallManager.Observers.NotifyItemUpdated(id);
            }
        });
    }

}
