using FluentNHibernate.DAL.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace FluentNHibernate.DAL.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFluentNHibernateDALServices(
        this IServiceCollection services, 
        IConfiguration configuration,
        Assembly assembly,
        string sectionKey)
    {
        services.Configure<DatabaseSettings>(configuration.GetSection(sectionKey));

        //services.AddSingleton<SessionFactoryHelper>();
        services.AddSingleton<SessionFactoryBuilder>(provider =>
        {
            var settings = provider.GetRequiredService<IOptions<DatabaseSettings>>();
            return new SessionFactoryBuilder(settings, assembly);
        });

        return services;
    }
}
