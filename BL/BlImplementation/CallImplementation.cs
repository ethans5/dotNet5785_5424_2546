
using BlApi;
using BO;
using Helpers;
using System.Net.Http.Headers;

namespace BlImplementation;

internal class CallImplementation : ICall
{
    private DalApi.IDal _dal = Factory.Get;
    static readonly Tools toolsInstance = new Tools();
    public void ChoiceCall(int id, int idC)
    {
        throw new NotImplementedException();
    }

    public void CreateCall(BO.Call call)
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
        throw new NotImplementedException();
    }

    public int[] GetCallCountsByStatus()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<CallInList> ReadAllCalls(CallFields? filter, object? obj, CallFields? sort)
    {
        throw new NotImplementedException();
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
        assignClosedCalls = assignClosedCalls.Where(call=>sortByCallType.Any(sortByCallType=>sortByCallType.Id==call.CallId));

        //third parameter is the sort
        var xx = from assignCall in assignClosedCalls
                 join call in sortByCallType
                 on assignCall.CallId equals call.Id
                 select new ClosedCallInList
                 {
                     Id = call.Id,
                     CallType= (BO.callType)call.CallType,
                     Address= call.Address,
                     Created=call.CallTime,
                     StartTreatment = assignCall.StartTreatment,
                     EndTreatment = assignCall.endTreatment,
                     TypeOfEndTreatment = (BO.typeOfEndTreatment)assignCall.typeOfEnd!

                 };
        return  sort switch
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
        throw new NotImplementedException();
    }

    public Call ReadCall(int id)
    {
        throw new NotImplementedException();
    }

    public void UpdateCall(Call call)
    {
        throw new NotImplementedException();
    }

    public void UpdateCallCancel(int id, int idA)
    {
        throw new NotImplementedException();
    }

    public void UpdateCallEns(int id, int idA)
    {
        throw new NotImplementedException();
    }
}
