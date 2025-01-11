using DalApi;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using BO;
using Newtonsoft.Json;
using System.Globalization;

namespace Helpers;

internal static class Tools
{
    public static IDal _dal = Factory.Get;

    public static int totalTreated(int id)
    {
        var doAssignment = _dal.Assignment.ReadAll();
        var volunteerAssignments = doAssignment.Where(a => a.VolunteerId == id);

        return volunteerAssignments.Count(a => a.typeOfEnd == DO.typeOfEndTreatment.treated);
    }

    public static int totalSelfCancelled(int id)
    {
        var doAssignment = _dal.Assignment.ReadAll();
        var volunteerAssignments = doAssignment.Where(a => a.VolunteerId == id);

        return volunteerAssignments.Count(a => a.typeOfEnd == DO.typeOfEndTreatment.selfCancellation);
    }

    public static int totalExpired(int id)
    {
        var doAssignment = _dal.Assignment.ReadAll();
        var volunteerAssignments = doAssignment.Where(a => a.VolunteerId == id);

        return volunteerAssignments.Count(a => a.typeOfEnd == DO.typeOfEndTreatment.Expired);
    }


    public static BO.Volunteer parseDoToBoVolunteer(DO.Volunteer volunteer)
    {
        return new BO.Volunteer
        {
            Id = volunteer.Id,
            Name = volunteer.Name,
            Phone = volunteer.Phone,
            Mail = volunteer.Email,
            Password = volunteer.Password,
            Address = volunteer.Address,
            Latitude = volunteer.Latitude,
            Longitude = volunteer.Longitude,
            Job = (BO.jobType)volunteer.JobType,
            IsActive = volunteer.isActive,
            MaxDistance = volunteer.MaxDistance,
            DistanceType = (BO.distanceType)volunteer.distanceType,
            Totaltreated = totalTreated(volunteer.Id),
            TotalSelfCancellation = totalSelfCancelled(volunteer.Id),
            TotalExpired = totalExpired(volunteer.Id)
        };
    }

    public static bool IsValidEmail(string email)
    {
        // pattern of a valid email
        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        //use of Regex.IsMatch to check for a match between the email and the pattern
        return Regex.IsMatch(email, pattern);
    }

    public static bool IsValidId(int id)
    {
        // pattern of a valid id
        string pattern = @"^[0-9]{9}$";
        //use of Regex.IsMatch to check for a match between the id and the pattern
        return Regex.IsMatch(id.ToString(), pattern);
    }
    public static bool IsValidPhone(string phone)
    {
        // pattern of a valid phone
        string pattern = @"^0[2-9]\d{7,8}$";
        //use of Regex.IsMatch to check for a match between the phone and the pattern
        return Regex.IsMatch(phone, pattern);
    }

    public static bool IsValidNumber(double? number)
    {
        // pattern of a valid number
        string pattern = @"^[0-9]+$";
        //use of Regex.IsMatch to check for a match between the number and the pattern
        return Regex.IsMatch(number.ToString()!, pattern);
    }

    public static bool isDirector(int id)
    {
        var volunteer = _dal.Volunteer.Read(id);
        return volunteer!.JobType == DO.jobType.Director;
    }

    public static void ValidateFieldsFormat(BO.Volunteer volunteer)
    {
        if (!IsValidId(volunteer.Id)) { throw new BlInvalidInputException("Invalid ID"); }
        if (!IsValidPhone(volunteer.Phone)) { throw new BlInvalidInputException("Invalid Phone"); }
        if (!IsValidEmail(volunteer.Mail)) { throw new BlInvalidInputException("Invalid Email"); }
        if (!IsValidNumber(volunteer.MaxDistance)) { throw new BlInvalidInputException("Invalid Max Distance"); }
    }




    //used for https://cane.marchejamais.com
    //private static string ApiKey = "6761782af11a3702282865asq982b03"; // Remplacez par votre clé d'API
    //used for https://geocode.maps.co/
    private static string ApiKey = "6761782af11a3702282865asq982b03";

    //public async Task<(double? Latitude, double? Longitude)> GetCoordinatesAsync(string address)
    //{
    //    if (string.IsNullOrEmpty(address))
    //        throw new ArgumentException("L'adresse ne peut pas être vide.");

    //    // URL pour appeler l'API Maps.co ou Google Maps
    //    string url = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(address)}&api_key={ApiKey}";

