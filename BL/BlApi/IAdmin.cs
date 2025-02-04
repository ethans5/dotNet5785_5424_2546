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
    void AddConfigObserver(Action configObserver);
    void RemoveConfigObserver(Action configObserver);
    void AddClockObserver(Action clockObserver);
    void RemoveClockObserver(Action clockObserver);
    void StartSimulator(int interval);
    void StopSimulator();


}
