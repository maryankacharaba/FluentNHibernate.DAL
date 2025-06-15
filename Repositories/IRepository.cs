using System.Linq.Expressions;

namespace FluentNHibernate.DAL.Repositories;

/// <summary>
/// Generic repository interface for performing CRUD operations.
/// </summary>
/// <typeparam name="T">Type of entity this repository manages.</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Asynchronously retrieves an entity by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity.</param>
    /// <returns>The entity instance or null if not found.</returns>
    Task<T> GetAsync(object id);

    /// <summary>
    /// Retrieves all entities of type T with optional pagination.
    /// </summary>
    /// <param name="skip">Number of entities to skip.</param>
    /// <param name="take">Number of entities to take.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a list of entities of type T.</returns>
    /// <example>
    /// Usage:
    /// <code>
    /// var allUsers = await repository.GetAllAsync(0, 10); // Retrieves the first 10 users.
    /// </code>
    /// </example>
    Task<IList<T>> GetAllAsync(int skip = 0, int take = int.MaxValue);

    /// <summary>
    /// Retrieves entities based on a given condition with optional pagination.
    /// </summary>
    /// <param name="predicate">A lambda expression to define the condition for querying the entities.</param>
    /// <param name="skip">Number of entities to skip (optional).</param>
    /// <param name="take">Number of entities to take (optional).</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a list of entities that match the condition.</returns>
    /// <example>
    /// Usage:
    /// <code>
    /// var users = await repository.GetByAsync(u => u.IsActive, 0, 10); // Get first 10 active users.
    /// </code>
    /// </example>
    Task<IList<T>> GetByAsync(Expression<Func<T, bool>> predicate, int skip = 0, int take = int.MaxValue);

    /// <summary>
    /// Retrieves the count of entities based on a given condition.
    /// </summary>
    /// <param name="predicate">A lambda expression to define the condition for counting the entities.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the count of entities that match the condition.</returns>
    /// <example>
    /// Usage:
    /// <code>
    /// var activeUserCount = await repository.GetCountByAsync(u => u.IsActive);
    /// </code>
    /// </example>
    Task<int> GetCountByAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Asynchronously saves a new entity to the database.
    /// Save should be used only for new entities.
    /// </summary>
    /// <param name="entity">The entity to save.</param>
    Task InsertAsync(T entity);

    /// <summary>
    /// Asynchronously saves multiple new entities to the database.
    /// </summary>
    /// <param name="entities">The collection of entities to save.</param>
    Task InsertAsync(IEnumerable<T> entities);

    /// <summary>
    /// Asynchronously updates an existing entity in the database.
    /// Update should be used for entities that are already persisted.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Asynchronously updates multiple existing entities in the database.
    /// </summary>
    /// <param name="entities">The collection of entities to update.</param>
    Task UpdateAsync(IEnumerable<T> entities);

    /// <summary>
    /// Asynchronously saves a new entity or updates an existing one in the database.
    /// It decides whether to save or update based on the unsaved-value definition or the version/identifier property.
    /// </summary>
    /// <param name="entity">The entity to save or update.</param>
    Task InsertOrUpdateAsync(T entity);

    /// <summary>
    /// Asynchronously saves or updates multiple entities in the database.
    /// </summary>
    /// <param name="entities">The collection of entities to save or update.</param>
    Task InsertOrUpdateAsync(IEnumerable<T> entities);

    /// <summary>
    /// Asynchronously deletes an entity from the database.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    Task DeleteAsync(T entity);

    /// <summary>
    /// Asynchronously deletes multiple entities from the database.
    /// </summary>
    /// <param name="entities">The collection of entities to delete.</param>
    Task DeleteAsync(IEnumerable<T> entities);

    /// <summary>
    /// Executes a SQL query and returns a list of entities of type T with pagination.
    /// </summary>
    /// <param name="sql">The SQL query to execute.</param>
    /// <param name="parameters">A dictionary of parameters to pass to the SQL query.</param>
    /// <param name="skip">Number of entities to skip.</param>
    /// <param name="take">Number of entities to take.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a list of entities of type T.</returns>
    /// <example>
    /// Usage:
    /// <code>
    /// string sql = "SELECT * FROM Users WHERE DepartmentId = :deptId";
    /// var parameters = new Dictionary<string, object> { { "deptId", 10 } };
    /// var users = await repository.GetBySQLAsync(sql, parameters, 0, 10); // Retrieves the first 10 users.
    /// </code>
    /// </example>
    Task<IList<T>> GetBySQLAsync(string sql, IDictionary<string, object> parameters, int skip = 0, int take = int.MaxValue);


    /// <summary>
    /// Executes a SQL query and returns a scalar value of the specified type.
    /// </summary>
    /// <typeparam name="TScalar">The type of the scalar result.</typeparam>
    /// <param name="sql">The SQL query to execute.</param>
    /// <param name="parameters">A dictionary of parameters to pass to the SQL query.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the scalar value of type TScalar.</returns>
    /// <example>
    /// This example shows how to use <see cref="ExecuteScalarAsync{TScalar}"/> to count active users:
    /// <code>
    /// var sql = "SELECT COUNT(*) FROM Users WHERE IsActive = :isActive";
    /// var parameters = new Dictionary&lt;string, object&gt;
    /// {
    ///     {"isActive", true}
    /// };
    /// int activeUserCount = await repository.ExecuteScalarAsync&lt;int&gt;(sql, parameters);
    /// </code>
    /// </example>
    Task<TScalar> ExecuteScalarAsync<TScalar>(string sql, IDictionary<string, object> parameters);


    /// <summary>
    /// Executes a custom SQL update or delete (non-query) command.
    /// </summary>
    /// <param name="sql">The SQL update command.</param>
    /// <param name="parameters">A dictionary of parameters for the SQL command.</param>
    /// <returns>The number of affected records.</returns>
    /// <example>
    /// <code>
    /// string sql = "UPDATE Employees SET Status = :status WHERE EmployeeId = :empId";
    /// var parameters = new Dictionary<string, object> { { "status", "Active" }, { "empId", 123 } };
    /// int updatedCount = await repository.ExecuteUpdateAsync(sql, parameters);
    /// </code>
    /// </example>
    Task<int> ExecuteNonQueryAsync(string sql, IDictionary<string, object> parameters);
}