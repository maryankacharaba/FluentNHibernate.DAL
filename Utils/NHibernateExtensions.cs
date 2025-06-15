using NHibernate;

namespace FluentNHibernate.DAL.Utils;

public static class NHibernateExtensions
{
    public static IList<dynamic> DynamicList(this IQuery query)
    {
        return query.SetResultTransformer(NhTransformers.ExpandoObject)
                    .List<dynamic>();
    }
}