    //    using (HttpClient client = new HttpClient())
    //    {
    //        try
    //        {
    //            // Envoi de la requête GET
    //            HttpResponseMessage response = await client.GetAsync(url);
    //            response.EnsureSuccessStatusCode();

    //            string jsonResponse = await response.Content.ReadAsStringAsync();

    //            // Parse le JSON avec Newtonsoft.Json
    //            JArray results = JArray.Parse(jsonResponse);

    //            // Vérifie si l'API a renvoyé des résultats
    //            if (results.Count > 0)
    //            {
    //                var result = results[0]; // Premier résultat retourné
    //                double latitude = double.Parse(result["lat"]?.ToString()!);
    //                double longitude = double.Parse(result["lon"]?.ToString()!);

    //                return (latitude, longitude);
    //            }

    //            // Retourne null si aucune donnée n'est trouvée
    //            return (null, null);
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine($"Erreur lors de la récupération des coordonnées : {ex.Message}");
    //            return (null, null);
    //        }
    //    }
    //}
    /*
  
    public static async Task<(double Latitude, double Longitude)> GetCoordinatesAsync(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentException("The address cannot be null or empty.", nameof(address));
        }

        string url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={GoogleApiKey}";

        var response = await HttpClient.GetStringAsync(url);
        var geocodeResponse = JsonSerializer.Deserialize<GoogleGeocodeResponse>(response);

        if (geocodeResponse == null  geocodeResponse.Status != "OK"  geocodeResponse.Results.Length == 0)
        {
            throw new Exception($"Address not found or invalid response from Google Maps API: {geocodeResponse?.Status}");
        }

        var location = geocodeResponse.Results[0].Geometry.Location;
        return (location.Lat, location.Lng);
    }
}
     
     
     
     */

    public static async Task<(double? Latitude, double? Longitude)> GetCoordinatesAsync(string address)
    {

        NumberFormatInfo provider = new NumberFormatInfo();
        provider.NumberDecimalSeparator = ".";

       string requestUrl = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(address)}&api_key={ApiKey}";

