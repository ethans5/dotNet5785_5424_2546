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
        return AdminManager.Now;
    }

    public void InitializaData()
    {
        DalTest.Initialization.Do();
        AdminManager.UpdateClock(DateTime.Now);
    }

    public void ResetData()
    {
        
        _dal.ResetDB();
    }

    public void UpdateClock(UnitTime time)
    {
        if(time == UnitTime.Minutes)
        {
            AdminManager.UpdateClock(AdminManager.Now.AddMinutes(1));
        }
        else if(time == UnitTime.Hours)
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

    public void UpdateRiskRange(TimeSpan range)
    {
        _dal.Config.RiskRange = range;
    }
}
