using DalApi;
using DO;

namespace Helpers;

internal static class AssignmentManager
{
    private static IDal _dal = Factory.Get; //stage 4
    internal static ObserverManager Observers = new();

    internal static void UpdateEndTreatment(DateTime newTime)
    {
        bool assignmentUpdated = false; // Indicateur pour suivre les modifications

        lock (AdminManager.blMutex) // Utilisation d'un verrou pour éviter les conflits multi-thread (stage 7)
        {
            var assignments = _dal.Assignment.ReadAll().ToList(); // Lire toutes les assignations

            foreach (var item in assignments)
            {
                var task = _dal.Call.Read(item.CallId); // Lire l'objet associé à l'assignation

                if (task?.MaxTime is not null && task.MaxTime <= newTime) // Vérifications combinées
                {
                    assignmentUpdated = true; // Indique qu'une modification a été faite

                    // Créer une copie avec les nouvelles valeurs (en utilisant "with")
                    var updatedAssignment = item with
                    {
                        endTreatment = newTime,
                        typeOfEnd = typeOfEndTreatment.Expired
                    };

                    // Mise à jour dans la base de données
                    _dal.Assignment.Update(updatedAssignment);

                    // Notifier l'observateur pour cet élément
                    Observers.NotifyItemUpdated(item.Id);
                }
            }
        }

        // Si une modification a été effectuée, notifier que la liste a changé
        if (assignmentUpdated)
        {
            Observers.NotifyListUpdated();
        }
    }

}
