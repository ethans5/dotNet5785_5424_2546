using DalApi;

namespace Helpers;

internal static class VolunteerManager
{
    private static IDal _dal = Factory.Get; //stage 4
    internal static ObserverManager Observers = new();


}
