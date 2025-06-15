using NHibernate;
using NHibernate.Linq;
using System.Linq.Expressions;

namespace FluentNHibernate.DAL.Repositories;

/// <summary>
/// Generic NHibernate repository implementing asynchronous CRUD operations.
/// </summary>
public class NHibernateRepository<T> : IRepository<T> where T : class
{
    private readonly ISessionFactory sessionFactory;

    public NHibernateRepository(ISessionFactory sessionFactory)
    {
        this.sessionFactory = sessionFactory;
    }

    public async Task<T> GetAsync(object id)
    {
        using (var session = sessionFactory.OpenSession())
        {
            return await session.GetAsync<T>(id);
        }
    }

    public async Task<IList<T>> GetAllAsync(int skip = 0, int take = int.MaxValue)
    {
        using (var session = sessionFactory.OpenSession())
        {
            return await session.Query<T>().Skip(skip).Take(take).ToListAsync();
        }
    }

    public async Task<IList<T>> GetByAsync(
        Expression<Func<T, bool>> predicate, int skip = 0, int take = int.MaxValue)
    {
        using (var session = sessionFactory.OpenSession())
        {
            return await session.Query<T>()
                .Where(predicate)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }
    }

    public async Task<int> GetCountByAsync(Expression<Func<T, bool>> predicate)
    {
        using (var session = sessionFactory.OpenSession())
        {
            var query = session.Query<T>().Where(predicate);
            var countSql = query.ToString()?.Replace("select", "SELECT COUNT(*)") ?? "SELECT COUNT(*) FROM " + typeof(T).Name;

            return await ExecuteScalarAsync<int>(countSql, new Dictionary<string, object>());
        }
    }


    public async Task<IList<T>> GetBySQLAsync(
        string sql,
        IDictionary<string, object>? parameters,
        int skip = 0,
        int take = int.MaxValue)
    {
        using (var session = sessionFactory.OpenSession())
        {
            var query = session.CreateSQLQuery(sql).AddEntity(typeof(T));
            SetParameters(query, parameters);

            return await query.SetFirstResult(skip).SetMaxResults(take).ListAsync<T>();
        }
    }

    public async Task<int> ExecuteNonQueryAsync(string sql, IDictionary<string, object>? parameters)
    {
        using (var session = sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            var query = session.CreateSQLQuery(sql);
            SetParameters(query, parameters);

            var result = await query.ExecuteUpdateAsync();
            await transaction.CommitAsync();

            return result;
        }
    }

    public async Task<TScalar> ExecuteScalarAsync<TScalar>(string sql, IDictionary<string, object>? parameters)
    {
        using (var session = sessionFactory.OpenSession())
        {
            var query = session.CreateSQLQuery(sql);
            SetParameters(query, parameters);
            return await query.UniqueResultAsync<TScalar>();
        }
    }

    public async Task InsertAsync(T entity)
    {
        using (var session = sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            await session.SaveAsync(entity);
            await transaction.CommitAsync();
        }
    }

    public async Task InsertAsync(IEnumerable<T> entities)
    {
        using (var session = sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            foreach (var entity in entities)
            {
                await session.SaveAsync(entity);
            }
            await transaction.CommitAsync();
        }
    }

    public async Task UpdateAsync(T entity)
    {
        using (var session = sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            await session.UpdateAsync(entity);
            await transaction.CommitAsync();
        }
    }

    public async Task UpdateAsync(IEnumerable<T> entities)
    {
        using (var session = sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            foreach (var entity in entities)
            {
                await session.UpdateAsync(entity);
            }
            await transaction.CommitAsync();
        }
    }

    public async Task InsertOrUpdateAsync(T entity)
    {
        using (var session = sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            await session.SaveOrUpdateAsync(entity);
            await transaction.CommitAsync();
        }
    }

    public async Task InsertOrUpdateAsync(IEnumerable<T> entities)
    {
        using (var session = sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            foreach (var entity in entities)
            {
                await session.SaveOrUpdateAsync(entity);
            }
            await transaction.CommitAsync();
        }
    }

    public async Task DeleteAsync(T entity)
    {
        using (var session = sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            await session.DeleteAsync(entity);
            await transaction.CommitAsync();
        }
    }

    public async Task DeleteAsync(IEnumerable<T> entities)
    {
        using (var session = sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            foreach (var entity in entities)
            {
                await session.DeleteAsync(entity);
            }
            await transaction.CommitAsync();
        }
    }


    private void SetParameters(IQuery query, IDictionary<string, object>? parameters)
    {
        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                query.SetParameter(param.Key, param.Value);
            }
        }
    }
}