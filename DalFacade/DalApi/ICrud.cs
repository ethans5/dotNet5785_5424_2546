namespace DalApi
{
    /// <summary>
    /// Interface defining CRUD (Create, Read, Update, Delete) operations for managing entities of type T.
    /// </summary>
    public interface ICrud<T> where T : class
    {
        /// <summary>
        /// Adds a new entity to the data source.
        /// </summary>
        void Create(T item);

        /// <summary>
        /// Retrieves an entity by its unique identifier.
        /// </summary>
        T? Read(int id);

        /// <summary>
        /// Retrieves an entity that matches the specified condition.
        /// </summary>
        T? Read(Func<T, bool> filter);

        /// <summary>
        /// Retrieves all entities that match the specified condition, or all entities if no condition is provided.
        /// </summary>
        IEnumerable<T> ReadAll(Func<T, bool>? filter = null);

        /// <summary>
        /// Updates an existing entity in the data source.
        /// </summary>
        void Update(T item);

        /// <summary>
        /// Deletes an entity by its unique identifier.
        /// </summary>
        void Delete(int id);

        /// <summary>
        /// Deletes all entities from the data source.
        /// </summary>
        void DeleteAll();
    }
}
