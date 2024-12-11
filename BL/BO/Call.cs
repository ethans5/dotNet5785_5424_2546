
namespace BO
{
    public class Call
    {
        public int Id { get; init; }
        public callType CallType { get; set; }
        public string? Description { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime Created { get; set; }
        public DateTime? MaxEndTreatment { get; set; }
        public Status Status { get; set; }
        public List <BO.CallAssignmentInList> callAssignInLists { get; set; }

    }
}
