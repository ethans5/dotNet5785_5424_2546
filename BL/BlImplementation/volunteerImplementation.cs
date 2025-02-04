using BlApi;
using BO;
using DO;
using Helpers;

namespace BlImplementation;

internal class VolunteerImplementation : IVolunteer
{
    private DalApi.IDal _dal = Factory.Get;

    public void CreateVolunteer(BO.Volunteer volunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  // Vérifier que le simulateur n'est pas en cours

        try
        {
            Tools.ValidateFieldsFormat(volunteer); // Vérifier le format des champs

            // Création de l'objet DO.Volunteer sans coordonnées GPS
            DO.Volunteer doVolunteer = new DO.Volunteer
            {
                Id = volunteer.Id,
                Name = volunteer.Name,
                Phone = volunteer.Phone,
                Email = volunteer.Mail,
                Password = volunteer.Password,
                Address = volunteer.Address,
                Latitude = null, // Les coordonnées seront mises à jour plus tard
                Longitude = null,
                JobType = (DO.jobType)volunteer.Job,
                isActive = volunteer.IsActive,
                MaxDistance = volunteer.MaxDistance,
                distanceType = (DO.distanceType)volunteer.DistanceType
            };

            // Ajouter le volontaire en base de données immédiatement
            lock (AdminManager.BlMutex)
            {
                _dal.Volunteer.Create(doVolunteer);
            }

            // Notifier l'interface utilisateur
            VolunteerManager.Observers.NotifyListUpdated();

            // Lancer la mise à jour des coordonnées GPS en arrière-plan
            _ = UpdateVolunteerCoordinatesAsync(doVolunteer);
        }
        catch (DO.DalAlreadyExistException ex)
        {
            Console.WriteLine($"Erreur : Le volontaire existe déjà - {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur inattendue : {ex.Message}");
        }
    }

