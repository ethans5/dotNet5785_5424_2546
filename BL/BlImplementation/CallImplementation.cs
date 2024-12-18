
using BlApi;
using BO;
using Helpers;

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
        throw new NotImplementedException();
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
