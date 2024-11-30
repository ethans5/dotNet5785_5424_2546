namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;

internal class CallImplementation : ICall
{
    // Create a new Call and save it to the XML file
    public void Create(Call item)
    {
        int id = Config.NextCallId; // Generate a new unique ID
        Call temp = item with { Id = id }; // Assign the new ID to the call
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        calls.Add(temp); // Add the new call to the list
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml); // Save the updated list
    }

    // Delete a Call by ID from the XML file
    public void Delete(int id)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (calls.RemoveAll(c => c.Id == id) == 0) // Remove the call and check if it was found
            throw new InvalidOperationException($"Call with ID {id} not found");
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml); // Save the updated list
    }

    // Delete all Calls from the XML file
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Call>(), Config.s_calls_xml); // Save an empty list
    }

    // Read a Call by ID from the XML file
    public Call? Read(int id)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return calls.Find(c => c.Id == id); // Find and return the call by ID
    }

    // Read a Call using a filter from the XML file
    public Call? Read(Func<Call, bool> filter)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return calls.FirstOrDefault(filter); // Find and return the first matching call
    }

    // Read all Calls or filtered Calls from the XML file
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return filter == null ? calls : calls.Where(filter); // Apply filter if provided
    }

    // Update an existing Call in the XML file
    public void Update(Call item)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (calls.RemoveAll(c => c.Id == item.Id) == 0) // Remove the existing call by ID
            throw new InvalidOperationException($"Call with ID {item.Id} not found");
        calls.Add(item); // Add the updated call
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml); // Save the updated list
    }
}
