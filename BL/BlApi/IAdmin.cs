using BO;

namespace BlApi;

public interface IAdmin
{
    public DateTime GetSystemeClock();
    public void UpdateClock(UnitTime time);
    public TimeSpan GetRiskRange();
    public void UpdateRiskRange(TimeSpan range);
    public void ResetData();
    public void InitializaData();
}
