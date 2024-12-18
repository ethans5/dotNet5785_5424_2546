using DalApi;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using BO;

namespace Helpers;

internal class Tools
{
    public IDal _dal = Factory.Get;

    public int totalTreated(int id)
    {
        var doAssignment = _dal.Assignment.ReadAll();
        var volunteerAssignments = doAssignment.Where(a => a.VolunteerId == id);

        return volunteerAssignments.Count(a => a.typeOfEnd == DO.typeOfEndTreatment.treated);
    }

    public int totalSelfCancelled(int id)
    {
        var doAssignment = _dal.Assignment.ReadAll();
        var volunteerAssignments = doAssignment.Where(a => a.VolunteerId == id);

        return volunteerAssignments.Count(a => a.typeOfEnd == DO.typeOfEndTreatment.selfCancellation);
    }

    public int totalExpired(int id)
    {
        var doAssignment = _dal.Assignment.ReadAll();
        var volunteerAssignments = doAssignment.Where(a => a.VolunteerId == id);

        return volunteerAssignments.Count(a => a.typeOfEnd == DO.typeOfEndTreatment.Expired);
    }


    public BO.Volunteer parseDoToBo(DO.Volunteer volunteer)
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

    public void ValidateFieldsFormat(BO.Volunteer volunteer)
    {
        if (!IsValidId(volunteer.Id)) { throw new BlInvalidInputException("Invalid ID"); }
        if (!IsValidPhone(volunteer.Phone)) { throw new BlInvalidInputException("Invalid Phone"); }
        if (!IsValidEmail(volunteer.Mail)) { throw new BlInvalidInputException("Invalid Email"); }
        if(!IsValidNumber(volunteer.MaxDistance)) { throw new BlInvalidInputException("Invalid Max Distance"); }
    }



       
    private const string ApiKey = "6761782af11a3702282865asq982b03"; // Remplacez par votre clé d'API

    public async Task<(double? Latitude, double? Longitude)> GetCoordinatesAsync(string address)
    {
        if (string.IsNullOrEmpty(address))
            throw new ArgumentException("L'adresse ne peut pas être vide.");

        // URL pour appeler l'API Maps.co ou Google Maps
        string url = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(address)}&api_key={ApiKey}";

        using (HttpClient client = new HttpClient())
        {
            try
            {
                // Envoi de la requête GET
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();

                // Parse le JSON avec Newtonsoft.Json
                JArray results = JArray.Parse(jsonResponse);

                // Vérifie si l'API a renvoyé des résultats
                if (results.Count > 0)
                {
                    var result = results[0]; // Premier résultat retourné
                    double latitude = double.Parse(result["lat"]?.ToString()!);
                    double longitude = double.Parse(result["lon"]?.ToString()!);

                    return (latitude, longitude);
                }

                // Retourne null si aucune donnée n'est trouvée
                return (null, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des coordonnées : {ex.Message}");
                return (null, null);
            }
        }
    }



    public void ValidateCallFieldsFormat(BO.Call call)
    {
        // Vérification de l'ID
        if (call.Id <= 0)
            throw new BlInvalidInputException("The ID must be geater than 0 and not null");

        // Vérification des coordonnées géographiques
        if (call.Latitude != null && (call.Latitude < -90 || call.Latitude > 90))
            throw new BlInvalidInputException("The Latitude must be between -90 and 90 degree");

        if (call.Longitude != null && (call.Longitude < -180 || call.Longitude > 180))
            throw new BlInvalidInputException("The Longitude must be between -180 and 180 degree");

        // Vérification des dates
        if (call.Created > DateTime.Now)
            throw new BlInvalidInputException("The creation date cannot be in the futur");

        if (call.MaxEndTreatment != null && call.MaxEndTreatment <= call.Created)
            throw new BlInvalidInputException("The maximal end of treatment's date must be greater than the creation's date");

        // Vérification du type d'appel
        if (!Enum.IsDefined(typeof(BO.callType), call.CallType))
            throw new BlInvalidInputException("Type of Call Invalid");

       
    }
}


