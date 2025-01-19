using BO;
using System.Runtime.CompilerServices;

namespace BlApi;

public interface ICall:IObservable
{
    public int[] GetCallCountsByStatus(); // groupBy
    public IEnumerable<BO.CallInList> ReadAllCalls(CallFields? filter, Object? obj,CallFields? sort);
    public BO.Call ReadCall(int id);
    public void UpdateCall(BO.Call call);
    public void DeleteCall(int id);
    public void CreateCall(BO.Call call);
    public IEnumerable<ClosedCallInList> ReadAllEndedCalls(int id, callType? filter, closedCallFields? sort);
    public IEnumerable<OpenCallInList> ReadAllOpenCalls(int id, callType? filter, OpenCallFields? sort);
    public void UpdateCallEnd(int id, int idA);
    public void UpdateCallCancel(int id, int idA);
    public void ChoiceCall(int id, int idC);
}
