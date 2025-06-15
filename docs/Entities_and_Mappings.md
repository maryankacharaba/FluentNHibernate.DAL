
# Defining Entities and Mappings in Fluent NHibernate

In this section, we'll provide examples of defining entities and their relationships using Fluent NHibernate. We will cover:

- **One-to-Many Relationships**: Example where one entity has many related entities.
- **Many-to-Many Relationships**: Example where two entities have a many-to-many relationship through a join table.

## 1. Define Entities

Let's assume we have the following entities in a typical e-commerce domain: `User`, `Order`, and `Product`.

#### User Entity
The `User` entity has a one-to-many relationship with `Order` and a many-to-many relationship with `Product` (representing purchased products).

```csharp
public class User
{
    public virtual int Id { get; set; }
    public virtual string Email { get; set; }
    public virtual string Name { get; set; }
    public virtual IList<Order> Orders { get; set; } = new List<Order>();
    public virtual IList<Product> Products { get; set; } = new List<Product>();
}
```

#### Order Entity
The `Order` entity belongs to one `User` and may contain multiple `Product` entities.

```csharp
public class Order
{
    public virtual int Id { get; set; }
    public virtual DateTime OrderDate { get; set; }
    public virtual User User { get; set; }
    public virtual IList<Product> Products { get; set; } = new List<Product>();
}
```

#### Product Entity
The `Product` entity can be linked to many `Order` entities through a many-to-many relationship.

```csharp
public class Product
{
    public virtual int Id { get; set; }
    public virtual string Name { get; set; }
    public virtual decimal Price { get; set; }
    public virtual IList<Order> Orders { get; set; } = new List<Order>();
}
```

## 2. Define Mappings

Now we define Fluent NHibernate mappings for the entities and their relationships.

#### User Mapping
- The `User` has a one-to-many relationship with `Order` (`HasMany`).
- The `User` has a many-to-many relationship with `Product` through a join table (`HasManyToMany`).

```csharp
public class UserMap : ClassMap<User>
{
    public UserMap()
    {
        Table("Users");

        Id(x => x.Id);
        Map(x => x.Email).Length(100).Not.Nullable();
        Map(x => x.Name).Length(100).Not.Nullable();

        // One-to-Many relationship
        HasMany(x => x.Orders)
            .Cascade.All() // Cascade operations to Orders
            .Inverse() // Orders are responsible for the relationship
            .KeyColumn("UserId");

        // Many-to-Many relationship with Product
        HasManyToMany(x => x.Products)
            .Cascade.All() // Cascade operations to Products
            .Table("UserProducts") // Join table
            .ParentKeyColumn("UserId") // FK to User
            .ChildKeyColumn("ProductId"); // FK to Product
    }
}
```

#### Order Mapping
- The `Order` has a many-to-one relationship with `User` (`References`).
- The `Order` has a many-to-many relationship with `Product` (`HasManyToMany`).

```csharp
public class OrderMap : ClassMap<Order>
{
    public OrderMap()
    {
        Table("Orders");

        Id(x => x.Id);
        Map(x => x.OrderDate).Not.Nullable();

        // Many-to-One relationship with User
        References(x => x.User)
            .Column("UserId")
            .Cascade.None();

        // Many-to-Many relationship with Product
        HasManyToMany(x => x.Products)
            .Cascade.All()
            .Table("OrderProducts") // Join table
            .ParentKeyColumn("OrderId") // FK to Order
            .ChildKeyColumn("ProductId"); // FK to Product
    }
}
```

#### Product Mapping
- The `Product` has a many-to-many relationship with both `Order` and `User`.

```csharp
public class ProductMap : ClassMap<Product>
{
    public ProductMap()
    {
        Table("Products");

        Id(x => x.Id);
        Map(x => x.Name).Length(100).Not.Nullable();
        Map(x => x.Price).Not.Nullable();

        // Many-to-Many relationship with Order
        HasManyToMany(x => x.Orders)
            .Cascade.None()
            .Table("OrderProducts")
            .ParentKeyColumn("ProductId")
            .ChildKeyColumn("OrderId");
    }
}
```

### Example of the Relationship

In this example:
- **One-to-Many**: A `User` can place many `Orders`, and each `Order` belongs to one `User`.
- **Many-to-Many**: A `User` can purchase many `Products`, and a `Product` can be purchased in many `Orders`. This relationship is represented using the `UserProducts` and `OrderProducts` join tables.

### 3. Saving Data

Here’s an example of saving entities with relationships:

```csharp
var user = new User
{
    Name = "John Doe",
    Email = "john@example.com",
    Orders = new List<Order>
    {
        new Order
        {
            OrderDate = DateTime.Now,
            Products = new List<Product>
            {
                new Product { Name = "Laptop", Price = 1200M },
                new Product { Name = "Mouse", Price = 25M }
            }
        }
    }
};

await userRepository.InsertAsync(user);
```

### Conclusion

With Fluent NHibernate, you can easily define one-to-many and many-to-many relationships using the `HasMany` and `HasManyToMany` methods. This approach makes it easier to handle complex data models with ease and flexibility.