        if (string.IsNullOrEmpty(address))
            throw new ArgumentException("L'adresse ne peut pas être vide.");

   
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(requestUrl);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    
                    // Parse the JSON if not empty
                    if (!string.IsNullOrWhiteSpace(jsonResponse))
                    {
                        JArray jsonArray = JArray.Parse(jsonResponse);

                        if (jsonArray.Count > 0)
                        {
                            // Extract the first result's latitude and longitude
                            var firstResult = jsonArray[0];
                            double latitude = Convert.ToDouble(firstResult["lat"]!.ToString(), provider);
                            double longitude = Convert.ToDouble(firstResult["lon"]!.ToString(), provider);

                            return new (latitude, longitude); // Pas de résultats
                        }
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Erreur HTTP : {httpEx.Message}");
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"Erreur de parsing JSON : {jsonEx.Message}");
            }
            catch (InvalidOperationException iex)
            {
                Console.WriteLine($"Erreur de Latitude/Longitude {iex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur inattendue : {ex.Message}");
            }

            return (null, null);
        }
    }




    public static async Task<string> GetAddressAsync(double? latitude, double? longitude)
    {
        string url = $"https://geocode.maps.co/reverse?lat={latitude}&lon={longitude}&api_key={ApiKey}";

        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                JObject result = JObject.Parse(jsonResponse);

                string? address = result["display_name"]?.ToString();

                return address ?? "Adresse introuvable";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération de l'adresse : {ex.Message}");
                return "Erreur lors de la récupération de l'adresse";
            }
        }
    }


    public static void ValidateCallFieldsFormat(BO.Call call)
    {


        // Vérification des coordonnées géographiques
        if (call.Latitude != null && (call.Latitude < -90 || call.Latitude > 90))
            throw new BlInvalidInputException("The Latitude must be between -90 and 90 degree");

        if (call.Longitude != null && (call.Longitude < -180 || call.Longitude > 180))
            throw new BlInvalidInputException("The Longitude must be between -180 and 180 degree");

        // Vérification des dates
        if (call.Created > ClockManager.Now)
            throw new BlInvalidInputException("The creation date cannot be in the futur");

        if (call.MaxEndTreatment != null && call.MaxEndTreatment <= call.Created)
            throw new BlInvalidInputException("The maximal end of treatment's date must be greater than the creation's date");

        // Vérification du type d'appel
        if (!Enum.IsDefined(typeof(BO.callType), call.CallType))
            throw new BlInvalidInputException("Type of Call Invalid");

        //// verification de l'adresse
        //GetAddressAsync(call.Latitude, call.Longitude).Wait();


    }

    public static Status DetermineCallStatus(DateTime created, DateTime? maxEndTreatment)
    {
        var now = ClockManager.Now;

        // Vérifie si maxEndTreatment est défini et compare avec la date actuelle
        if (maxEndTreatment.HasValue)
        {
            if (now > maxEndTreatment.Value)
            {
                return Status.Expired; // Si la date actuelle dépasse maxEndTreatment
            }

            // Vérifie si la date est proche d'expirer (par exemple, dans les 2 heures)
            var timeUntilExpiration = maxEndTreatment.Value - now;
            if (timeUntilExpiration.TotalHours <= 2)
            {
                return Status.AlmostExpired;
            }
        }

        // Si la date actuelle est proche de la création mais pas encore expirée
        var elapsedTimeSinceCreation = now - created;

        if (elapsedTimeSinceCreation.TotalHours <= 1)
        {
            return Status.Open; // Statut ouvert pour les appels récents
        }

        // Appel en cours si aucune autre condition ne s'applique
        return Status.InProgress;
    }

    public static List<CallAssignInList> CreateCallAssignListFromDo(DO.Call call, IEnumerable<DO.Assignment> assignments, IEnumerable<DO.Volunteer> volunteers)
    {
        // Filtrer les assignations qui correspondent à l'appel
        var callAssignments = assignments.Where(assign => assign.CallId == call.Id);

        // Mapper les assignations vers des CallAssignInList
        return callAssignments.Select(assign =>
        {
            var volunteer = volunteers.FirstOrDefault(v => v.Id == assign.VolunteerId);

            return new CallAssignInList
            {
                volounteerId = assign.VolunteerId,
                volounteerName = volunteer?.Name ?? "Unknown",
                startingTime = assign.StartTreatment,
                endingTime = assign.endTreatment,
                TypeOfEndTreatment = assign.typeOfEnd.HasValue
                    ? (BO.typeOfEndTreatment?)assign.typeOfEnd.Value
                    : null
            };
        }).ToList();
    }


    public static BO.Call parseDoToBoCall(DO.Call call)
    {
        return new BO.Call
        {
            Id = call.Id,
            CallType = (BO.callType)call.CallType,
            Description = call.Description,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            Created = call.CallTime,
            MaxEndTreatment = call.MaxTime,
            Status = DetermineCallStatus(call.CallTime, call.MaxTime),
            callAssignInLists = CreateCallAssignListFromDo(call, _dal.Assignment.ReadAll(), _dal.Volunteer.ReadAll())
        };
    }

    public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Rayon de la Terre en km
        double dLat = DegreesToRadians(lat2 - lat1);
        double dLon = DegreesToRadians(lon2 - lon1);
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c; // Distance en km
    }

    public static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
    public static Treatment DetermineTreatment(DO.Call call)
    {
        if (call.MaxTime == null)
            return BO.Treatment.Intreatment;
        else if (call.MaxTime - ClockManager.Now < _dal.Config.RiskRange)
            return BO.Treatment.Inrisktreatment;
        else
            return BO.Treatment.Intreatment;



    }
    public static CallInProgress GetCallInProgress(int id)
    {
        var call = _dal.Call.ReadAll();
        var callAssign = _dal.Assignment.ReadAll();
        var volunteer = _dal.Volunteer.Read(id);
        if (volunteer == null)
            throw new BlNotFoundException("Volunteer not found");


        var progress = (from c in call
                        join a in callAssign on c.Id equals a.CallId
                        where a.VolunteerId == id && a.endTreatment == null
                        select new CallInProgress
                        {
                            Id = a.Id,
                            CallId = c.Id,
                            CallType = (BO.callType)c.CallType,
                            Description = c.Description,
                            Address = c.Address,
                            Created = c.CallTime,
                            MaxEndTreatment = c.MaxTime,
                            StartTreatment = a.StartTreatment,
                            Distance = (volunteer.Latitude.HasValue && volunteer.Longitude.HasValue)
                            ? CalculateDistance(c.Latitude, c.Longitude, volunteer.Latitude.Value, volunteer.Longitude.Value)
                            : 0,
                            Treatment = DetermineTreatment(c)
                        }).FirstOrDefault();

        return progress;


    }
    public class GeocodeResult
    {
        public string Lat { get; set; }
        public string Lon { get; set; }
    }

}


