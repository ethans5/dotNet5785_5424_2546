
namespace BO
{
    internal class Call
    {
        public int Id { get; init; }
        public callType CallType { get; set; }
        public string? Description { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime Created { get; set; }
        public DateTime? MaxEndTreatment { get; set; }

    }
}
