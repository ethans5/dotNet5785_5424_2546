using BlApi;
using BO;
using DO;
using Helpers;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using callType = BO.callType;
using System.Globalization;
using DalApi;

namespace BlImplementation;

internal class CallImplementation : BlApi.ICall
{
    private DalApi.IDal _dal = Factory.Get;
    CultureInfo culture = CultureInfo.InvariantCulture;
    public void CreateCall(BO.Call call)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  // Vérifier que le simulateur n'est pas en cours

        try
        {
            Tools.ValidateCallFieldsFormat(call); // Vérifier le format des champs

            // Création de l'objet DO.Call sans coordonnées GPS
            DO.Call doCall = new DO.Call
            {
                CallType = (DO.callType)call.CallType,
                Description = call.Description,
                Address = call.Address!,
                Latitude = 0, // Les coordonnées seront mises à jour plus tard
                Longitude = 0,
                CallTime = call.Created,
                MaxTime = call.MaxEndTreatment
            };

            // Ajouter l’appel en base immédiatement
            lock (AdminManager.BlMutex)
            {
                _dal.Call.Create(doCall);
            }

            // Notifier l'interface utilisateur
            CallManager.Observers.NotifyListUpdated();

            // Lancer la mise à jour des coordonnées GPS en arrière-plan
            _ = UpdateCallCoordinatesAsync(doCall);
        }
        catch (DO.DalAlreadyExistException ex)
        {
            Console.WriteLine($"Erreur : L’appel existe déjà - {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur inattendue : {ex.Message}");
        }
    }

    // Fonction asynchrone pour mettre à jour les coordonnées GPS de l’appel
    private async Task UpdateCallCoordinatesAsync(DO.Call doCall)
    {
        try
        {
            if (!string.IsNullOrEmpty(doCall.Address))
            {
                var coordinates = await Tools.GetCoordinatesAsync(doCall.Address);
                if (coordinates.Latitude != null && coordinates.Longitude != null)
                {
                    // Mise à jour des coordonnées GPS
                    var updatedCall = doCall with { Latitude = (double)coordinates.Latitude, Longitude = (double)coordinates.Longitude };

                    lock (AdminManager.BlMutex)
                    {
                        _dal.Call.Update(updatedCall);
                    }

                    // Notifier que les coordonnées ont été mises à jour
                    CallManager.Observers.NotifyItemUpdated(doCall.Id);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Impossible de récupérer les coordonnées GPS : {ex.Message}");
        }
    }
    public BO.Call ReadCall(int id)
    {
        try
        {
            lock (AdminManager.BlMutex)
            {
                var call = _dal.Call.Read(id);
                if (call == null)
                {
                    throw new BlDoesNotExistException("Call not found");
                }
                var boCall = CallManager.parseDoToBoCall(call);
                return boCall;
            }

        }

        catch (DO.DalDoesNotExistException ex)
        {
            throw new BlNotFoundException("Call not found", ex);
        }
    }
    public int[] GetCallCountsByStatus()
    {
        lock (AdminManager.BlMutex)
        {
            // Lire tous les appels depuis la couche DAL
            var doCalls = _dal.Call.ReadAll();

            // Convertir les objets DO en BO tout en calculant le statut
            var boCalls = doCalls.Select(doCall =>
            {
                var boCall = CallManager.parseDoToBoCall(doCall);
                return boCall;
            });

            // Grouper les appels par leur statut
            var groupedByStatus = boCalls
                .GroupBy(call => (int)call.Status) // Utilise l'index numérique du statut
                .ToDictionary(group => group.Key, group => group.Count());

            // Trouver le nombre maximal de statuts (par exemple, Status peut aller de 0 à 4)
            int maxStatusIndex = Enum.GetValues(typeof(Status)).Cast<int>().Max();

            // Initialiser un tableau pour stocker les résultats
            int[] callCounts = new int[maxStatusIndex + 1];

            // Remplir le tableau avec les quantités pour chaque statut
            foreach (var kvp in groupedByStatus)
            {
                callCounts[kvp.Key] = kvp.Value;
            }

            return callCounts;
        }
    }

    public void ChoiceCall(int volunteerId, int callId)
    {
        // Retrieve the volunteer data
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        lock (AdminManager.BlMutex)
        {
            var volunteer = _dal.Volunteer.Read(volunteerId);
            if (volunteer == null)
            {
                throw new BlNotFoundException("Volunteer not found.");
            }
            var myVolunteer = VolunteerManager.ParseDoToBoVolunteer(volunteer);
            if (myVolunteer.CallInProgress != null || _dal.Assignment.ReadAll()
            .Any(a => a.VolunteerId == volunteerId && a.endTreatment == null))
            {
                throw new BlUnauthorizedException("The volunteer is already assigned to another call.");
            }


            // Retrieve the call data
            var call = _dal.Call.Read(callId);
            if (call == null)
            {
                throw new BlNotFoundException("Call not found.");
            }

            // Convert the call to BO entity to include the status field
            var boCall = CallManager.parseDoToBoCall(call);

            // Validate the call
            if (boCall.Status != Status.Open)
            {
                throw new BlUnauthorizedException("The call is not available for treatment.");
            }

            // Check if the call has expired
            if (boCall.Status == Status.Expired)
            {
                throw new BlUnauthorizedException("The call has expired.");
            }

            // Check if the call is already assigned to another volunteer
            var existingAssignments = _dal.Assignment.ReadAll()
                .Where(a => a.CallId == callId && a.endTreatment == null);
            if (existingAssignments.Any())
            {
                throw new BlUnauthorizedException("The call is already assigned to another volunteer.");
            }

            // Create a new assignment entity
            var newAssignment = new DO.Assignment
            {
                VolunteerId = volunteerId,
                CallId = callId,
                StartTreatment = AdminManager.Now,
                endTreatment = null,
                typeOfEnd = null
            };

            // Add the new assignment to the data layer
            _dal.Assignment.Create(newAssignment);

            // Update the call's status to InProgress
            boCall.Status = Status.InProgress;

            UpdateCall(boCall);
        }
        CallManager.Observers.NotifyItemUpdated(callId);
        CallManager.Observers.NotifyListUpdated();

    }

    public void DeleteCall(int id)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

        lock (AdminManager.BlMutex)
        {

            BO.Call myCall = ReadCall(id);
            var assign = _dal.Assignment.ReadAll().Where(a => a.CallId == id).FirstOrDefault();
            int voulunteerAssigned = assign!.VolunteerId;
            try
            {
                if (myCall == null || myCall.Status != Status.Open || !Tools.isDirector(voulunteerAssigned))
                {
                    throw new BlDeletionImpossible($"The coditions to delete the call Id: {id} are not match");
                }

                _dal.Call.Delete(id);
                CallManager.Observers.NotifyListUpdated();

            }
            catch
            {
                throw new BlDeletionImpossible($"The coditions to delete the call Id: {id} are not match");
            }
        }
    }

    public IEnumerable<CallInList> ReadAllCalls(CallFields? filter, object? obj, CallFields? sort)
    {
        lock (AdminManager.BlMutex)
        {
            // Read data from the data access layer
            var calls = _dal.Call.ReadAll();
            var assignments = _dal.Assignment.ReadAll();
            var volunteers = _dal.Volunteer.ReadAll();

            // Build the query to generate CallInList objects
            var callInList = from call in calls
                             let hasAssignments = assignments.Any(a => a.CallId == call.Id)
                             let validAssignments = hasAssignments
                              ? assignments.Where(a => a.CallId == call.Id).OrderByDescending(a => a.StartTreatment)
                              : Enumerable.Empty<Assignment>()
                             let lastAssignment = hasAssignments
                                 ? assignments.Where(a => a.CallId == call.Id &&
                                     (a.typeOfEnd == DO.typeOfEndTreatment.treated ||
                                      a.typeOfEnd == DO.typeOfEndTreatment.Expired ||
                                      a.typeOfEnd == null)).FirstOrDefault()
                                 : null
                             let lastVolunteer = hasAssignments && lastAssignment != null
                                 ? volunteers.FirstOrDefault(v => v.Id == lastAssignment.VolunteerId)
                                 : null
                             select new CallInList
                             {
                                 Id = call.Id,
                                 callId = call.Id,
                                 callType = (BO.callType)call.CallType,
                                 startingTime = call.CallTime,
                                 remainingTime = call.MaxTime.HasValue
        ? (call.MaxTime!.Value - AdminManager.Now) < TimeSpan.Zero
            ? TimeSpan.MinValue  // ou tu pourrais utiliser un autre indicateur pour "terminé"
            : call.MaxTime!.Value - AdminManager.Now
        : (TimeSpan?)null,


                                 LastVolunteerName = lastVolunteer?.Name,
                                 duration = hasAssignments && lastAssignment?.endTreatment.HasValue == true
                                 ? lastAssignment.endTreatment - lastAssignment.StartTreatment
                                 : null,
                                 Status = Tools.DetermineCallStatus(call.Id, call.CallTime, call.MaxTime),
                                 TotalAssignmentAllocations = hasAssignments ? validAssignments.Count() : 0
                             };

            // Apply filtering based on the given field and value
            if (filter.HasValue && obj != null)
            {
                switch (filter.Value)
                {
                    case CallFields.Id:
                        callInList = callInList.Where(c => c.Id == (int)obj);
                        break;
                    case CallFields.CallId:
                        callInList = callInList.Where(c => c.callId == (int)obj);
                        break;
                    case CallFields.callType:
                        callInList = callInList.Where(c => c.callType == (BO.callType)obj);
                        break;
                    case CallFields.startingTime:
                        callInList = callInList.Where(c => c.startingTime == (DateTime)obj);
                        break;
                    case CallFields.remainingTime:
                        callInList = callInList.Where(c => c.remainingTime == (TimeSpan)obj);
                        break;
                    case CallFields.LastVolunteerName:
                        callInList = callInList.Where(c => c.LastVolunteerName == (string)obj);
                        break;
                    case CallFields.duration:
                        callInList = callInList.Where(c => c.duration == (TimeSpan)obj);
                        break;
                    case CallFields.Status:
                        callInList = callInList.Where(c => c.Status == (Status)obj);
                        break;
                    case CallFields.totalAssignmentAllocation:
                        callInList = callInList.Where(c => c.TotalAssignmentAllocations == (int)obj);
                        break;
                }
            }

            // Apply sorting based on the given field
            if (sort.HasValue)
            {
                switch (sort.Value)
                {
                    case CallFields.Id:
                        callInList = callInList.OrderBy(c => c.Id);
                        break;
                    case CallFields.CallId:
                        callInList = callInList.OrderBy(c => c.callId);
                        break;
                    case CallFields.callType:
                        callInList = callInList.OrderBy(c => c.callType);
                        break;
                    case CallFields.startingTime:
                        callInList = callInList.OrderBy(c => c.startingTime);
                        break;
                    case CallFields.remainingTime:
                        callInList = callInList.OrderBy(c => c.remainingTime);
                        break;
                    case CallFields.LastVolunteerName:
                        callInList = callInList.OrderBy(c => c.LastVolunteerName);
                        break;
                    case CallFields.duration:
                        callInList = callInList.OrderBy(c => c.duration);
                        break;
                    case CallFields.Status:
                        callInList = callInList.OrderBy(c => c.Status);
                        break;
                    case CallFields.totalAssignmentAllocation:
                        callInList = callInList.OrderBy(c => c.TotalAssignmentAllocations);
                        break;
                }
            }
            else
            {
                // Default sorting by ID
                callInList = callInList.OrderBy(c => c.Id);
            }

            return callInList;
        }
    }


    public IEnumerable<ClosedCallInList> ReadAllEndedCalls(int id, BO.callType? filter, closedCallFields? sort)
    {
        lock (AdminManager.BlMutex)
        {
            var assign = _dal.Assignment.ReadAll().Where(c => c.VolunteerId == id);
            var assignClosedCalls = assign.Where(a => a.typeOfEnd != null);
            var calls = _dal.Call.ReadAll();

            // second parameter is the filter
            var closedCalls = calls.Where(call => assignClosedCalls.Any(assignClosedCalls => assignClosedCalls.CallId == call.Id));
            var sortByCallType = filter switch
            {

                callType.BuyingFood => closedCalls.Where(c => c.CallType == (DO.callType)callType.BuyingFood),
                callType.BuyingMedicine => closedCalls.Where(c => c.CallType == (DO.callType)callType.BuyingMedicine),
                callType.BuyingClothes => closedCalls.Where(c => c.CallType == (DO.callType)callType.BuyingClothes),
                callType.BuyingCartoons => closedCalls.Where(c => c.CallType == (DO.callType)callType.BuyingCartoons),
                callType.PackingFood => closedCalls.Where(c => c.CallType == (DO.callType)callType.PackingFood),
                callType.PackingMedicine => closedCalls.Where(c => c.CallType == (DO.callType)callType.PackingMedicine),
                callType.PackingClothes => closedCalls.Where(c => c.CallType == (DO.callType)callType.PackingClothes),
                callType.PackingCartoonsInTheTruck => closedCalls.Where(c => c.CallType == (DO.callType)callType.PackingCartoonsInTheTruck),
                callType.Deliveries => closedCalls.Where(c => c.CallType == (DO.callType)callType.Deliveries),
                callType.DelivriesToTheDoor => closedCalls.Where(c => c.CallType == (DO.callType)callType.DelivriesToTheDoor),
                _ => closedCalls
            };
            assignClosedCalls = assignClosedCalls.Where(call => sortByCallType.Any(sortByCallType => sortByCallType.Id == call.CallId));

            //third parameter is the sort
            var xx = from assignCall in assignClosedCalls
                     join call in sortByCallType
                     on assignCall.CallId equals call.Id
                     select new ClosedCallInList
                     {
                         Id = call.Id,
                         CallType = (BO.callType)call.CallType,
                         Address = call.Address,
                         Created = call.CallTime,
                         StartTreatment = assignCall.StartTreatment,
                         EndTreatment = assignCall.endTreatment,
                         TypeOfEndTreatment = (BO.typeOfEndTreatment)assignCall.typeOfEnd!

                     };
            return sort switch
            {
                closedCallFields.Id => xx.OrderBy(c => c.Id),
                closedCallFields.callType => xx.OrderBy(c => c.CallType),
                closedCallFields.adress => xx.OrderBy(c => c.Address),
                closedCallFields.createdTime => xx.OrderBy(c => c.Created),
                closedCallFields.startTreatment => xx.OrderBy(c => c.StartTreatment),
                closedCallFields.endTreatment => xx.OrderBy(c => c.EndTreatment),
                closedCallFields.typeOfEndTreatment => xx.OrderBy(c => c.TypeOfEndTreatment),
                _ => xx.OrderBy(c => c.Id)
            };

        }
    }

    public IEnumerable<OpenCallInList> ReadAllOpenCalls(int id, BO.callType? filter, OpenCallFields? sort)
    {
        lock (AdminManager.BlMutex)
        {
            // Lire les informations sur le volontaire
            var myVolunteer = _dal.Volunteer.Read(id);
            if (myVolunteer == null || myVolunteer.Latitude == null || myVolunteer.Longitude == null || myVolunteer.MaxDistance == null)
                return Enumerable.Empty<OpenCallInList>(); // Si les données nécessaires sont manquantes, retourner une liste vide

            // Lire tous les appels et les convertir en BO
            var doCalls = _dal.Call.ReadAll();
            var boCalls = doCalls.Select(call => CallManager.parseDoToBoCall(call));

            // Filtrer les appels ouverts
            var openCalls = boCalls.Where(call => call.Status == Status.Open);

            // Calculer et filtrer les appels par distance
            var nearbyCalls = openCalls.Where(call =>
            {
                if (call.Latitude == null || call.Longitude == null) return false;
                double distance = Tools.CalculateDistance(
                    myVolunteer.Latitude.Value,
                    myVolunteer.Longitude.Value,
                    call.Latitude.Value,
                    call.Longitude.Value
                );
                return distance <= myVolunteer.MaxDistance.Value;
            });
            // Appliquer le filtre sur le type de call, si nécessaire

            // Mapper les appels en OpenCallInList
            var result = nearbyCalls.Select(call => new OpenCallInList
            {
                Id = call.Id,
                callType = call.CallType,
                description = call.Description,
                Address = call.Address!,
                Created = call.Created,
                MaxEndTreatment = call.MaxEndTreatment,
                Distance = Tools.CalculateDistance(
                    myVolunteer.Latitude.Value,
                    myVolunteer.Longitude.Value,
                    call.Latitude!.Value,
                    call.Longitude!.Value
                )
            });

            if (filter.HasValue)
            {
                result = result.Where(call => call.callType == filter.Value);
                if (result.Count() == 0)
                    Console.WriteLine("Any calls with this filter exist");
            }



            // Appliquer le tri, si nécessaire
            return sort switch
            {
                OpenCallFields.Id => result.OrderBy(call => call.Id),
                OpenCallFields.callType => result.OrderBy(call => call.callType),
                OpenCallFields.description => result.OrderBy(call => call.description),
                OpenCallFields.Address => result.OrderBy(call => call.Address),
                OpenCallFields.Created => result.OrderBy(call => call.Created),
                OpenCallFields.MaxEndTreatment => result.OrderBy(call => call.MaxEndTreatment),
                OpenCallFields.Distance => result.OrderBy(call => call.Distance),
                _ => result.OrderBy(call => call.Id)
            };
        }
    }

    public void UpdateCall(BO.Call call)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  // Vérifier que le simulateur n'est pas en cours

        DO.Call existingCall;

        // Vérifier l'existence de l'appel sous `lock`
        lock (AdminManager.BlMutex)
        {
            existingCall = _dal.Call.Read(call.Id)
                ?? throw new BlNotFoundException($"Call with ID {call.Id} not found.");
        }

        // Créer une copie de l'appel pour la mise à jour
        DO.Call updatedCall = new DO.Call
        {
            Id = call.Id,
            CallType = (DO.callType)call.CallType,
            Description = call.Description,
            Address = call.Address!,
            Latitude = existingCall.Latitude, // Garder les anciennes coordonnées pour l'instant
            Longitude = existingCall.Longitude,
            CallTime = call.Created,
            MaxTime = call.MaxEndTreatment
        };

        // Mise à jour de l'appel en base sous `lock`
        lock (AdminManager.BlMutex)
        {
            _dal.Call.Update(updatedCall);
        }

        // Notifier immédiatement l’interface utilisateur
        CallManager.Observers.NotifyItemUpdated(call.Id);
        CallManager.Observers.NotifyListUpdated();

        // Lancer la mise à jour des coordonnées GPS en arrière-plan
        _ = UpdateCallCoordinatesAsync(updatedCall);
    }

    // Fonction asynchrone pour récupérer les coordonnées GPS et mettre à jour la base de données
   

    public void UpdateCallCancel(int requesterId, int assignmentId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        try
        {
            // Retrieve the assignment from the data layer
            var assign = _dal.Assignment.Read(assignmentId);
            if (assign == null)
            {
                throw new BlInvalidInputException("The assignment does not exist.");
            }


            // Check if the treatment has already ended
            if (assign.typeOfEnd != null)
            {
                throw new BlInvalidInputException("The call has already been closed.");
            }

            // Check if the requester has permission to cancel
            bool isManager = Tools.isDirector(requesterId); // Hypothetical check for manager status
            bool isVolunteerAssigned = assign.VolunteerId == requesterId;

            if (!isManager && !isVolunteerAssigned)
            {
                throw new BlUnauthorizedException("You do not have permission to cancel this assignment.");
            }

            // Determine the type of cancellation based on the requester
            var cancellationType = isVolunteerAssigned
                ? DO.typeOfEndTreatment.selfCancellation
                : DO.typeOfEndTreatment.directorCancellation;

            // Update the type of end and end treatment timestamp
            var updatedAssignment = assign with
            {
                typeOfEnd = cancellationType,
                endTreatment = AdminManager.Now
            };



            // Update the record in the data layer
            _dal.Assignment.Update(updatedAssignment);
            Tools.DetermineCallStatus(assign.CallId, assign.StartTreatment, assign.endTreatment);
            CallManager.Observers.NotifyItemUpdated(assign.CallId);
            CallManager.Observers.NotifyListUpdated();

        }
        catch (Exception ex)
        {
            // Catch other exceptions and rethrow them with meaningful messages
            throw new BlInvalidInputException("An error occurred while canceling the call.", ex);
        }
    }

    public void UpdateCallEnd(int volunteerId, int assignmentId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        try
        {
            // Retrieve the volunteer and assignment from the data layer
            var volunteer = _dal.Volunteer.Read(volunteerId);
            var assign = _dal.Assignment.Read(assignmentId);

            // Check if the assignment belongs to the given volunteer
            if (assign!.VolunteerId != volunteerId)
            {
                throw new BlNotFoundException("The assignment does not belong to this volunteer.");
            }

            // Check if the treatment has already ended
            if (assign.endTreatment != null)
            {
                throw new BlAlreadyExistsException("The call has already ended.");
            }

            //// Check if the treatment has started
            //if (assign.StartTreatment == null)
            //{
            //    throw new BlInvalidInputException("The call has not started yet.");
            //}

            // Update the type of end and end treatment timestamp
            var updatedAssignment = assign with
            {
                typeOfEnd = DO.typeOfEndTreatment.treated,
                endTreatment = AdminManager.Now
            };

            // Update the record in the data layer
            _dal.Assignment.Update(updatedAssignment);
            CallManager.Observers.NotifyItemUpdated(assign.CallId);
            CallManager.Observers.NotifyListUpdated();

        }

        catch (Exception ex)
        {
            // Catch other exceptions and rethrow them with meaningful messages
            throw new BlInvalidInputException("An error occurred while updating the call.", ex);
        }
    }

    public void AddObserver(Action listObserver) => CallManager.Observers.AddListObserver(listObserver);

    public void AddObserver(int id, Action observer) => CallManager.Observers.AddObserver(id, observer);

    public void RemoveObserver(Action listObserver) => CallManager.Observers.RemoveListObserver(listObserver);

    public void RemoveObserver(int id, Action observer) => CallManager.Observers.RemoveObserver(id, observer);

}