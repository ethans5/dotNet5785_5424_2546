namespace BO;

public enum jobType
{
    Volunteer,
    Director
}
public enum distanceType
{
    aerial ,
    walking,
    driving
}
public enum callType
{
    BuyingFood ,
    BuyingMedicine,
    BuyingClothes,
    BuyingCartoons,
    PackingFood,
    PackingMedicine,
    PackingClothes,
    PackingCartoonsInTheTruck,
    Deliveries,
    DelivriesToTheDoor
}
public enum Treatment 
{
    Intreatment,
    Inrisktreatment
}
public enum typeOfEndTreatment
{
    treated = 1,
    selfCancellation,
    directorCancellation,
    Expired
}


public enum Status
{
    Open,
    InProgress,
    Closed,
    Expired,
    AlmostExpired

}
public enum VolunteerSortField
{
    Name,
    Totaltreated,
    TotalSelfCancellation,
    TotalExpired,
    CallType
}

public enum CallFields
{
    Id,
    CallId,
    callType,
    startingTime,
    remainingTime,
    LastVolunteerName,
    duration,
    Status,
    totalAssignmentAllocation
}

public enum closedCallFields
{
    Id,
    callType,
    adress,
    createdTime,
    startTreatment,
    endTreatment,
    typeOfEndTreatment
}
public enum OpenCallFields
{
    Id,
    callType,
    description,
    Address,
    Created,
    MaxEndTreatment,
    Distance
}
public enum UnitTime
{
    Minutes,
    Hours,
    Days,
    Months,
    Years
}