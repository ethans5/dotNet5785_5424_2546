namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;

internal class AssignmentImplementation : IAssignment
{
    // Create a new Assignment and save it to the XML file
    public void Create(Assignment item)
    {
        int id = Config.NextAssignmentId; // Generate a new unique ID
        Assignment copy = item with { Id = id }; // Assign the new ID to the assignment
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        assignments.Add(copy); // Add the new assignment to the list
        XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignments_xml); // Save the updated list
    }

    // Delete an Assignment by ID from the XML file
    public void Delete(int id)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (assignments.RemoveAll(a => a.Id == id) == 0) // Remove the assignment and check if it was found
            throw new InvalidOperationException($"Assignment with ID {id} not found");
        XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignments_xml); // Save the updated list
    }

    // Delete all Assignments from the XML file
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Assignment>(), Config.s_assignments_xml); // Save an empty list
    }

    // Read an Assignment by ID from the XML file
    public Assignment? Read(int id)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        return assignments.Find(a => a.Id == id); // Find and return the assignment by ID
    }

    // Read an Assignment using a filter from the XML file
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        return assignments.FirstOrDefault(filter); // Find and return the first matching assignment
    }

    // Read all Assignments or filtered Assignments from the XML file
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        return filter == null ? assignments : assignments.Where(filter); // Apply filter if provided
    }

    // Update an existing Assignment in the XML file
    public void Update(Assignment item)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (assignments.RemoveAll(a => a.Id == item.Id) == 0) // Remove the existing assignment by ID
            throw new InvalidOperationException($"Assignment with ID {item.Id} not found");
        assignments.Add(item); // Add the updated assignment
        XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignments_xml); // Save the updated list
    }
}
