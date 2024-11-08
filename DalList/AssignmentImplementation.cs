namespace Dal;
using DO;
using DalApi;

public class AssignmentImplementation : IAssignment
{
    public void Create(Assignment assignment)
    {
        if (DataSource.Assignments.FirstOrDefault(a => a.Id == assignment.Id) != null)
            throw new InvalidOperationException($"Assignment with the ID : {assignment.Id} already exists.");
        else
            DataSource.Assignments.Add(assignment);
    }

    public Assignment? Read(int id)
    {
        if (DataSource.Assignments.Exists(a => a.Id == id))
            return DataSource.Assignments.Find(a => a.Id == id);
        else
            throw new InvalidOperationException($"Assignment with the ID : {id} does not exist...");
    }

    public List<Assignment> ReadAll()
    {
        return DataSource.Assignments;
    }

    public void Update(Assignment assignment)
    {
        int index = DataSource.Assignments.FindIndex(a => a.Id == assignment.Id);
        if (index == -1)
            throw new InvalidOperationException($"Assignment with the ID : {assignment.Id} does not exist...");
        else
            DataSource.Assignments[index] = assignment;
    }


    public void Delete(int id)
    {
        if (DataSource.Assignments.Exists(a => a.Id == id))
            DataSource.Assignments.RemoveAll(a => a.Id == id);
        else
            throw new InvalidOperationException($"Assignment with the ID : {id} does not exist...");
    }

    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }

 
}
