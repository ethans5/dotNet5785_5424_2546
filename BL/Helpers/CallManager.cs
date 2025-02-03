using BO;
using DalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers;

internal static class CallManager
{
    private static IDal _dal = Factory.Get; //stage 4
    internal static ObserverManager Observers = new();

    public static BO.Call parseDoToBoCall(DO.Call call)
    {
        return new BO.Call
        {
            Id = call.Id,
            CallType = (BO.callType)call.CallType,
            Description = call.Description,
            Address=call.Address,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            Created = call.CallTime,
            MaxEndTreatment = call.MaxTime,
            Status = Tools.DetermineCallStatus(call.Id, call.CallTime, call.MaxTime),
            callAssignInLists = Tools.CreateCallAssignListFromDo(call, _dal.Assignment.ReadAll(), _dal.Volunteer.ReadAll())
        };
    }

    public static DO.Call parseBoToDoCall(BO.Call call)
    {
        return new DO.Call
        {
            Id = call.Id,
            CallType = (DO.callType)call.CallType,
            Description = call.Description,
            Address=call.Address!,
            Latitude = call.Latitude ?? 0,
            Longitude = call.Longitude ?? 0,
            CallTime = call.Created,
            MaxTime = call.MaxEndTreatment
        };
    }

    //public static void CheckCallStatuses(DateTime currentSystemTime)
    //{
    //    var _calls = _dal.Call.ReadAll();

    //    foreach (var doCall in _calls)
    //    {
    //        var boCall = parseDoToBoCall(doCall);

    //        if (boCall.Status == Status.Open || boCall.Status == Status.InProgress ||
    //            boCall.Status == Status.OpenAlmostExpired || boCall.Status==Status.AlmostExpired)
    //        {
    //            if (currentSystemTime >= boCall.MaxEndTreatment)
    //            {
    //                boCall.Status = Status.Expired;
    //            }
    //            else if ((currentSystemTime - _dal.Config.RiskRange) < boCall.MaxEndTreatment && boCall.Status == Status.Open)
    //            {
    //                boCall.Status = Status.OpenAlmostExpired;
    //            }
    //            else if ((currentSystemTime - _dal.Config.RiskRange) < boCall.MaxEndTreatment && boCall.Status == Status.InProgress)
    //            {
    //                boCall.Status = Status.AlmostExpired;
    //            }
    //        }
    //    }
    //}

    public static void CheckCallStatuses(DateTime oldClock, DateTime newClock)
    {
        lock (AdminManager.BlMutex) // Protection contre les accès concurrents
        {
            var calls = _dal.Call.ReadAll().ToList(); // Charger toutes les données une seule fois

            foreach (var doCall in calls)
            {
                var boCall = parseDoToBoCall(doCall);

                if (boCall.Status == Status.Open || boCall.Status == Status.InProgress ||
                    boCall.Status == Status.OpenAlmostExpired || boCall.Status == Status.AlmostExpired)
                {
                    if (newClock >= boCall.MaxEndTreatment)
                    {
                        boCall.Status = Status.Expired;
                    }
                    else if ((newClock - _dal.Config.RiskRange) < boCall.MaxEndTreatment && boCall.Status == Status.Open)
                    {
                        boCall.Status = Status.OpenAlmostExpired;
                    }
                    else if ((newClock - _dal.Config.RiskRange) < boCall.MaxEndTreatment && boCall.Status == Status.InProgress)
                    {
                        boCall.Status = Status.AlmostExpired;
                    }

                    // Mise à jour de l'appel en base de données
                    _dal.Call.Update(parseBoToDoCall(boCall));
                }
            }
        }
    }

}
