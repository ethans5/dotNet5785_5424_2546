
namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

internal class VolunteerImplementation : IVolunteer
{
    static Volunteer GetVolunteer(XElement s)
    {
        return new Volunteer()
        {
            Id = s.ToIntNullable("Id") ?? throw new FormatException("missing Id"),
            Name = s.Element("Name")?.Value ?? throw new FormatException("missing Name"),
            Phone = s.Element("Phone")?.Value ?? throw new FormatException("missing Phone"),
            Email = s.Element("Email")?.Value ?? throw new FormatException("missing Email"),
            JobType = s.ToEnumNullable<jobType>("JobType") ?? throw new FormatException("missing JobType"),
            isActive = (bool?)s.Element("isActive") ?? throw new FormatException("missing isActive"),
            distanceType = s.ToEnumNullable<distanceType>("DistanceType") ?? throw new FormatException("missing DistanceType"),
            MaxDistance = s.ToDoubleNullable("MaxDistance"),
            Password = s.Element("Password")?.Value,
            Address = s.Element("Address")?.Value,
            Latitude = s.ToDoubleNullable("Latitude"),
            Longitude = s.ToDoubleNullable("Longitude")
        };
    }
    
    public void Create(Volunteer item)
    {

        XElement volunteerList = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        List<XElement> volunteers = volunteerList.Elements().ToList();
        if (volunteers.Any(s => s.ToIntNullable("Id") == item.Id))
            throw new DalAlreadyExistException($"Volunteer with the ID : {item.Id} already exists...");
        volunteerList.Add(new XElement("Volunteer",
            new XAttribute("Id", item.Id),
            new XElement("Name", item.Name),
            new XElement("Phone", item.Phone),
            new XElement("Email", item.Email),
            new XElement("JobType", item.JobType),
            new XElement("isActive", item.isActive),
            new XElement("DistanceType", item.distanceType),
            new XElement("MaxDistance", item.MaxDistance),
            new XElement("Password", item.Password),
            new XElement("Address", item.Address),
            new XElement("Latitude", item.Latitude),
            new XElement("Longitude", item.Longitude)
            ));
        XMLTools.SaveListToXMLElement(volunteerList, Config.s_volunteers_xml);

    }       

    public void Delete(int id)
    {
        XElement volunteerList = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        XElement? volunteerElem = volunteerList.Elements().FirstOrDefault(s => s.ToIntNullable("Id") == id);
        if (volunteerElem == null)
            throw new DalDoesNotExistException($"Volunteer with the ID : {id} does not exist...");
        volunteerElem.Remove();
        XMLTools.SaveListToXMLElement(volunteerList, Config.s_volunteers_xml);

    }


    public void DeleteAll()
    {
        XMLTools.SaveListToXMLElement(new XElement("Volunteers"), Config.s_volunteers_xml);
    }

    public Volunteer? Read(int id)
    {
        XElement? voluteerElem= XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml)?.Elements().FirstOrDefault(s => s.ToIntNullable("Id") == id);
        return voluteerElem == null ? null : GetVolunteer(voluteerElem);
    }

    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml)?.Elements().Select(GetVolunteer).FirstOrDefault(filter);
    }

    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        XElement? volunteerList = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        IEnumerable<Volunteer> volunteers = volunteerList.Elements().Select(GetVolunteer);
        return filter!=null?volunteers.Where(filter):volunteers;

    }

    public void Update(Volunteer item)
    {
        XElement volunteerRootElem= XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        (volunteerRootElem.Elements().FirstOrDefault(s => s.ToIntNullable("Id") == item.Id) ?? 
            throw new DalDoesNotExistException($"Volunteer with the ID : {item.Id} does not exist...")).Remove();
        volunteerRootElem.Add(new XElement("Volunteer",
            new XElement("Id", item.Id),
            new XElement("Name", item.Name),
            new XElement("Phone", item.Phone),
            new XElement("Email", item.Email),
            new XElement("JobType", item.JobType),
            new XElement("isActive", item.isActive),
            new XElement("DistanceType", item.distanceType),
            new XElement("MaxDistance", item.MaxDistance),
            new XElement("Password", item.Password),
            new XElement("Address", item.Address),
            new XElement("Latitude", item.Latitude),
            new XElement("Longitude", item.Longitude)
        ));
        XMLTools.SaveListToXMLElement(volunteerRootElem, Config.s_volunteers_xml);

    }
}
