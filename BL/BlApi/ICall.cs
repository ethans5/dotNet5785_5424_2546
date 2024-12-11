using BO;
using System.Runtime.CompilerServices;

namespace BlApi;

public interface ICall
{
    public int[] GetCallCountsByStatus(); // groupBy
    public IEnumerable<BO.CallInList> ReadAllCalls(CallFields? filter, Object? obj,CallFields? sort);
    public BO.Call ReadCall(int id);
    public void UpdateCall(BO.Call call);
    public void DeleteCall(int id);
    public void CreateCall(BO.Call call);
    public IEnumerable<ClosedCallInList> ReadAllEndedCalls(int id, typeOfEndTreatment? filetr, closedCallFields? sort);
}
