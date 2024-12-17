
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

    public void CreateCall(Call call)
    {
        
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
