using BlApi;
using BO;
using DalApi;
using Helpers;

namespace BlImplementation;

internal class AdminImplementation : IAdmin
{
    private DalApi.IDal _dal = Factory.Get;   
    public TimeSpan GetRiskRange()
    {
        return AdminManager.RiskRange;
    }

    public DateTime GetSystemeClock()
    {
        return AdminManager.Now;
    }

    public void InitializaData()
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        lock (AdminManager.BlMutex)
        {
            DalTest.Initialization.Do();
            AdminManager.UpdateClock(DateTime.Now);
            AdminManager.RiskRange = AdminManager.RiskRange;
        }
      

    }

    public void ResetData()
    {
        
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        lock (AdminManager.BlMutex)
        {
            _dal.ResetDB();
            AdminManager.RiskRange = AdminManager.RiskRange;
        }
    }

    public void UpdateClock(UnitTime time)
    {
        lock (AdminManager.BlMutex)
        {
            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
            if (time == UnitTime.Minutes)
            {
                AdminManager.UpdateClock(AdminManager.Now.AddMinutes(1));
            }
            else if (time == UnitTime.Hours)
            {
                AdminManager.UpdateClock(AdminManager.Now.AddHours(1));
            }
            else if (time == UnitTime.Days)
            {
                AdminManager.UpdateClock(AdminManager.Now.AddDays(1));
            }
            else if (time == UnitTime.Months)
            {
                AdminManager.UpdateClock(AdminManager.Now.AddMonths(1));
            }
            else if (time == UnitTime.Years)
            {
                AdminManager.UpdateClock(AdminManager.Now.AddYears(1));
            }
        }
    }

    public void UpdateRiskRange(TimeSpan range)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        lock (AdminManager.BlMutex)
        {
            AdminManager.RiskRange = range;
        }
    }
    
    public void AddClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers += clockObserver;
    public void RemoveClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers -= clockObserver;
    public void AddConfigObserver(Action configObserver) =>
   AdminManager.ConfigUpdatedObservers += configObserver;
    public void RemoveConfigObserver(Action configObserver) =>
    AdminManager.ConfigUpdatedObservers -= configObserver;

    public void StartSimulator(int interval)
    {
      /*  AdminManager.ThrowOnSimulatorIsRunning();  *///stage 7
        AdminManager.Start(interval);
    }
    public void StopSimulator()=>AdminManager.Stop();
}
