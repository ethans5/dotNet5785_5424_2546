
namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;

internal class CallImplementation : ICall
{
    public void Create(Call item)
    {
        int id = Config.NextCallId;
        Call temp = item with { Id = id };
        List<Call> Call = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        Call.Add(temp);
        XMLTools.SaveListToXMLSerializer(Call, Config.s_calls_xml);
    }

    public void Delete(int id)
    {
        List<Call> Call = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (Call.RemoveAll(c => c.Id == id) == 0)
            throw new InvalidOperationException($"Call with ID {id} not found");
        XMLTools.SaveListToXMLSerializer(Call, Config.s_calls_xml);
    }

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Call>(), Config.s_calls_xml);
    }

    public Call? Read(int id)
    {
        List<Call> Call = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return Call.Find(c => c.Id == id);
    }

    public Call? Read(Func<Call, bool> filter)
    {
        List<Call> Call = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return Call.FirstOrDefault(filter);
    }

    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        List<Call> Call = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (filter == null)
            return Call.Select(item => item);
        else
            return Call.Where(filter);
    }

    public void Update(Call item)
    {
     List<Call> Call=XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if(Call.RemoveAll(c => c.Id == item.Id) == 0)
            throw new InvalidOperationException($"Call with ID {item.Id} not found");
        Call.Add(item);
        XMLTools.SaveListToXMLSerializer(Call, Config.s_calls_xml);
    }
}
