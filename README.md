# FluentNHibernate.DAL

A robust and flexible Data Access Layer (DAL) built with FluentNHibernate, providing a generic repository pattern implementation with advanced querying capabilities and comprehensive database management features.

## 🚀 Features

- **Generic Repository Pattern**: Complete CRUD operations with async/await support
- **Custom Repository Support**: Easily extend base functionality for entity-specific operations
- **Advanced Querying**: Support for custom SQL queries, pagination, and complex filtering
- **Batch Operations**: Efficient bulk insert, update, and delete operations
- **Schema Management**: Automated schema creation, updates, and validation
- **Migration Support**: Database versioning and migration capabilities
- **Transaction Support**: Built-in transaction handling and management
- **Fluent Configuration**: Easy-to-use fluent API for entity mappings
- **Cross-Database Support**: Works with multiple database providers (PostgreSQL, SQL Server, MySQL, etc.)

## 📋 Prerequisites

- .NET Core 3.1 or higher
- NHibernate 5.x or higher
- A supported database (PostgreSQL, SQL Server, MySQL, SQLite, Oracle)

## 📦 Installation

### NuGet Package Manager

```bash
Install-Package FluentNHibernate
Install-Package NHibernate
```

### .NET CLI

```bash
dotnet add package FluentNHibernate
dotnet add package NHibernate
```

### PackageReference

```xml
<PackageReference Include="FluentNHibernate" Version="3.1.0" />
<PackageReference Include="NHibernate" Version="5.4.6" />
```

## 🔧 Quick Start

### 1. Define Your Entity

```csharp
public class User
{
    public virtual int Id { get; set; }
    public virtual string Name { get; set; }
    public virtual string Email { get; set; }
    public virtual bool IsActive { get; set; }
}
```

### 2. Create Entity Mapping

```csharp
public class UserMap : ClassMap<User>
{
    public UserMap()
    {
        Table("Users");
        Id(x => x.Id);
        Map(x => x.Name).Length(100).Not.Nullable();
        Map(x => x.Email).Length(100).Not.Nullable();
        Map(x => x.IsActive).Not.Nullable();
    }
}
```

### 3. Configure Session Factory

```csharp
public static ISessionFactory CreateSessionFactory()
{
    return Fluently.Configure()
        .Database(PostgreSQLConfiguration.Standard
            .ConnectionString("your-connection-string"))
        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<User>())
        .BuildSessionFactory();
}
```

### 4. Use Repository

```csharp
// Basic operations
var repository = new NHibernateRepository<User>(sessionFactory);

// Insert
await repository.InsertAsync(new User { Name = "John Doe", Email = "john@example.com" });

// Get by ID
var user = await repository.GetAsync(1);

// Update
user.IsActive = true;
await repository.UpdateAsync(user);

// Delete
await repository.DeleteAsync(user);

// Custom queries
var activeUsers = await repository.GetByAsync(u => u.IsActive == true);
```

## 📚 Documentation

### Core Documentation

- **[Custom Repository](docs/Custom_Repository.md)** - Learn how to create custom repositories with entity-specific operations
- **[Entities and Mappings](docs/Entities_and_Mappings.md)** - Complete guide to defining entities and their relationships
- **[Migrations](docs/Migrations.md)** - Database migration strategies and implementation
- **[Schema Export](docs/SchemaExport.md)** - Schema management and database structure updates

### Key Topics Covered

- Repository pattern implementation
- One-to-many and many-to-many relationships
- Complex entity mappings
- Database migration strategies
- Transaction management
- Batch operations
- Custom SQL queries
- Pagination and filtering

## 🎯 Usage Examples

### Basic CRUD Operations

```csharp
// Create repository instance
var userRepository = new NHibernateRepository<User>(sessionFactory);

// Insert single entity
await userRepository.InsertAsync(new User
{
    Name = "Alice Johnson",
    Email = "alice@example.com"
});

// Insert multiple entities
var users = new List<User>
{
    new User { Name = "Bob Smith", Email = "bob@example.com" },
    new User { Name = "Carol Davis", Email = "carol@example.com" }
};
await userRepository.InsertAsync(users);

// Query with filtering
var activeUsers = await userRepository.GetByAsync(u => u.IsActive);

// Pagination
var pagedUsers = await userRepository.GetBySQLAsync(
    "SELECT * FROM Users WHERE IsActive = :active",
    new Dictionary<string, object> { { "active", true } },
    skip: 0,
    take: 10
);
```

### Custom Repository Implementation

```csharp
public interface IUserRepository : IRepository<User>
{
    Task<User> GetByEmailAsync(string email);
    Task<int> GetActiveUserCountAsync();
}

public class UserRepository : NHibernateRepository<User>, IUserRepository
{
    public UserRepository(ISessionFactory sessionFactory) : base(sessionFactory) { }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await GetByAsync(u => u.Email == email);
    }

    public async Task<int> GetActiveUserCountAsync()
    {
        return await GetCountByAsync(u => u.IsActive);
    }
}
```

## 🏗️ Architecture

```
FluentNHibernate.DAL/
├── Configuration/          # Database and session configuration
├── Infrastructure/         # Core repository implementations
├── Repositories/          # Custom repository implementations
├── Utils/                # Utility classes and helpers
└── docs/                 # Documentation files
```

## 🤝 Contributing

We welcome contributions! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Development Guidelines

- Follow C# coding conventions
- Include unit tests for new features
- Update documentation as needed
- Ensure all tests pass before submitting PR

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- [FluentNHibernate](https://github.com/nhibernate/fluent-nhibernate) - The awesome ORM that makes this possible
- [NHibernate](https://nhibernate.info/) - The underlying ORM framework
- Community contributors and testers

## 📞 Support

- Create an [Issue](../../issues) for bug reports or feature requests
- Check the [Documentation](docs/) for detailed guides
- Review [Examples](docs/Custom_Repository.md#usage-in-a-service) for implementation patterns

## 🔖 Version History

- **v1.0.0** - Initial release with core repository functionality
- **v1.1.0** - Added batch operations and custom SQL support
- **v1.2.0** - Enhanced migration support and schema management

---

**Made with ❤️ for the .NET community**
