# FluentNHibernate.DAL

A robust and flexible Data Access Layer (DAL) built with FluentNHibernate, providing a generic repository pattern implementation with advanced querying capabilities and comprehensive database management features.

## 🚀 Features

- **Generic Repository Pattern**: Complete CRUD operations with async/await support
- **Custom Repository Support**: Easily extend base functionality for entity-specific operations
- **Code-First Approach**: Start with C# classes and let the database be auto-generated
- **Auto Mapping**: Automatic table and relationship creation from entity definitions
- **Advanced Querying**: Support for custom SQL queries, pagination, and complex filtering
- **Batch Operations**: Efficient bulk insert, update, and delete operations
- **Schema Management**: Automated schema creation, updates, and validation
- **Migration Support**: Database versioning and migration capabilities for keeping database in sync with code changes
- **Transaction Support**: Built-in transaction handling and management
- **Fluent Configuration**: Easy-to-use fluent API for entity mappings
- **Cross-Database Support**: Works with multiple database providers (PostgreSQL, SQL Server, MySQL, etc.)

## 🎯 Development Approaches

FluentNHibernate.DAL supports both **Code-First** and **Database-First** development approaches, giving you flexibility to choose the workflow that best fits your project needs.

### Code-First Approach

Start with C# entities and auto-generate database schema:

1. Define entity classes and mappings
2. Configure session factory with schema generation
3. Database tables are created automatically

**Benefits**: Rapid prototyping, version-controlled schema, consistent across environments.

📖 **Code-First Guides**: [Entity Mappings](docs/Entities_and_Mappings.md) | [Schema Generation](docs/SchemaExport.md)

### Database-First Approach

Work with existing databases by mapping to established schema:

1. Connect to existing database
2. Create entity classes matching database structure
3. Define mappings that align with current schema

**Benefits**: Integration with legacy systems, preserve existing data, work with established database designs.

📖 **Database-First Guides**: [Entity Mappings](docs/Entities_and_Mappings.md) | [Custom Repositories](docs/Custom_Repository.md)

📖 **Learn More**:

- [Entity Definitions & Relationships](docs/Entities_and_Mappings.md) - Complete guide for both approaches
- [Schema Generation Options](docs/SchemaExport.md) - Code-first schema management
- [Database Migration Strategies](docs/Migrations.md) - Evolution strategies for both approaches

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

### 1. Define Entity & Mapping

```csharp
public class User
{
    public virtual int Id { get; set; }
    public virtual string Name { get; set; }
    public virtual string Email { get; set; }
    public virtual bool IsActive { get; set; }
}

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

📖 **Learn More**: [Entity Definitions & Complex Relationships](docs/Entities_and_Mappings.md)

### 2. Configure Session Factory

```csharp
public static ISessionFactory CreateSessionFactory()
{
    return Fluently.Configure()
        .Database(PostgreSQLConfiguration.Standard.ConnectionString("your-connection-string"))
        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<User>())
        .ExposeConfiguration(cfg => new SchemaExport(cfg).Create(false, true)) // Auto-create tables
        .BuildSessionFactory();
}
```

📖 **Learn More**: [Schema Generation & Management](docs/SchemaExport.md) | [Migration Strategies](docs/Migrations.md)

### 3. Use Repository

```csharp
var repository = new NHibernateRepository<User>(sessionFactory);
await repository.InsertAsync(new User { Name = "John Doe", Email = "john@example.com" });
var user = await repository.GetAsync(1);
```

📖 **Learn More**: [Custom Repository Patterns](docs/Custom_Repository.md)

📖 **Detailed Guides**: [Entity Mappings](docs/Entities_and_Mappings.md) | [Schema Management](docs/SchemaExport.md) | [Custom Repositories](docs/Custom_Repository.md)

## 📚 Documentation

| Guide                                                    | Description                                                 |
| -------------------------------------------------------- | ----------------------------------------------------------- |
| **[Custom Repository](docs/Custom_Repository.md)**       | Entity-specific operations and advanced repository patterns |
| **[Entities & Mappings](docs/Entities_and_Mappings.md)** | Define entities, relationships, and complex mappings        |
| **[Migrations](docs/Migrations.md)**                     | Database versioning and schema evolution strategies         |
| **[Schema Export](docs/SchemaExport.md)**                | Automated schema generation and management                  |
| **[NuGet Deployment](docs/NuGet_Deployment.md)**         | Complete guide to building and deploying NuGet packages     |

## 🎯 Usage Examples

### Basic Operations

```csharp
var repository = new NHibernateRepository<User>(sessionFactory);

// CRUD operations
await repository.InsertAsync(new User { Name = "Alice", Email = "alice@example.com" });
var user = await repository.GetAsync(1);
await repository.UpdateAsync(user);
await repository.DeleteAsync(user);

// Filtering & pagination
var activeUsers = await repository.GetByAsync(u => u.IsActive);
var pagedUsers = await repository.GetBySQLAsync("SELECT * FROM Users WHERE IsActive = :active",
    new Dictionary<string, object> { { "active", true } }, 0, 10);
```

### Custom Repository

```csharp
public class UserRepository : NHibernateRepository<User>, IUserRepository
{
    public async Task<User> GetByEmailAsync(string email) =>
        await GetByAsync(u => u.Email == email);

    public async Task<int> GetActiveUserCountAsync() =>
        await GetCountByAsync(u => u.IsActive);
}
```

📖 **More Examples**: [Custom Repository Guide](docs/Custom_Repository.md#usage-in-a-service) | [Entity Relationship Examples](docs/Entities_and_Mappings.md)

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
- Check our [Documentation](docs/) for detailed guides
- Review [Custom Repository Examples](docs/Custom_Repository.md#usage-in-a-service)
- Learn about [Database Migrations](docs/Migrations.md) for production deployments

---

**Made with ❤️ for the .NET community**
