Fluent NHibernate does not natively include migrations like Entity Framework, but you can implement migrations in your Fluent NHibernate setup using a combination of schema generation tools like **NHibernate's `SchemaUpdate`** and external libraries such as **FluentMigrator**.

Here are a few approaches to handle migrations in Fluent NHibernate using a **Code-First** approach:

### 1. Using NHibernate's Built-in Schema Tools

NHibernate offers built-in schema tools like `SchemaExport`, `SchemaUpdate`, and `SchemaValidator`. However, these are quite basic and should be used with caution, especially in production environments. Here's how you can use them for managing migrations:

#### SchemaUpdate (For updating the schema)
`SchemaUpdate` can be used to update the database schema based on the current Fluent NHibernate mappings without deleting data.

**Setup Example:**
```csharp
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Tool.hbm2ddl;

public class SessionFactoryHelper
{
    public static ISessionFactory CreateSessionFactory()
    {
        return Fluently.Configure()
            .Database(PostgreSQLConfiguration.Standard
                .ConnectionString("your-connection-string"))
            .Mappings(m => m.FluentMappings.AddFromAssemblyOf<YourEntity>())
            .ExposeConfiguration(cfg =>
            {
                // Update the schema based on the mappings
                var schemaUpdate = new SchemaUpdate(cfg);
                schemaUpdate.Execute(false, true);
            })
            .BuildSessionFactory();
    }
}
```

#### SchemaExport (For creating the schema)
`SchemaExport` generates a new schema by dropping existing tables and recreating them. This should only be used in a development environment or during initial database creation.

```csharp
.ExposeConfiguration(cfg =>
{
    var schemaExport = new SchemaExport(cfg);
    schemaExport.Create(false, true); // Use Drop(false, true) to drop and recreate
})
```

#### SchemaValidator (For validating the schema)
`SchemaValidator` ensures that the database schema matches your Fluent NHibernate mappings.

```csharp
.ExposeConfiguration(cfg =>
{
    var schemaValidator = new SchemaValidator(cfg);
    schemaValidator.Validate(); // Throws exception if schema doesn't match
})
```

### 2. Using FluentMigrator for Advanced Migrations

For a more sophisticated and production-friendly migration strategy, you can integrate **FluentMigrator**, a database migration framework that allows you to write migrations in C#. FluentMigrator supports versioned migrations and works well alongside Fluent NHibernate.

#### Step 1: Install FluentMigrator
You can install the FluentMigrator NuGet packages:

```bash
dotnet add package FluentMigrator
dotnet add package FluentMigrator.Runner
```

#### Step 2: Create Migrations

FluentMigrator uses classes to represent migrations. You can write these migrations by creating a migration class that inherits from `Migration`.

```csharp
using FluentMigrator;

[Migration(202309270001)]
public class CreateUsersTable : Migration
{
    public override void Up()
    {
        Create.Table("Users")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Name").AsString(100).NotNullable()
            .WithColumn("Email").AsString(100).NotNullable();
    }

    public override void Down()
    {
        Delete.Table("Users");
    }
}
```

#### Step 3: Configure FluentMigrator in Your Application

You'll need to configure FluentMigrator in your project, which can be done in your startup configuration or a dedicated migration runner.

```csharp
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

public class MigrationRunner
{
    public static void RunMigrations(string connectionString)
    {
        var serviceProvider = new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres() // or other DBs like AddSqlServer()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(CreateUsersTable).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider(false);

        using (var scope = serviceProvider.CreateScope())
        {
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }
    }
}
```

#### Step 4: Running Migrations

You can run the migrations by calling the `RunMigrations` method at the appropriate place in your application:

```csharp
MigrationRunner.RunMigrations("your-connection-string");
```

### 3. Manual Migrations with Custom SQL
You can also manually manage migrations by using custom SQL scripts or by extending your Fluent NHibernate mappings with SQL commands for schema alterations. This approach is not recommended unless you're dealing with very specific cases, but it offers full control over database changes.

### Summary

- **NHibernate Schema Tools (`SchemaUpdate`, `SchemaExport`, `SchemaValidator`)**: Great for quick schema generation and updates, but not ideal for production migrations.
- **FluentMigrator**: Provides robust, versioned, and repeatable migrations and is a better fit for production environments.
- **Custom SQL Migrations**: Gives full control but is more tedious to maintain.

For production-ready solutions, **FluentMigrator** is recommended for database versioning and migration management, while **NHibernate SchemaUpdate** can be useful in development or for small, controlled updates.