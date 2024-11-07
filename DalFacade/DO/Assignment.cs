namespace DO;

public record Assignment(
    int Id,
    int CallId,
    int VolunteerId,
    DateTime StartTreatment,
    DateTime? endTreatment=null,
    typeOfEndTreatment? typeOfEnd = null
    )
{
    public Assignment() : this(0, 0, 0, DateTime.Now) { }
}

