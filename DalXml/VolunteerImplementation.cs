namespace Dal;

// Import necessary namespaces
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

internal class VolunteerImplementation : IVolunteer
{
    // Function to get a Volunteer from an XML element
    static Volunteer GetVolunteer(XElement s)
    {
        return new Volunteer()
        {
            Id = int.Parse(s.Attribute("Id")?.Value ?? throw new FormatException("missing Id")),
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

    // Function to create a Volunteer and add it to the XML
    public void Create(Volunteer item)
    {
        XElement volunteerList = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        List<XElement> volunteers = volunteerList.Elements().ToList();

        // Check if the Volunteer already exists
        if (volunteers.Any(s => s.Attribute("Id")!.Value == item.Id.ToString()))
            throw new DalAlreadyExistException($"Volunteer with the ID : {item.Id} already exists...");

        // Add the new Volunteer
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

    // Function to delete a Volunteer from the XML
    public void Delete(int id)
    {
        XElement volunteerList = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        XElement? volunteerElem = volunteerList.Elements().FirstOrDefault(s => s.Attribute("Id")!.Value == id.ToString());

        // Check if the Volunteer exists
        if (volunteerElem == null)
            throw new DalDoesNotExistException($"Volunteer with the ID : {id} does not exist...");

        // Remove the Volunteer
        volunteerElem.Remove();
        XMLTools.SaveListToXMLElement(volunteerList, Config.s_volunteers_xml);
    }

    // Function to delete all Volunteers from the XML
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLElement(new XElement("Volunteers"), Config.s_volunteers_xml);
    }

    // Function to read a Volunteer by ID from the XML
    public Volunteer? Read(int id)
    {
        XElement? volunteerElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml)?.Elements().FirstOrDefault(s => s.Attribute("Id")!.Value == id.ToString());
        return volunteerElem == null ? null : GetVolunteer(volunteerElem);
    }

    // Function to read a Volunteer by a filter from the XML
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml)?.Elements().Select(GetVolunteer).FirstOrDefault(filter);
    }

    // Function to read all Volunteers from the XML
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        XElement? volunteerList = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        IEnumerable<Volunteer> volunteers = volunteerList.Elements().Select(GetVolunteer);
        return filter != null ? volunteers.Where(filter) : volunteers;
    }

    // Function to update a Volunteer in the XML
    public void Update(Volunteer item)
    {
        XElement volunteerRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        // Check if the Volunteer exists and remove the old data
        (volunteerRootElem.Elements().FirstOrDefault(s => s.Attribute("Id")!.Value == item.Id.ToString()) ??
            throw new DalDoesNotExistException($"Volunteer with the ID : {item.Id} does not exist...")).Remove();

        // Add the updated Volunteer data
        volunteerRootElem.Add(new XElement("Volunteer",
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
        XMLTools.SaveListToXMLElement(volunteerRootElem, Config.s_volunteers_xml);
    }
}
