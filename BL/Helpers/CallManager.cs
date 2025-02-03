using BlImplementation;
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
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

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
                    lock (AdminManager.BlMutex)
                    {
                        _dal.Call.Update(parseBoToDoCall(boCall));
                    }
                }
            }
        }
    }
    private static readonly Random s_rand = new(); // Générateur de nombres aléatoires
    private static int s_simulatorCounter = 0; // Compteur du simulateur

    internal static void SimulateCallHandling()
    {
        Thread.CurrentThread.Name = $"CallSimulator{++s_simulatorCounter}";

        LinkedList<int> callsToUpdate = new();
        List<DO.Call> doCallList;

        lock (AdminManager.BlMutex)
            doCallList = _dal.Call.ReadAll().ToList(); // On récupère tous les appels

        foreach (var doCall in doCallList)
        {
            int callId = 0;
            lock (AdminManager.BlMutex)
            {
                // Convertir DO.Call en BO.Call
                BO.Call boCall = parseDoToBoCall(doCall);

                // Vérifier si l'appel est en attente d'un volontaire
                if (boCall.Status == Status.Open || boCall.Status == Status.OpenAlmostExpired)
                {
                    var availableVolunteers = VolunteerManager.GetAvailableVolunteers();
                    int cntVolunteers = availableVolunteers.Count();

                    if (cntVolunteers != 0)
                    {
                        // Sélection aléatoire d’un volontaire
                        int volunteerId = availableVolunteers.Skip(s_rand.Next(0, cntVolunteers)).First()!.Id;

                        // Assigner l'appel au volontaire et mettre à jour le statut
                        s_bl.Call.ChoiceCall(callId, volunteerId);
                        boCall.Status = Status.InProgress ;
                        callId = boCall.Id;
                    }
                }

                // Simuler la clôture de l’appel avec une évaluation aléatoire
                if (boCall.Status == Status.InProgress)
                {
                    bool closeCall = s_rand.NextDouble() > 0.5; // 50% de chances de clôturer l’appel
                    if (closeCall)
                    {
                        boCall.Status = Status.Closed;
                        callId = boCall.Id;
                    }
                }

                // Convertir BO.Call en DO.Call et mettre à jour la base
                DO.Call updatedDoCall = CallManager.parseBoToDoCall(boCall);
                _dal.Call.Update(updatedDoCall);

                if (callId != 0)
                    callsToUpdate.AddLast(callId);
            }
        }

        // Notifier les observateurs des mises à jour
        foreach (int id in callsToUpdate)
            Observers.NotifyItemUpdated(id);
    }

}
