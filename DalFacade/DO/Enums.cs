namespace DO;
public enum jobType
{
    Volunteer,
    Director
}
public enum distanceType
{
    aerial = 1,
    walking,
    driving
}   
public enum callType
{
    BuyingFood = 1,
    BuyingMedicine,
    BuyingClothes,
    BuyingCartoons,
    PackingFood,
    PackingMedicine,
    PackingClothes,
    PackingCartoonsInTheTruck,
    Delivries,
    DelivriesToTheDoor,
}

public enum typeOfEndTreatment
{
    treated = 1,
    selfCancellation,
    directorCancellation,
    Expired
}