    // Fonction asynchrone pour mettre à jour les coordonnées GPS du volontaire
    private async Task UpdateVolunteerCoordinatesAsync(DO.Volunteer doVolunteer)
    {
        try
        {
            if (!string.IsNullOrEmpty(doVolunteer.Address))
            {
                var coordinates = await Tools.GetCoordinatesAsync(doVolunteer.Address);
                if (coordinates.Latitude != null && coordinates.Longitude != null)
                {
                    // Mise à jour des coordonnées GPS
                    var updatedVolunteer = doVolunteer with { Latitude = coordinates.Latitude, Longitude = coordinates.Longitude };

                    lock (AdminManager.BlMutex)
                    {
                        _dal.Volunteer.Update(updatedVolunteer);
                    }

                    // Notifier que les coordonnées ont été mises à jour
                    VolunteerManager.Observers.NotifyItemUpdated(doVolunteer.Id);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Impossible de récupérer les coordonnées GPS : {ex.Message}");
        }
    }

    public void DeleteVolunteer(int id)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        lock (AdminManager.BlMutex)
        {
            BO.Volunteer? volunteer = ReadVolunteer(id);
            if (volunteer is null || volunteer.IsActive || volunteer.CallInProgress != null
                || volunteer.Totaltreated != 0)
                throw new BlDeletionImpossible($"Deletion of the Volunteer with ID {id} impossible");
        }
        try
        {
            lock (AdminManager.BlMutex)
            {
                _dal.Volunteer.Delete(id);
            }
            VolunteerManager.Observers.NotifyListUpdated();
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlNotFoundException($"Volunteer with ID={id} does not exist", ex);
       }
    }
    public BO.jobType LogIn(int id, string password)
    {
        try
        {
            lock (AdminManager.BlMutex)
            {
                var volunteer = ReadVolunteer(id);

                if (volunteer.Password == password)
                {
                    return volunteer.Job;
                }
                else
                {
                    throw new WrongPassword("Wrong password");
                }
            }
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("Volunteer does not exist", ex);
        }
    }

    public IEnumerable<VolunteerInList> ReadAllVolunteers(bool? actif, VolunteerSortField? sortBy)
    {
        lock (AdminManager.BlMutex)
        {
            var allAssign = _dal.Assignment.ReadAll();
            var allCalls = _dal.Call.ReadAll();


            var volunteers = actif switch
            {
                null => _dal.Volunteer.ReadAll(),
                true => _dal.Volunteer.ReadAll().Where(v => v.isActive),
                false => _dal.Volunteer.ReadAll().Where(v => !v.isActive)
            };



            var volunteerList = from volunteer in volunteers
                                let volunteerAssignments = allAssign.Where(a => a.VolunteerId == volunteer.Id)
                                let activeCallId = volunteerAssignments
                                          .Where(a => a.endTreatment == null)
                                          .Select(a => a.CallId)
                                          .FirstOrDefault()
                                let activeCallType = allCalls
                                                     .Where(c => c.Id == activeCallId)
                                                     .Select(c => (BO.callType)c.CallType)
                                                     .FirstOrDefault()
                                select new VolunteerInList
                                {
                                    Id = volunteer.Id,
                                    Name = volunteer.Name,
                                    IsActive = volunteer.isActive,
                                    Totaltreated = volunteerAssignments.Count(a => a.typeOfEnd == DO.typeOfEndTreatment.treated),
                                    TotalSelfCancellation = volunteerAssignments.Count(a => a.typeOfEnd == DO.typeOfEndTreatment.selfCancellation),
                                    TotalExpired = volunteerAssignments.Count(a => a.typeOfEnd == DO.typeOfEndTreatment.Expired),
                                    IdCall = activeCallId,
                                    callType = activeCallType
                                };

            return sortBy switch
            {
                VolunteerSortField.Name => volunteerList.OrderBy(v => v.Name),
                VolunteerSortField.Totaltreated => volunteerList.OrderBy(v => v.Totaltreated),
                VolunteerSortField.TotalSelfCancellation => volunteerList.OrderBy(v => v.TotalSelfCancellation),
                VolunteerSortField.TotalExpired => volunteerList.OrderBy(v => v.TotalExpired),
                VolunteerSortField.CallType => volunteerList.OrderBy(v => v.callType),
                _ => volunteerList.OrderBy(v => v.Id)
            };

        }

    }
    public BO.Volunteer ReadVolunteer(int id)
    {
        try
        {
            lock (AdminManager.BlMutex)
            {
                DO.Volunteer doVolunteer = _dal.Volunteer.Read(id)!;
                var volunter = VolunteerManager.ParseDoToBoVolunteer(doVolunteer);
                volunter.CallInProgress = Tools.GetCallInProgress(id);
                return volunter;
            }
        }
        catch
        {
            throw new BlDoesNotExistException($"Volunteer with ID: {id} doesn't exist");
        }
    }

    public void UpdateVolunteer(int id, BO.Volunteer volunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  // Vérifier que le simulateur n'est pas en cours

        DO.Volunteer myVolunteer;
        DO.Volunteer existingVolunteer;

        // Vérifier l'existence du volontaire et l'autorisation de mise à jour sous `lock`
        lock (AdminManager.BlMutex)
        {
            existingVolunteer = _dal.Volunteer.Read(volunteer.Id)
                ?? throw new BlNotFoundException($"Volunteer with ID {volunteer.Id} not found.");

            myVolunteer = _dal.Volunteer.Read(id)
                ?? throw new BlNotFoundException($"Volunteer with ID {id} not found.");

            if (myVolunteer.JobType != DO.jobType.Director && id != volunteer.Id)
            {
                throw new BlUnauthorizedException("You are not authorized to update this volunteer.");
            }
        }

        // Créer une copie du volontaire pour la mise à jour
        DO.Volunteer updatedVolunteer = new DO.Volunteer
        {
            Id = volunteer.Id,
            Name = volunteer.Name,
            Phone = volunteer.Phone,
            Email = volunteer.Mail,
            Password = volunteer.Password,
            Address = volunteer.Address,
            Latitude = existingVolunteer.Latitude, // On garde les anciennes coordonnées au début
            Longitude = existingVolunteer.Longitude,
            JobType = (DO.jobType)volunteer.Job,
            isActive = volunteer.IsActive,
            MaxDistance = volunteer.MaxDistance,
            distanceType = (DO.distanceType)volunteer.DistanceType
        };

        // Vérifier l'autorisation de modifier le JobType
        if (existingVolunteer.JobType != (DO.jobType)volunteer.Job && myVolunteer.JobType != DO.jobType.Director)
        {
            throw new BlUnauthorizedException("You are not authorized to change the job of this volunteer.");
        }

        // Mise à jour du volontaire en base sous `lock`
        lock (AdminManager.BlMutex)
        {
            _dal.Volunteer.Update(updatedVolunteer);
        }

        // Notifier immédiatement l’interface utilisateur
        VolunteerManager.Observers.NotifyItemUpdated(id);
        VolunteerManager.Observers.NotifyListUpdated();

        // Lancer la mise à jour des coordonnées en arrière-plan
        _ = UpdateVolunteerCoordinatesAsync(updatedVolunteer);
    }

    // Fonction asynchrone pour récupérer les coordonnées GPS et mettre à jour la base de données
   

    public void AddObserver(Action listObserver) => VolunteerManager.Observers.AddListObserver(listObserver);


    public void AddObserver(int id, Action observer) => VolunteerManager.Observers.AddObserver(id, observer);
    

    public void RemoveObserver(Action listObserver) => VolunteerManager.Observers.RemoveListObserver(listObserver);

    public void RemoveObserver(int id, Action observer) => VolunteerManager.Observers.RemoveObserver(id, observer);

}

