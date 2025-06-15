

# Custom Repository with NHibernate

## Prerequisites

- .NET Core 3.1 or higher.
- NHibernate 5.x or higher.
- Existing database and NHibernate configurations.

## How to Create a Custom Nested Repository


Here’s the revised version of the custom repository implementation using the newly introduced `GetByAsync` and `GetCountByAsync` methods, allowing more flexibility and reusability.

#### 1. Extend the `NHibernateRepository<T>`
Create a custom repository by inheriting from `NHibernateRepository<T>`, leveraging the `GetByAsync` and `GetCountByAsync` methods.

**Custom Repository Interface:**
```csharp
public interface IUserRepository : IRepository<User> {
    Task<User> GetByEmailAsync(string email);
    Task<int> GetActiveUserCountAsync();
}
```

**Custom Repository Implementation:**
```csharp
public class UserRepository : NHibernateRepository<User>, IUserRepository {
    public UserRepository(ISessionFactory sessionFactory) : base(sessionFactory) {}

    public async Task<User> GetByEmailAsync(string email) {
        return await GetByAsync(u => u.Email == email);
    }

    public async Task<int> GetActiveUserCountAsync() {
        return await GetCountByAsync(u => u.IsActive);
    }
}
```

#### 2. Usage in a Service
You can now inject and use your custom repository, making use of `GetByEmailAsync` and `GetActiveUserCountAsync`.

```csharp
public class UserService {
    private readonly IUserRepository userRepository;

    public UserService(IUserRepository userRepository) {
        this.userRepository = userRepository;
    }

    public async Task<User> FindUserByEmail(string email) {
        return await userRepository.GetByEmailAsync(email);
    }

    public async Task<int> GetActiveUsersCount() {
        return await userRepository.GetActiveUserCountAsync();
    }
}
```

### Summary
- **Base Class**: `UserRepository` extends `NHibernateRepository<User>`, reusing generic methods.
- **Custom Logic**: Use `GetByAsync` and `GetCountByAsync` for entity-specific queries like fetching a user by email or counting active users.
- **Dependency Injection**: Easily inject the custom repository in services to maintain a clean architecture.

This approach maximizes reuse and maintains clean, scalable, and maintainable code by leveraging the generic repository for both fetching entities and executing counts.



## Repository Methods Overview

### CRUD Operations

#### Insert Entity

```csharp
await repository.InsertAsync(new User { Name = "John Doe", Email = "john@example.com" });
```

#### Get Entity by ID

```csharp
var user = await repository.GetAsync(userId);
```

#### Update Entity

```csharp
user.Name = "Jane Doe";
await repository.UpdateAsync(user);
```

#### Delete Entity

```csharp
await repository.DeleteAsync(user);
```

### Batch Operations

#### Insert Multiple Entities

```csharp
var users = new List<User>
{
    new User { Name = "Alice", Email = "alice@example.com" },
    new User { Name = "Bob", Email = "bob@example.com" }
};
await repository.InsertAsync(users);
```

#### Update Multiple Entities

```csharp
users.ForEach(u => u.IsActive = true);
await repository.UpdateAsync(users);
```

#### Delete Multiple Entities

```csharp
await repository.DeleteAsync(users);
```

### Custom SQL Queries

#### Get Entities by Custom SQL with Pagination

```csharp
string sql = "SELECT * FROM Users WHERE IsActive = :isActive";
var parameters = new Dictionary<string, object> { { "isActive", true } };
var activeUsers = await repository.GetBySQLAsync(sql, parameters, 0, 10);
```

### Scalar Queries

#### Execute Scalar

```csharp
string countSql = "SELECT COUNT(*) FROM Users WHERE IsActive = :isActive";
var countParams = new Dictionary<string, object> { { "isActive", true } };
int activeUserCount = await repository.ExecuteScalarAsync<int>(countSql, countParams);
```

### Non-Query Operations (Update/Delete)

#### Execute Non-Query

```csharp
string updateSql = "UPDATE Users SET LastLogin = CURRENT_TIMESTAMP WHERE Id = :userId";
var updateParams = new Dictionary<string, object> { { "userId", userId } };
int rowsAffected = await repository.ExecuteNonQueryAsync(updateSql, updateParams);
```

## Advanced Usage

### Pagination Example

Fetch paginated lists of entities:

```csharp
// Get the first 10 active users
var paginatedUsers = await repository.GetBySQLAsync(
    "SELECT * FROM Users WHERE IsActive = :isActive",
    new Dictionary<string, object> { { "isActive", true } },
    0, // skip
    10 // take
);
```

### Handling Transactions

It's recommended to handle transactions outside of the repository methods if multiple operations need to be grouped. NHibernate sessions manage transactions, which can be initiated before calling repository methods and committed or rolled back based on the outcome of the operations.

```csharp
using (var transaction = session.BeginTransaction())
{
    try
    {
        await repository.InsertAsync(new User { ... });
        await repository.UpdateAsync(existingUser);
        transaction.Commit();
    }
    catch (Exception)
    {
        transaction.Rollback();
        throw;
    }
}
```



