
using BlApi;
using BO;
using Helpers;
using System.Net.Http.Headers;
using System.Security.Cryptography;

namespace BlImplementation;

internal class CallImplementation : ICall
{
    private DalApi.IDal _dal = Factory.Get;
    static readonly Tools toolsInstance = new Tools();
    public void ChoiceCall(int volunteerId, int callId)
    {
        // Retrieve the volunteer data
        var volunteer = _dal.Volunteer.Read(volunteerId);
        if (volunteer == null)
        {
            throw new BlNotFoundException("Volunteer not found.");
        }

        // Retrieve the call data
        var call = _dal.Call.Read(callId);
        if (call == null)
        {
            throw new BlNotFoundException("Call not found.");
        }

        // Convert the call to BO entity to include the status field
        var boCall = toolsInstance.parseDoToBoCall(call);

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
            StartTreatment = DateTime.Now,
            endTreatment = null,
            typeOfEnd = null
        };

        // Add the new assignment to the data layer
        _dal.Assignment.Create(newAssignment);
    }


    public async void CreateCall(BO.Call call)
    {
        try
        {
            // 1. Validation du format des champs
            toolsInstance.ValidateCallFieldsFormat(call);

            // 2. Vérifier si l'appel existe déjà dans la couche DAL
            try
            {
                var existingCall = _dal.Call.Read(call.Id);
                if (existingCall != null)
                {
                    throw new BlAlreadyExistsException("The call already exist");
                }
            }
            catch (DO.DalAlreadyExistException ex)
            {
                throw new BlAlreadyExistsException("The call already exist", ex);
            }

            // 3. Conversion de BO.Call en DO.Call
            DO.Call doCall = new DO.Call
            {
                Id = call.Id,
                CallType = (DO.callType)call.CallType,
                Description = call.Description,
                Address = await toolsInstance.GetAddressAsync(call.Latitude, call.Longitude),
                Latitude = call.Latitude ?? 0,
                Longitude = call.Longitude ?? 0,
                CallTime = call.Created,
                MaxTime = call.MaxEndTreatment
            };

            // 4. Ajout de l'appel dans la base de données via DAL
            _dal.Call.Create(doCall);
        }
        catch (BlAlreadyExistsException)
        {
            throw; // Relance l'exception pour que la couche d'affichage la capture
        }
        catch
        {
            // Gestion des erreurs générales
            throw new BlInvalidInputException("Invalid inputs");
        }
    }

    public void DeleteCall(int id)
    {
        BO.Call myCall = ReadCall(id);
        var assign = _dal.Assignment.ReadAll().Where(a=>a.CallId==id).FirstOrDefault();
        int voulunteerAssigned = assign!.VolunteerId;
        try
        {
            if (myCall == null || myCall.Status!=Status.Open|| !toolsInstance.isDirector(voulunteerAssigned)) 
            {
                throw new BlDeletionImpossible($"The coditions to delete the call Id: {id} are not match");
            }

            _dal.Call.Delete(id);

        }
        catch 
        { 
            throw new BlDeletionImpossible($"The coditions to delete the call Id: {id} are not match");
        }
    }



    public int[] GetCallCountsByStatus()
    {
        // Lire tous les appels depuis la couche DAL
        var doCalls = _dal.Call.ReadAll();

        // Convertir les objets DO en BO tout en calculant le statut
        var boCalls = doCalls.Select(doCall =>
        {
            var boCall = toolsInstance.parseDoToBoCall(doCall);
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

    public IEnumerable<CallInList> ReadAllCalls(CallFields? filter, object? obj, CallFields? sort)
    {
        var calls = _dal.Call.ReadAll();
        var assign = _dal.Assignment.ReadAll();
        var volunteers = _dal.Volunteer.ReadAll();
 
                    
        var xx = from call in calls
                 let validAssign= assign.Where(a => a.CallId == call.Id).OrderByDescending(a => a.StartTreatment)
                 let lastAssignment = assign.Where(assign => assign.CallId == call.Id&&(assign.typeOfEnd == DO.typeOfEndTreatment.treated || assign.typeOfEnd == DO.typeOfEndTreatment.Expired||assign.typeOfEnd==null)).FirstOrDefault()
                 let lastVolunteer = lastAssignment != null
      ? volunteers.FirstOrDefault(v => v.Id == lastAssignment.VolunteerId)
      : (validAssign.Any() ? volunteers.FirstOrDefault(v => v.Id == validAssign.FirstOrDefault().VolunteerId) : null)

                 select new CallInList
                 {
                     Id = call.Id,
                     callId = call.Id,
                     callType = (BO.callType)call.CallType,
                     startingTime = lastAssignment.StartTreatment,
                     remainingTime = call.MaxTime.HasValue
                         ? call.MaxTime.Value - ClockManager.Now
                         : null,
                     LastVolunteerName = lastVolunteer?.Name,
                     duration = lastAssignment?.endTreatment.HasValue == true 
                         ? lastAssignment.endTreatment - lastAssignment.StartTreatment
                         : null,
                     Status = toolsInstance.DetermineCallStatus(call.CallTime,call.MaxTime),
                     TotalAssignmentAllocations = validAssign.Count()
                 };
      if(filter.HasValue&& obj!=null)
        {
            switch (filter.Value)
            {
                case CallFields.Id:
                    xx = xx.Where(c => c.Id == (int)obj);
                    break;
                case CallFields.CallId:
                    xx = xx.Where(c => c.callId == (int)obj);
                    break;
                case CallFields.callType:
                    xx = xx.Where(c => c.callType == (BO.callType)obj);
                    break;
                case CallFields.startingTime:
                    xx = xx.Where(c => c.startingTime == (DateTime)obj);
                    break;
                case CallFields.remainingTime:
                    xx = xx.Where(c => c.remainingTime == (TimeSpan)obj);
                    break;
                case CallFields.LastVolunteerName:
                    xx = xx.Where(c => c.LastVolunteerName == (string)obj);
                    break;
                case CallFields.duration:
                    xx = xx.Where(c => c.duration == (TimeSpan)obj);
                    break;
                case CallFields.Status:
                    xx = xx.Where(c => c.Status == (Status)obj);
                    break;
                case CallFields.totalAssignmentAllocation:
                    xx = xx.Where(c => c.TotalAssignmentAllocations == (int)obj);
                    break;

            }
        }
        if (sort.HasValue)
        {
            switch (sort.Value)
            {
                case CallFields.Id:
                    xx = xx.OrderBy(c => c.Id);
                    break;
                case CallFields.CallId:
                    xx = xx.OrderBy(c => c.callId);
                    break;
                case CallFields.callType:
                    xx = xx.OrderBy(c => c.callType);
                    break;
                case CallFields.startingTime:
                    xx = xx.OrderBy(c => c.startingTime);
                    break;
                case CallFields.remainingTime:
                    xx = xx.OrderBy(c => c.remainingTime);
                    break;
                case CallFields.LastVolunteerName:
                    xx = xx.OrderBy(c => c.LastVolunteerName);
                    break;
                case CallFields.duration:
                    xx = xx.OrderBy(c => c.duration);
                    break;
                case CallFields.Status:
                    xx = xx.OrderBy(c => c.Status);
                    break;
                case CallFields.totalAssignmentAllocation:
                    xx = xx.OrderBy(c => c.TotalAssignmentAllocations);
                    break;

            }
        }
        else
        {
            xx = xx.OrderBy(c => c.Id);
        }


        return xx;

    }

    public IEnumerable<ClosedCallInList> ReadAllEndedCalls(int id, callType? filter, closedCallFields? sort)
    {
        var assign = _dal.Assignment.ReadAll().Where(c => c.VolunteerId == id);
        var assignClosedCalls = assign.Where(a => a.typeOfEnd != null);
        var calls = _dal.Call.ReadAll();

        // second parameter is the filter
        var closedCalls = calls.Where(call => assignClosedCalls.Any(assignClosedCalls => assignClosedCalls.CallId == call.Id));
        var sortByCallType = filter switch
        {
            null => closedCalls,
            callType.BuyingFood => closedCalls.Where(c => c.CallType == (DO.callType)callType.BuyingFood),
            callType.BuyingMedicine => closedCalls.Where(c => c.CallType == (DO.callType)callType.BuyingMedicine),
            callType.BuyingClothes => closedCalls.Where(c => c.CallType == (DO.callType)callType.BuyingClothes),
            callType.BuyingCartoons => closedCalls.Where(c => c.CallType == (DO.callType)callType.BuyingCartoons),
            callType.PackingFood => closedCalls.Where(c => c.CallType == (DO.callType)callType.PackingFood),
            callType.PackingMedicine => closedCalls.Where(c => c.CallType == (DO.callType)callType.PackingMedicine),
            callType.PackingClothes => closedCalls.Where(c => c.CallType == (DO.callType)callType.PackingClothes),
            callType.PackingCartoonsInTheTruck => closedCalls.Where(c => c.CallType == (DO.callType)callType.PackingCartoonsInTheTruck),
            callType.Deliveries => closedCalls.Where(c => c.CallType == (DO.callType)callType.Deliveries),
            callType.DelivriesToTheDoor => closedCalls.Where(c => c.CallType == (DO.callType)callType.DelivriesToTheDoor)
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
            null => xx.OrderBy(c => c.Id)
        };


    }



    public IEnumerable<OpenCallInList> ReadAllOpenCalls(int id, callType? filter, OpenCallFields? sort)
    {
        // Lire les informations sur le volontaire
        var myVolunteer = _dal.Volunteer.Read(id);
        if (myVolunteer == null || myVolunteer.Latitude == null || myVolunteer.Longitude == null || myVolunteer.MaxDistance == null)
            return Enumerable.Empty<OpenCallInList>(); // Si les données nécessaires sont manquantes, retourner une liste vide

        // Lire tous les appels et les convertir en BO
        var doCalls = _dal.Call.ReadAll();
        var boCalls = doCalls.Select(call => toolsInstance.parseDoToBoCall(call));

        // Filtrer les appels ouverts
        var openCalls = boCalls.Where(call => call.Status == Status.Open);

        // Calculer et filtrer les appels par distance
        var nearbyCalls = openCalls.Where(call =>
        {
            if (call.Latitude == null || call.Longitude == null) return false;
            double distance = toolsInstance.CalculateDistance(
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
            Address = toolsInstance.GetAddressAsync(call.Latitude, call.Longitude).Result,
            Distance = toolsInstance.CalculateDistance(
                myVolunteer.Latitude.Value,
                myVolunteer.Longitude.Value,
                call.Latitude!.Value,
                call.Longitude!.Value
            )
        });

        if (filter.HasValue)
        {
            result = result.Where(call => call.callType == filter.Value);
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
            OpenCallFields.Distance => result.OrderBy(call => call.Distance)
        };
    }




    public Call ReadCall(int id)
    {
        try
        {
            var call = _dal.Call.Read(id);
            if (call == null)
            {
                throw new BlDoesNotExistException("Call not found");
            }
            var boCall = toolsInstance.parseDoToBoCall(call);
            return boCall;

        }

        catch (DO.DalDoesNotExistException ex)
        {
            throw new BlNotFoundException("Call not found", ex);
        }
    }


    public void UpdateCall(Call call)
    {
        throw new NotImplementedException();
    }
    public void UpdateCallCancel(int requesterId, int assignmentId)
    {
        try
        {
            // Retrieve the assignment from the data layer
            var assign = _dal.Assignment.Read(assignmentId);

            // Check if the treatment has already ended
            if (assign.typeOfEnd != null)
            {
                throw new BlInvalidInputException("The call has already been closed.");
            }

            // Check if the requester has permission to cancel
            bool isManager = toolsInstance.isDirector(requesterId); // Hypothetical check for manager status
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
                endTreatment = DateTime.Now
            };

            // Update the record in the data layer
            _dal.Assignment.Update(updatedAssignment);
        }
        catch (Exception ex)
        {
            // Catch other exceptions and rethrow them with meaningful messages
            throw new BlInvalidInputException("An error occurred while canceling the call.", ex);
        }
    }


    public void UpdateCallEnd(int volunteerId, int assignmentId)
    {
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
                endTreatment = DateTime.Now
            };

            // Update the record in the data layer
            _dal.Assignment.Update(updatedAssignment);
        }
        
        catch (Exception ex)
        {
            // Catch other exceptions and rethrow them with meaningful messages
            throw new BlInvalidInputException("An error occurred while updating the call.", ex);
        }
    }
}