using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.DAL.Configuration;
using HibernatingRhinos.Profiler.Appender.NHibernate;
using Microsoft.Extensions.Options;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FluentNHibernate.DAL.Infrastructure;

public class SessionFactoryBuilder
{
    private readonly ISessionFactory sessionFactory;

    public ISessionFactory SessionFactory => sessionFactory;

    public SessionFactoryBuilder(IOptions<DatabaseSettings> settings, Assembly assembly)
    {
        sessionFactory = CreateSessionFactory(settings.Value, assembly);
    }

    private static ISessionFactory CreateSessionFactory(DatabaseSettings settings, Assembly assembly)
    {
        var configuration = Fluently.Configure();

        switch (settings.DBType)
        {
            case DBType.Postgres:
                configuration.Database(PostgreSQLConfiguration.Standard
                    .ConnectionString(c => c
                        .Host(settings.Host)
                        .Port(settings.Port)
                        .Database(settings.Database)
                        .Username(settings.Username)
                        .Password(settings.Password)));
                break;
            case DBType.MSSQL:
                configuration.Database(MsSqlConfiguration.MsSql2012
                    .ConnectionString(c => c
                        .Server(settings.Host)
                        .Database(settings.Database)
                        .Username(settings.Username)
                        .Password(settings.Password)));
                break;
            case DBType.SQLite:
                configuration.Database(SQLiteConfiguration.Standard
                    .UsingFile(settings.DBFilePath));
                break;
            default:
                throw new InvalidOperationException("Unsupported database type");
        }

        return configuration
            .Mappings(m => m.FluentMappings.AddFromAssembly(assembly))
            .ExposeConfiguration(cfg =>
            {
                cfg.SetProperty(NHibernate.Cfg.Environment.BatchSize, settings.BatchSize.ToString());

                cfg.SetProperty(NHibernate.Cfg.Environment.ShowSql, settings.ShowSql.ToString().ToLower());
                cfg.SetProperty(NHibernate.Cfg.Environment.FormatSql, settings.ShowSql.ToString().ToLower());

                if (settings.UseProfiler)
                {
                    NHibernateProfiler.Initialize();
                }

                // SchemaUpdate.Execute(): Updates schema without dropping tables(data is preserved).                
                if (settings.UpdateSchema)
                {
                    var schemaUpdate = new SchemaUpdate(cfg);
                    schemaUpdate.Execute(settings.ShowSql, true);
                }
                // SchemaExport.Create(): Drops and recreates tables (data is lost).
                else if (settings.DropAndCreateSchema)
                {
                    var schemaExport = new SchemaExport(cfg);
                    schemaExport.Create(settings.ShowSql, true);
                }

                if (settings.ValidateSchema)
                {
                    var schemaValidator = new SchemaValidator(cfg);
                    schemaValidator.Validate(); // Throws exception if schema doesn't match
                }
            })
            .BuildSessionFactory();
    }
}