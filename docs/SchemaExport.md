If you use `SchemaExport` with the `Create(false, true)` method, it will **drop** and **recreate** the database schema every time it runs. This means it will **delete** all the tables, along with the data in them, and then recreate the schema based on your current Fluent NHibernate mappings.

### Breakdown of Parameters for `SchemaExport.Create()`
```csharp
schemaExport.Create(false, true);
```
- **`false`**: This means not to output the SQL to the console or log.
- **`true`**: This indicates that the schema will be **executed** against the database, dropping and recreating the schema.

**Important Note**: **Even if the table definitions have not changed**, running `SchemaExport.Create(false, true)` will still drop and recreate the tables, resulting in the loss of any existing data in the database.

### When to Use `SchemaExport`
- **Development environments**: Useful when starting fresh, where dropping the schema and recreating it with initial data is acceptable.
- **Not recommended for production**: Since it **drops** tables, it will result in data loss.

### For Production: Use `SchemaUpdate`
If you want to **update the schema** without losing data (and without dropping and recreating the entire schema), use `SchemaUpdate` instead:
```csharp
var schemaUpdate = new SchemaUpdate(cfg);
schemaUpdate.Execute(false, true);
```

- **`SchemaUpdate`** only alters the schema based on changes in your mappings without dropping tables or affecting existing records. It is much safer to use when you have existing data that must be preserved.

### Summary:
- **`SchemaExport.Create()`**: Drops and recreates tables (data is lost).
- **`SchemaUpdate.Execute()`**: Updates schema without dropping tables (data is preserved).

In most cases, for production and environments where you want to preserve data, you should use `SchemaUpdate` instead of `SchemaExport`.

---

When you use **`SchemaUpdate.Execute()`** in NHibernate, it works as follows:

### 1. **Creates Tables If They Do Not Exist**
- If a table or column defined in your Fluent NHibernate mappings does not exist in the database, **`SchemaUpdate`** will **create** it.
  
### 2. **Modifies Tables If They Differ**
- If a table already exists but its structure (columns, data types, constraints, etc.) differs from the current mappings, **`SchemaUpdate`** will attempt to **modify** the table to match the mappings.

However, **SchemaUpdate** is limited in the changes it can automatically handle. Here’s what it does and does not do:

### What **`SchemaUpdate`** Can Do:
- **Add new tables**: If a table is missing in the database but is defined in the mappings, it will be created.
- **Add new columns**: If a column is missing from an existing table, it will be added.
- **Modify simple changes**: It can handle simple changes like changing a column's length or adding new constraints.

### What **`SchemaUpdate`** Cannot Do:
- **Dropping columns**: If a column exists in the database but is no longer defined in your mappings, **`SchemaUpdate`** does **not** drop it.
- **Renaming columns or tables**: It cannot detect if a column or table was renamed; it will just try to add the new ones based on the mappings.
- **Complex schema changes**: It does not handle more complex modifications like altering constraints, changing primary keys, or migrating data.

### Key Points:
- **Safe for production**: It will update the schema without dropping existing tables or losing data. However, its ability to handle schema modifications is limited to non-destructive operations.
- **Might require manual changes**: If you have complex schema changes (like renaming columns or dropping unused columns), you will need to manually apply these changes with SQL scripts or a migration tool like **FluentMigrator**.

### Example:
```csharp
var schemaUpdate = new SchemaUpdate(cfg);
schemaUpdate.Execute(false, true);
```
- **Creates** tables/columns if they don’t exist.
- **Modifies** tables/columns if they are different (within its capabilities).
- **Does not drop** or rename existing tables/columns.

### When to Use **`SchemaUpdate`**:
- For environments where you need to **preserve existing data**.
- For development or production environments where you need to make incremental changes to your database schema based on mappings.

For more complex schema changes or versioning, you might want to use a tool like **FluentMigrator** to manage database migrations in a more controlled manner.