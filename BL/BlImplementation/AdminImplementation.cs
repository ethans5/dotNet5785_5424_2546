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
        return _dal.Config.RiskRange;
    }

    public DateTime GetSystemeClock()
    {
        return ClockManager.Now;
    }

    public void InitializaData()
    {
        DalTest.Initialization.Do();
        ClockManager.UpdateClock(DateTime.Now);
    }

    public void ResetData()
    {
        _dal.Config.Reset();
    }

    public void UpdateClock(UnitTime time)
    {
        if(time == UnitTime.Minutes)
        {
            ClockManager.UpdateClock(ClockManager.Now.AddMinutes(1));
        }
        else if(time == UnitTime.Hours)
        {
            ClockManager.UpdateClock(ClockManager.Now.AddHours(1));
        }
        else if (time == UnitTime.Days)
        {
            ClockManager.UpdateClock(ClockManager.Now.AddDays(1));
        }
        else if (time == UnitTime.Months)
        {
            ClockManager.UpdateClock(ClockManager.Now.AddMonths(1));
        }
        else if (time == UnitTime.Years)
        {
            ClockManager.UpdateClock(ClockManager.Now.AddYears(1));
        }
    }

    public void UpdateRiskRange(TimeSpan range)
    {
        _dal.Config.RiskRange = range;
    }
}
